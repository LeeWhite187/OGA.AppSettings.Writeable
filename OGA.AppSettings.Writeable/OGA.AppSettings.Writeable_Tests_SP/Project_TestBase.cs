using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using NLog;
using OGA.SharedKernel.Config.structs;
using OGA.SharedKernel;

namespace OGA.AppSettings.Writeable_Tests.HelperClasses
{
    public class Project_TestBase
    {
        #region Private Fields

        protected Random rng = new Random();

        protected string _classname;

        protected string _testfolder;

        #endregion


        #region Setup

        public virtual void Setup()
        {
            SetupLogging_toConsole();

            // Create a test folder...
            this.Create_TestFolder();

            // Runs after each test. (Optional)

        }

        public virtual void TearDown()
        {
            // Runs after each test. (Optional)


            // Delete test folder...
            this.DeleteTestFolder();

        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Checks if the given timestamps are close to eachother, within the given tolerance.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="tolerance_seconds"></param>
        /// <returns></returns>
        protected bool IsTimestampWithinBounds(DateTime? t1, DateTime? t2, int tolerance_seconds)
        {
            if (t1 == null && t2 == null)
                return true;
            else if (t1 == null && t2 != null)
                return false;
            else if (t1 != null && t2 == null)
                return false;
            else
            {
                // Both dates are valid.

                if (t1 > t2)
                {
                    // T1 is bigger.

                    // See if it's bigger than t2 + tolerance...
                    var a = ((DateTime)t2).AddSeconds(tolerance_seconds);
                    if (t1 > a)
                        return false;
                    else
                        return true;
                }
                else if (t1 < t2)
                {
                    // T2 is bigger.

                    // See if it's bigger than t1 + tolerance...
                    var a = ((DateTime)t1).AddSeconds(tolerance_seconds);
                    if (t2 > a)
                        return false;
                    else
                        return true;
                }
                else
                {
                    // They are the same.
                    return true;
                }

            }
        }
        protected bool DateTimeIsClose(DateTime? dt, int tolerance)
        {
            if (dt == null)
                return false;

            DateTime ctime = DateTime.UtcNow;
            if (dt > ctime.AddSeconds(tolerance))
                return false;
            else if (dt < ctime.AddSeconds(-1 * tolerance))
                return false;
            else
                return true;
        }

        protected bool CreateRandomBool()
        {
            int val = rng.Next(1, 2);
            return ((val%2) == 1);
        }
        protected int CreateRandomInt()
        {
            int val  = rng.Next(1, 1000000);
            return val;
        }
        protected float CreateRandomFloat()
        {
            int magnitude = rng.Next(1, 10);
            int val  = rng.Next(1, 1000000);
            return (float)(val / (10^magnitude));
        }
        protected byte[] CreateRandomByteArray(int length)
        {

            var bytes = new Byte[length];
            rng.NextBytes(bytes);
            return bytes;
        }
        protected string CreateRandomString()
        {
            var val = Guid.NewGuid().ToString();
            return val;
        }
        protected Guid CreateRandomGuid()
        {
            var val = Guid.NewGuid();
            return val;
        }
        protected DateTime CreateRandomDateTime()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;           

            // Add a random time of day...
            start.AddSeconds(rng.Next(1, 86400));

            DateTime val = start.AddDays(rng.Next(range));
            return val;
        }
        protected TimeSpan CreateRandomTimespan()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;           
            DateTime end = start.AddDays(rng.Next(range));

            // Add a random time of day...
            end.AddSeconds(rng.Next(1, 86400));

            // Get the difference...
            TimeSpan val = end.Subtract(start);
            return val;
        }
        protected Uri CreateRandomUrl()
        {
            // Compose a random url, with random origin and path fragments...
            string url = "https://" + RandomString(10) + ".com/api/" + RandomString(10) + "/GetListing";

            Uri val = new Uri(url);

            return val;
        }
        protected string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rng.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Common method for registering config and services, in the same way that NET Core startup does.<br/>
        /// </summary>
        /// <remarks>
        /// This call will create and return a root-scoped service provider that includes any services and config defined in the given delegate.<br/>
        /// Pass in a delegate method containing additional services that your tests require.<br/>
        /// See this: <see href="https://oga.atlassian.net/wiki/spaces/~311198967/pages/116129793/Duplicating+.NET+Core+DI"></see>
        /// <para>
        /// <example>
        /// Here's an example registrationdelegate method that allows you to add services to the returned provider:
        /// <code>
        ///     protected void registrationdelegate(IServiceCollection services)
        ///     {
        ///         services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));
        ///         services.AddSingleton&lt;InterfaceB, ClassB&gt;();
        ///     }
        /// </code>
        /// </example>
        /// </para>
        /// </remarks>
        /// <returns></returns>
        protected IServiceProvider Setup_DIProvider(Action<IServiceCollection> registrationdelegate = null)
        {
            // Here are steps to create the same type of DI service provider that the .NET runtime generates:
            //  1. Create a service collection instance.
            //  2. Add services and config to the collection. This is the registration step.
            //  3. Tell the service collection to build a service provider that we can use across the app.


            //  1. Create a service collection instance.
            // Initiate the service collection...
            IServiceCollection services = new ServiceCollection();


            //  2. Add services and config to the collection. This is the registration step.
            {
                // Register any services and config to be available via DI...

                //services.Add(new ServiceDescriptor(typeof(InterfaceA), typeof(ClassA), ServiceLifetime.Singleton));
                //services.AddSingleton<InterfaceB, ClassB>();

                // Add any services and config the caller passed in via delegate...
                if (registrationdelegate != null)
                    registrationdelegate(services);
            }


            //  3. Tell the service collection to build a service provider that we can use across the app.
            // Create the ServiceProvider, so it can be used...
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            // Return the provider instance to the caller, so it can be used for tests...
            return serviceProvider;
        }

