using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OGA.AppSettings.Writeable.JSONConfig;
using OGA.AppSettings.Writeable_Tests.HelperClasses;

namespace OGA.AppSettings.Writeable_Tests
{
    /*  Integration tests for classes in OGA.AppSettings.Writeable.
     *  The testing methodology here, recreates the net core runtime environment of IConfigurationRoot and IServiceProvider instances,
     *  with registered config and services that mutate and realize state.
     *  Including this level of ceremony in each test, ensures representative config update behavior in live process.

        //  Test_1_1_1  Perform a simulation of app startup: standing up configurationroot and serviceprovider.
        //              Include registration of a writeable json config file, that will get updated.
        //              Include registration of two different service classes, that will consume our test config: one to update config; the other to show changes.
        //              Perform a config udpate through a DI-registered service instance.
        //              Verify config is updated across the app through a second DI-registered service of different type.
        //              Verify the offline config file is also updated.

        //  Test_1_1_2  This test is a variation of Test_1_1_1, specifically testing that a string config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.

        //  Test_1_1_3  This test is a variation of Test_1_1_1, specifically testing that an integer config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
     
        //  Test_1_1_4  This test is a variation of Test_1_1_1, specifically testing that a float config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.

        //  Test_1_1_5  This test is a variation of Test_1_1_1, specifically testing that a boolean config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.

        //  Test_1_1_6  This test is a variation of Test_1_1_1, specifically testing that a Guid config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.

        //  Test_1_1_7  This test is a variation of Test_1_1_1, specifically testing that a DateTime config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.

        //  Test_1_1_8  This test is a variation of Test_1_1_1, specifically testing that a byte array config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.

        //  Test_1_1_9  This test is a variation of Test_1_1_1, specifically testing that a TimeSpan config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.

        //  Test_1_1_10 This test is a variation of Test_1_1_1, specifically testing that a Uri config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
    */


    [TestClass]
    public class AppSettingsWriteable_IntegrationTests : Project_TestBase
    {
        #region Private Fields

        #endregion


        #region ctor / dtor

        public AppSettingsWriteable_IntegrationTests()
        {
            _classname = nameof(AppSettingsWriteable_IntegrationTests);
        }

        #endregion


        #region Setup

        [TestInitialize]
        override public void Setup()
        {
            base.Setup();

            OGA.SharedKernel.Logging_Base.Logger_Ref?.Info(
                $"{_classname}:::{nameof(Setup)} - " +
                $"Base Setup done.");


            // Runs before each test. (Optional)
        }

        [TestCleanup]
        override public void TearDown()
        {
            // Runs after each test. (Optional)

            base.TearDown();
        }

        #endregion


        #region Test Methods

