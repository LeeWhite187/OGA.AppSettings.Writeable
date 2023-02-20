# OGA.AppSettings.Writeable
Provides write access to Json files loaded by ConfigurationBuilder

## Description
This library contains the classes and logic necessary for runtime-update of json config files that are consumed by ConfigurationBuilder.
This allows json files, such as appsettings.json (loaded by a NET Core service), to be edited from within the service, and changes take immediate effect.

## Installation
OGA.AppSettings.Writeable is available via NuGet:
* NuGet Official Releases: [![NuGet](https://img.shields.io/nuget/vpre/OGA.AppSettings.Writeable.svg?label=NuGet)](https://www.nuget.org/packages/OGA.AppSettings.Writeable)

## Dependencies
This library depends on:
* [OGA.SharedKernel](https://github.com/LeeWhite187/OGA.SharedKernel)
* [NewtonSoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

## Usage
Using this library, requires a couple things:

If you want to allow runtime editing of json settings files that are automatically loaded by the runtime (such as appsettings.json), you will need to run the following replacer method call, from inside the lambda of the ConfigureAppConfiguration method.
Here's an example of what that looks like:
```
            // This call adds in our Writeable JSON config file...
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
            // Replace existing JSON config sources with own writeable JSON sources...
            OGA.AppSettings.Writeable.JsonConfigSource_Replacer.Replace_JSONConfigSources_with_Writeable_Sources(config);
            })
```

If you have additional json files that you want loaded by ConfigurationBuilder, and be runtime editable, add them with the writeablejsonfile extension method.
For example, the following snippet adds a json file (in the exe folder) named, 'config.json', and makes it runtime editable:
```
            // Add config.json file as runtime editable...
            config.AddWriteableJsonFile("config.json", optional: true, reloadOnChange: true);
```

## Building OGA.AppSettings.Writeable
This library is built with the new SDK-style projects.
It contains multiple projects, one for each of the following frameworks:
* NET 5
* NET 6

And, the output nuget package includes runtimes targets for:
* linux-64
* win-x64

## Framework and Runtime Support
Currently, the nuget package of this library supports the framework versions and runtimes of applications that I maintain (see above).
If someone needs others (older or newer), let me know, and I'll add them to the build script.

## Visual Studio
It is currently built using Visual Studio 2019 17.1.

## License
Please see the [License](LICENSE).
