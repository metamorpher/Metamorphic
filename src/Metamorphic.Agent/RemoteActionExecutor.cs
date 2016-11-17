//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Metamorphic.Core;
using Metamorphic.Core.Actions;
using Metamorphic.Core.Jobs;
using Metamorphic.Agent.Properties;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Agent
{
    internal class RemoteActionExecutor : MarshalByRefObject, IExecuteActions
    {
        /// <summary>
        /// Loads the <see cref="Type"/> for the given identity.
        /// </summary>
        /// <param name="assemblyQualifiedTypeName">The assembly qualified name of the type that should be loaded.</param>
        /// <returns>The requested type.</returns>
        /// <exception cref="UnableToLoadActionTypeException">
        ///     Thrown when something goes wrong while loading the assembly containing the type or the type.
        /// </exception>
        private static Type LoadType(string assemblyQualifiedTypeName)
        {
            try
            {
                return Type.GetType(assemblyQualifiedTypeName, true, false);
            }
            catch (TargetInvocationException e)
            {
                // Type initializer throw an exception
                throw new UnableToLoadActionTypeException(Resources.Exceptions_Messages_UnableToLoadActionType, e);
            }
            catch (TypeLoadException e)
            {
                // Type is not found, typeName contains invalid characters, typeName represents an array type with an invalid size,
                // typeName represents and array of TypedReference
                throw new UnableToLoadActionTypeException(Resources.Exceptions_Messages_UnableToLoadActionType, e);
            }
            catch (ArgumentException e)
            {
                // typeName contains invalid syntax, typeName represents a generic type that has a pointer, a ByRef type or Void as one of
                // its type arguments, typeName represents a generic type that has an incorrect number of type arguments, typeName
                // represents a generic type, and one of its arguments does not satisfy the constraints for the corresponding type parameter
                throw new UnableToLoadActionTypeException(Resources.Exceptions_Messages_UnableToLoadActionType, e);
            }
            catch (FileNotFoundException e)
            {
                // The assembly or one of its dependencies was not found
                throw new UnableToLoadActionTypeException(Resources.Exceptions_Messages_UnableToLoadActionType, e);
            }
            catch (BadImageFormatException e)
            {
                // The assembly or one of its dependencies was not valid
                throw new UnableToLoadActionTypeException(Resources.Exceptions_Messages_UnableToLoadActionType, e);
            }
        }

        private static ActionParameterValueMap[] ToParameterData(Job job)
        {
            var namedParameters = new List<ActionParameterValueMap>();
            foreach (var name in job.ParameterNames())
            {
                var value = job.ParameterValue(name);

                namedParameters.Add(
                    new ActionParameterValueMap(
                        new ActionParameterDefinition(name),
                        value));
            }

            return namedParameters.ToArray();
        }

        /// <summary>
        /// The object that is used to create a dependency injection container.
        /// </summary>
        private readonly ContainerBuilder _builder;

        /// <summary>
        /// The object that provides logging for the current Appdomain.
        /// </summary>
        private readonly ILogMessagesFromRemoteAppDomains _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteActionExecutor"/> class.
        /// </summary>
        /// <param name="builder">The container builder that is used to create the dependency injection container.</param>
        /// <param name="logger">The object that provides the logging for the current <c>AppDomain</c>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public RemoteActionExecutor(ContainerBuilder builder, ILogMessagesFromRemoteAppDomains logger)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _builder = builder;
            _logger = logger;
        }

        /// <summary>
        /// Executes the actions necessary to complete the given job.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <param name="action">The action that should be executed for the job.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Don't know what the code will throw at us so Exception is the best thing we can catch.")]
        public void Execute(Job job, ActionDefinition action)
        {
            if (job == null)
            {
                return;
            }

            if (action == null)
            {
                return;
            }

            try
            {
                var parameters = ToParameterData(job);

                var actionParameters = action.Parameters;
                var mappedParameterValues = new object[actionParameters.Count];

                var i = -1;
                foreach (var expectedParameter in actionParameters)
                {
                    i++;

                    var providedParameter = parameters.FirstOrDefault(
                        m => string.Equals(m.Parameter.Name, expectedParameter.Name, StringComparison.OrdinalIgnoreCase));
                    if (providedParameter == null)
                    {
                        throw new MissingActionParameterException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Exceptions_Messages_MissingActionParameter_WithParameterName,
                                expectedParameter.Name));
                    }

                    mappedParameterValues[i] = providedParameter.Value;
                }

                var type = LoadType(action.ActionType);
                _builder.RegisterType(type);
                _builder.RegisterAssemblyTypes(type.Assembly)
                    .Where(t => !t.IsAbstract && typeof(IProvideConfigurationKeys).IsAssignableFrom(t));

                var container = _builder.Build();

                var obj = container.Resolve(type);

                var actionToExecute = type.GetMethod(action.ActionMethod);
                actionToExecute.Invoke(obj, mappedParameterValues);
            }
            catch (Exception e)
            {
                _logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_RemoteActionExecutor_ProcessJobFailed_WithId_WithException,
                        job.Action,
                        e.ToString()));
            }
        }
    }
}