        //  Test_1_1_1  Perform a simulation of app startup: standing up configurationroot and serviceprovider.
        //              Include registration of a writeable json config file, that will get updated.
        //              Include registration of two different service classes, that will consume our test config: one to update config; the other to show changes.
        //              Perform a config udpate through a DI-registered service instance.
        //              Verify config is updated across the app through a second DI-registered service of different type.
        //              Verify the offline config file is also updated.
        [TestMethod]
        public async Task Test_1_1_1()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();

                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that a string config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.stringVal = CreateRandomString();


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            string updatedvalue = Guid.NewGuid().ToString();
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.Source_URL = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.Source_URL), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        //  Test_1_1_2  This test is a variation of Test_1_1_1, specifically testing that a string config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
        [TestMethod]
        public async Task Test_1_1_2()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();
                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that a string config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.stringVal = CreateRandomString();


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            string updatedvalue = Guid.NewGuid().ToString();
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.Source_URL = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.Source_URL), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        //  Test_1_1_3  This test is a variation of Test_1_1_1, specifically testing that an integer config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
        [TestMethod]
        public async Task Test_1_1_3()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();
                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that an integer config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.intVal = CreateRandomInt();


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            int updatedvalue = CreateRandomInt();
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.intVal = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.intVal), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        //  Test_1_1_4  This test is a variation of Test_1_1_1, specifically testing that a float config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
        [TestMethod]
        public async Task Test_1_1_4()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();
                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that a float config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            float updatedvalue = CreateRandomFloat();
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.floatVal = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.floatVal), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        //  Test_1_1_5  This test is a variation of Test_1_1_1, specifically testing that a boolean config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
        [TestMethod]
        public async Task Test_1_1_5()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();
                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that a boolean config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.boolVal = CreateRandomBool();


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            // Since only two states exist for a bool, we will flip it, to ensure an update occurs.
            bool updatedvalue = !originalsettingsconfig.BuildData.boolVal;
            //bool updatedvalue = CreateRandomBool();
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.boolVal = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.boolVal), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        //  Test_1_1_6  This test is a variation of Test_1_1_1, specifically testing that a Guid config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
        [TestMethod]
        public async Task Test_1_1_6()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();
                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that a Guid config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            Guid updatedvalue = CreateRandomGuid();
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.guidVal = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.guidVal), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        //  Test_1_1_7  This test is a variation of Test_1_1_1, specifically testing that a DateTime config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
        [TestMethod]
        public async Task Test_1_1_7()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();
                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that a DateTime config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            DateTime updatedvalue = CreateRandomDateTime();
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.datetimeVal = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.datetimeVal), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        //  Test_1_1_8  This test is a variation of Test_1_1_1, specifically testing that a byte array config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
        [TestMethod]
        public async Task Test_1_1_8()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();
                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that a byte array config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            byte[] updatedvalue = CreateRandomByteArray(100);
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.bytesVal = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.bytesVal), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        //  Test_1_1_9  This test is a variation of Test_1_1_1, specifically testing that a TimeSpan config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
        [TestMethod]
        public async Task Test_1_1_9()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();
                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that a TimeSpan config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            TimeSpan updatedvalue = CreateRandomTimespan();
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.timespanVal = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.timespanVal), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        //  Test_1_1_10 This test is a variation of Test_1_1_1, specifically testing that a Uri config property can be runtime-updated app-wide and updated in offline config.
        //              We have chosen to manipulate properties of the BuildData config block, while leaving the other sections as unchanged, but verified spectators.
        //              Ceremony and methodology is same as Test_1_1_1.
        [TestMethod]
        public async Task Test_1_1_10()
        {
            // For diagnostics, get the working directory, here...
            var workingdirectory = Environment.CurrentDirectory;


            // Create a json configuration file, like a net core process uses, and populate it with some configuration we will update...
            AppSettingsConfigRoot originalsettingsconfig;
            {
                originalsettingsconfig = new AppSettingsConfigRoot();
                originalsettingsconfig.Paths.LogPath = CreateRandomString();
                originalsettingsconfig.Paths.CommonConfigPath = CreateRandomString();
                originalsettingsconfig.Paths.AppConfigPath = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Default = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.Microsoft_AspNetCore_SignalR = CreateRandomString();
                originalsettingsconfig.Logging.LogLevel.System = CreateRandomString();
                originalsettingsconfig.Avatar_Store = CreateRandomString();
                originalsettingsconfig.AllowedHosts = CreateRandomString();
                originalsettingsconfig.SQLServerConfig.ConnectionString = CreateRandomString();
                originalsettingsconfig.BuildData.RepoType = CreateRandomString();
                originalsettingsconfig.BuildData.Source_Revision = CreateRandomString();
                originalsettingsconfig.BuildData.Source_SolutionSubFolder = CreateRandomString();
                originalsettingsconfig.BuildData.Source_URL = CreateRandomString();

                // Baseline values of different types, that will be individually changed in separate tests...
                originalsettingsconfig.BuildData.stringVal = CreateRandomString();
                originalsettingsconfig.BuildData.intVal = CreateRandomInt();
                originalsettingsconfig.BuildData.floatVal = CreateRandomFloat();
                originalsettingsconfig.BuildData.boolVal = CreateRandomBool();
                originalsettingsconfig.BuildData.guidVal = CreateRandomGuid();
                originalsettingsconfig.BuildData.datetimeVal = CreateRandomDateTime();
                originalsettingsconfig.BuildData.bytesVal = CreateRandomByteArray(100);
                originalsettingsconfig.BuildData.timespanVal = CreateRandomTimespan();
                originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();
            }


            // We are testing that a Uri config value can be updated across the app and in stored appsettings.json.
            // Assign the original value under test...
            originalsettingsconfig.BuildData.uriVal = CreateRandomUrl();


            // Make a copy of the settings data, that we can update to track changes we make and verify they are mirrored by the settings classes under test....
            var trackingconfig = AppSettingsConfigRoot.DeepCopy(originalsettingsconfig);


            // Determine a filepath for the config file...
            var cfg_filename = Guid.NewGuid().ToString() + ".json";
            var cfg_filepath = System.IO.Path.Combine(workingdirectory, cfg_filename);


            // Store the config in an accessible file, like it would exist during startup...
            {
                try
                {
                    // Serialize config to the file...
                    var jsoncfg = Newtonsoft.Json.JsonConvert.SerializeObject(originalsettingsconfig, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(cfg_filepath, jsoncfg);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }


            // Have a config builder create the configuration instance just like a net core process would have...
            // Give it the path to our settings file, from above.
            IConfigurationRoot config;
            {
                // Start a config builder that will load our test config file...
                config = new ConfigurationBuilder()
                // Have it load the settings file as writeable...
                .AddWriteableJsonFile(cfg_filepath)

                // Build the config provider, so we can use the collected config data...
                .Build();
            }


            // Retrieve a section from the config instance...
            IConfigurationSection cfgsection;
            {
                var sectionname = nameof(AppSettingsConfigRoot.BuildData);

                // Retrieve the configuration section we need...
                cfgsection = config.GetSection(sectionname);

                // As a sanity check, hydrate the config to ensure we got it...
                var bd = cfgsection.Get<BuildDataConfig>();
                if(bd == null)
                    Assert.Fail("Wrong Value");

                // Do a compare to check that it was loaded correctly...
                this.Compare_ConfigInstances(originalsettingsconfig.BuildData, bd);
            }


            // Create a service provider that registers the config for use...
            IServiceProvider svcprov;
            {
                // Create the callback that will register our mock service for use...
                Action<IServiceCollection> didelegate = (services) =>
                {
                    // Firstly, register the root configuration instance with services, like the net core DI runtime does...
                    services.AddScoped<IConfiguration>(_=> config);
                    //services.AddScoped<IConfiguration, IConfigurationRoot>(_=> config);


                    // Register the IOptions instance of our config...
                    services.ConfigureWritable<BuildDataConfig>(cfgsection);


                    // Register our mock service...
                    // NOTE: We will let DI figure out constructor parms.
                    services.AddSingleton<Service_Mock_A>();
                    //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));

                    // Register another service that also uses the same config...
                    services.AddSingleton<Service_Mock_B>();
                };

                // Standup the root-scoped service provider...
                svcprov = Setup_DIProvider(didelegate);
            }


            // Get a reference to our mock service...
            // This will let us simulate changes to config at runtime.
            var mockservicea = svcprov.GetService<Service_Mock_A>();


            // Determine a new value...
            Uri updatedvalue = CreateRandomUrl();
            // Update our tracking copy for simple comparison...
            trackingconfig.BuildData.uriVal = updatedvalue;


            // Make a change to config via the service...
            mockservicea.UpdateConfigProperty(nameof(BuildDataConfig.uriVal), updatedvalue);


            // Check that the service has an updated to work with...
            var servicecfgstate = mockservicea.Get_Config();
            this.Compare_ConfigInstances(trackingconfig.BuildData, servicecfgstate);


            // Verify the config value is updated across the process...
            // Easiest way to do this is to get an instance of another registered object that uses the same config.
            var mockserviceb = svcprov.GetService<Service_Mock_B>();
            var retrievedconfig = mockserviceb.Get_Config();

            // Compare what the second service got to what we expect...
            this.Compare_ConfigInstances(trackingconfig.BuildData, retrievedconfig);


            // Verify the offline config file is updated...
            {
                try
                {
                    var retrievedjson = System.IO.File.ReadAllText(cfg_filepath);

                    var retreivedconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettingsConfigRoot>(retrievedjson);
                    if(retreivedconfig == null)
                        Assert.Fail("Wrong Value");

                    // Compare config instances...
                    this.Compare_ConfigInstances(trackingconfig, retreivedconfig);
                }
                catch(Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
        }

        #endregion


        #region Private Methods

        private void Compare_ConfigInstances(BuildDataConfig c1, BuildDataConfig c2)
        {
            if (c1 == null || c2 == null)
                Assert.Fail("Wrong value");

            if (c1.RepoType != c2.RepoType)
                Assert.Fail("Wrong value");
            if (c1.Source_Revision != c2.Source_Revision)
                Assert.Fail("Wrong value");
            if (c1.Source_SolutionSubFolder != c2.Source_SolutionSubFolder)
                Assert.Fail("Wrong value");
            if (c1.Source_URL != c2.Source_URL)
                Assert.Fail("Wrong value");

            if(c1.stringVal != c2.stringVal)
                Assert.Fail("Wrong value");
            if(c1.intVal != c2.intVal)
                Assert.Fail("Wrong value");
            if(c1.floatVal != c2.floatVal)
                Assert.Fail("Wrong value");
            if(c1.boolVal != c2.boolVal)
                Assert.Fail("Wrong value");
            if(c1.guidVal != c2.guidVal)
                Assert.Fail("Wrong value");
            if(c1.datetimeVal != c2.datetimeVal)
                Assert.Fail("Wrong value");
            if(c1.timespanVal != c2.timespanVal)
                Assert.Fail("Wrong value");
            if(c1.uriVal != c2.uriVal)
                Assert.Fail("Wrong value");

            if(c1.bytesVal.Length != c2.bytesVal.Length)
                Assert.Fail("Wrong value");
            var c1b64 = Convert.ToBase64String(c1.bytesVal);
            var c2b64 = Convert.ToBase64String(c2.bytesVal);
            if(c1b64 != c2b64)
                Assert.Fail("Wrong value");
        }

        private void Compare_ConfigInstances(AppSettingsConfigRoot c1, AppSettingsConfigRoot c2)
        {
            if (c1 == null || c2 == null)
                Assert.Fail("Wrong value");


            if (c1.AllowedHosts == null || c2.AllowedHosts == null)
                Assert.Fail("Wrong value");

            this.Compare_ConfigInstances(c1.BuildData, c2.BuildData);


            if (c1.Logging == null || c2.Logging == null)
                Assert.Fail("Wrong value");
            if (c1.Logging.LogLevel == null || c2.Logging.LogLevel == null)
                Assert.Fail("Wrong value");
            if (c1.Logging.LogLevel.Default != c2.Logging.LogLevel.Default)
                Assert.Fail("Wrong value");
            if (c1.Logging.LogLevel.Microsoft != c2.Logging.LogLevel.Microsoft)
                Assert.Fail("Wrong value");
            if (c1.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections != c2.Logging.LogLevel.Microsoft_AspNetCore_Http_Connections)
                Assert.Fail("Wrong value");
            if (c1.Logging.LogLevel.Microsoft_AspNetCore_SignalR != c2.Logging.LogLevel.Microsoft_AspNetCore_SignalR)
                Assert.Fail("Wrong value");
            if (c1.Logging.LogLevel.System != c2.Logging.LogLevel.System)
                Assert.Fail("Wrong value");


            if (c1.Paths == null || c2.Paths == null)
                Assert.Fail("Wrong value");
            if (c1.Paths.CommonConfigPath != c2.Paths.CommonConfigPath)
                Assert.Fail("Wrong value");
            if (c1.Paths.AppConfigPath != c2.Paths.AppConfigPath)
                Assert.Fail("Wrong value");
            if (c1.Paths.LogPath != c2.Paths.LogPath)
                Assert.Fail("Wrong value");


            if (c1.Avatar_Store != c2.Avatar_Store)
                Assert.Fail("Wrong value");


            if (c1.SQLServerConfig.ConnectionString != c2.SQLServerConfig.ConnectionString)
                Assert.Fail("Wrong value");

            // If here, no differences found.
        }

        #endregion
    }
}
