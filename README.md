# Metamorphic

Metamorphic is a .NET workflow system based on the [StackStorm](https://stackstorm.com/) concept.

Metamorphic listens for incoming messages, evaluates the message against one or more rules and then
executes an action if the message satisfies .


## Parts

* Actions
* Rules
* Signals


### Actions


### Rules

Rules define a relation between signals and actions. Each rule describes which signal types will be
evaluated and what action will be executed when the rule is satisfied.

Rules are described in YAML rule files. The format is largely the same as the format used by
[StackStorm](https://stackstorm.com). An example is given below:

    ---
        name: "rule_name"
        description: "Rule description."
        enabled: true

        signal:
            id: "signal_type_ref"
            parameters:
                foo: "bar"
                baz: "otherBar"

        condition:
            - name: "signal_parameter_name1"
            type: "matchregex"
            pattern : "^value$"
            - name: "signal_parameter_name2"
            type: "iequals"
            pattern : "watchevent"

        action:
            id: "action_ref"
            parameters:
                foo: "bar"
                baz: "{{signal.signal_parameter_1}}"

Rules are scanned and processed either from

* A NuGet package placed in a NuGet feed known by Metamorphic
* An `.mmrule` file in a directory known by Metamorphic

This ensures that rules can be versioned in some form (rule files can be committed to a version control
system and packages or file delivered according to standard software release and deploy methods)


### Signals


## Installing

* Need a RabbitMQ instance somewhere. Add a user that is allowed to create a queue. Default queue name will be
  `Metamorphic.Signals`. An additional error queue may be created called `Queue EasyNetQ_Default_Error_Queue`.


### Metamorphic.Signal.Http



### Metamorphic.Storage

* Windows service
* Configuration:
  * NuGet feeds that contain the actions and rules packages
  * Directory that can contain the rule files (should be local machine for the moment)


### Metamorphic.Server

* Windows service
* Configuration:
  * NuGet feeds that contain the action packages
  * Address of the RabbitMQ instance


## How to build

To build the project invoke MsBuild on the `entrypoint.msbuild` script in the repository root directory. This will
build the visual studio solution, run the unit tests and create the NuGet packages and ZIP archives. Final artifacts
will be placed in the `build\deploy` directory.

The build script assumes that:

* The connection to the repository is available so that the version number can be obtained via
  [GitVersion](https://github.com/GitTools/GitVersion) tool.
* The [NuGet command line executable](http://dist.nuget.org/win-x86-commandline/latest/nuget.exe) is available
  from the PATH so that the build can restore all the NuGet packages.
* The GIT executable is availabe from the PATH so that the build can gather information about the current branch and
  commit ID.


## How to contribute

There are many ways to contribute to the project:

* By opening an [issue](https://github.com/nbuildkit/nBuildKit.MsBuild/issues/new) and describing the issue
  that occurred or the feature that would make things better.
* By providing a [pull-request](https://github.com/nbuildkit/nBuildKit.MsBuild/pulls) for a new feature or
  a bug.

Any suggestions or improvements you may have are more than welcome.