        static protected void Enable_AllLoggingLevels() 
        {
            foreach(var rule in LogManager.Configuration.LoggingRules)
            {
                rule.EnableLoggingForLevel(LogLevel.Trace);
                rule.EnableLoggingForLevel(LogLevel.Debug);
                rule.EnableLoggingForLevel(LogLevel.Warn);
                rule.EnableLoggingForLevel(LogLevel.Error);
                rule.EnableLoggingForLevel(LogLevel.Fatal);
            }

            LogManager.ReconfigExistingLoggers();
        }
        protected void SetupLogging_toConsole()
        {
            // Get the current logging configuration...
            var config = new NLog.Config.LoggingConfiguration();

            // Check if the console logger has been enabled...
            if(config.FindTargetByName("logconsole") == null)
            {
                // The console logger has not been added, yet.
                // We will add it, now.

                // Targets where to log to: Console
                var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

                // Enable standard log levels for the console logger...
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);

                // Apply config...
                NLog.LogManager.Configuration = config;

                // Reconfigure all loggers...
                LogManager.ReconfigExistingLoggers();
            }

            // Assign the logger to our shared logging refs...
            var logger = NLog.LogManager.GetCurrentClassLogger();
            OGA.SharedKernel.Logging_Base.Logger_Ref = logger;
        }
        static protected void Print_Enabled_LogLevels()
        {
            OGA.SharedKernel.Logging_Base.Logger_Ref?.Error("**** Start of Enabled Log Levels ****");

            if(!(OGA.SharedKernel.Logging_Base.Logger_Ref?.IsTraceEnabled ?? false))
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Error("Trace NOT Enabled");
            else
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Trace("Trace Enabled");

            if(!(OGA.SharedKernel.Logging_Base.Logger_Ref?.IsDebugEnabled ?? false))
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Error("Debug NOT Enabled");
            else
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Debug("Debug Enabled");

            if(!(OGA.SharedKernel.Logging_Base.Logger_Ref?.IsInfoEnabled ?? false))
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Error("Info NOT Enabled");
            else
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Info("Info Enabled");

            if(!(OGA.SharedKernel.Logging_Base.Logger_Ref?.IsWarnEnabled ?? false))
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Error("Warn NOT Enabled");
            else
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Warn("Warn Enabled");

            if(!(OGA.SharedKernel.Logging_Base.Logger_Ref?.IsErrorEnabled ?? false))
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Error("Error NOT Enabled");
            else
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Error("Error Enabled");

            if(!(OGA.SharedKernel.Logging_Base.Logger_Ref?.IsFatalEnabled ?? false))
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Error("Fatal NOT Enabled");
            else
                OGA.SharedKernel.Logging_Base.Logger_Ref?.Fatal("Fatal Enabled");

            OGA.SharedKernel.Logging_Base.Logger_Ref?.Error("**** End of Enabled Log Levels ****");
        }

        protected void Create_TestFolder()
        {
            try
            {
                // Get the user's temp path...
                var temppath = System.IO.Path.GetTempPath();

                // Create a test folder name...
                var tempfoldername = Guid.NewGuid().ToString();

                // Compose a path to it...
                this._testfolder = System.IO.Path.Combine(temppath, tempfoldername);

                // Create a test folder...
                var di = System.IO.Directory.CreateDirectory(this._testfolder);
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        protected string Create_FilePath_for_TestFile(string extension)
        {
            // Create a filename...
            var fn = Guid.NewGuid().ToString() + "." + extension;

            var fp = System.IO.Path.Combine(this._testfolder, fn);

            return fp;
        }

        private void DeleteTestFolder()
        {
            try
            {
                System.IO.Directory.Delete(this._testfolder, true);
            }
            catch(Exception)
            {

            }
        }

        #endregion
    }
}
