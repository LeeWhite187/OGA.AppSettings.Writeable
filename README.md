# OGA.AppSettings.Writeable
Provides write access to Json files loaded by ConfigurationBuilder.
Also included is NET5 support for base64-encoded byte arrays, stored in configuration (such as appsettings.json). See Below.

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

It also requires a framework reference to: Microsoft.AspNetCore.App

## Usage
Editing json-file configuration at runtime, requires that associated FileProvider instances be writable, and that DI-registered config instances are writable.
Once that is done, mappable config properties can be updated at runtime.

### Writable FileProvider Setup
To set file providers for json config files as writeable, we can add them as writeable, or convert loaded ones to writeable.

#### Explicitly Add Writeable Json Config File
If you have additional json files that you want loaded by ConfigurationBuilder, and be runtime editable, you can add them during configuration build.
For example, the following snippet adds a json file (in the exe folder) named, 'config.json', and makes it runtime editable:
``` cs
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                        // Add config.json file as runtime editable...
                        config.AddWriteableJsonFile("config.json", optional: true, reloadOnChange: true);
            })
```

#### Convert Existing Json Config File to Writeable
If you want to allow runtime editing of json settings files that are automatically loaded by the runtime (such as appsettings.json), you will need to run the following replacer method call, from inside the lambda of the ConfigureAppConfiguration method.
Here's an example of what that looks like:
``` cs
            // This call adds in our Writeable JSON config file...
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
            // Replace existing JSON config sources with own writeable JSON sources...
            OGA.AppSettings.Writeable.JsonConfigSource_Replacer.Replace_JSONConfigSources_with_Writeable_Sources(config);
            })
```

### Register Writeable Config with DI
With the file providers of json config files set to writeable by one of the two above methods, you then need to DI-register the config class instances as writeable.
This is done using the ConfigureWritable extension method, in startup.cs.
For example, the following snippet (from startup.cs) retrieves a section of configuration, maps it to an app path class, and registers it as writeable with DI:
``` cs
            // Get application path configuration so that it's available to the process...
            IConfigurationSection apc = Configuration.GetSection(cConfig_AppPaths.CONSTANT_SectionName);
            services.ConfigureWritable<cConfig_AppPaths>(apc);
```

### Update Config at Runtime
Once we have a json file loaded as writeable, and registered with DI, we can recall it (using DI) and read/write to it.
Recalling config data from DI, comes in the form of IOptions instances.\
In the case of writeable config, we recall config data with IWritableOptions.

For example, the following is a simple class that uses DI to get an instance of writeable config, and read and write to it.
``` cs
    public class SampleService
    {
        // Keep a local copy of the writeable options instance...
        private readonly IWritableOptions<cConfig_AppPaths> _appSettings;


        // Have the constructor pull our writeable config from DI...
        public SampleService(IWritableOptions<cConfig_AppPaths> appSettings)
        {
            // And, save it locally...
            this._appSettings = appSettings;
        }

        // Sample usage method...
        public void SampleUsage()
        {
            // To read from the config, we will reference the IOptions value, to reach the config properties....
            string val = this._appSettings.Value.RepoType;

            // To update the config, we need to compose a lambda and pass it to the writeable options instance...
            this._appSettings.Update(opt =>
            {
                opt.RepoType = "newval";
            });
        }
    }
```
The above sample class gets the writeable appsettings config from DI (at construction).
And, it exposes a method call that reads and writes to the config instance at runtime.

### NET5 Support for Base64-Encoded Byte Arrays in Configuration.
NET5 has no native support for base64-encoded for storing byte arrays in configuration files (such as, appsettings.json).
See: https://github.com/dotnet/runtime/issues/36034<br>
This library includes a type converter to add support for that, based on the example listed here: https://github.com/dotnet/runtime/issues/37384#:~:text=ericstj%20commented%20on%20Jul%207%2C%202020

To enable NET5 support for base-64 encoded byte arrays, run this static call, in your program.cs, somewhere before the Configuration Builder executes:
``` cs
        OGA.AppSettings.Writeable.NET5_Workarounds.NET5_Base64ByteArrayStorageSupport.Add_Base64StorageSupport_forByteArray_in_NET5();
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

