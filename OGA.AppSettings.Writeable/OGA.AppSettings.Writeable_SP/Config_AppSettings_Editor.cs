using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OGA.AppSettings.Writeable.JSONConfig;

namespace OGA.AppSettings.Writeable
{
    /// <summary>
    /// Meant to be used outside of normal net core process operation, as startup or setup of a process config.
    /// For runtime access to app settings, try and use an injected config instance.
    /// </summary>
    public class Config_AppSettings_Editor
    {
        /// <summary>
        /// Attempts to retrieve the Common configuration folder path from appsettings.json.
        /// </summary>
        /// <param name="tempstr"></param>
        /// <returns></returns>
        public static int Get_CommonConfigFolder_from_LocalAppConfig(ref string tempstr)
        {
            var ap = Get_AppPaths();
            if(ap == null)
            {
                return -1;
            }

            if (string.IsNullOrEmpty(ap.CommonConfigPath))
                return -1;
            if (ap.CommonConfigPath == "")
                return -1;

            tempstr = ap.CommonConfigPath;
            return 1;
        }
        static public int Set_CommonConfigFolder_in_LocalAppConfig(string defaultpath)
        {
            try
            {
                // Set the builder to look in the exe folder...
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);

                // Tell it to load the file with our app paths...
                builder.AddWriteableJsonFile(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_ConfigFile, optional: true, reloadOnChange: true);
                IConfigurationRoot config = builder.Build();

                var sect = config.GetSection(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_SectionName);

                // Map config data to a model...
                OGA.SharedKernel.Config.structs.Config_AppPaths_v2 ap = new OGA.SharedKernel.Config.structs.Config_AppPaths_v2();
                sect.Bind(ap);

                // Do the update...
                ap.CommonConfigPath = defaultpath;

                // Flatten the updated config struct to a dictionary...
                var fd = OGA.AppSettings.Writeable.JSONConfig.cJSONFlattener.Parse(ap);

                // Look for the correct provider to update...
                // In order to locate the provider furthest down the list, we will keep assigning matches to our section name.
                // And, the last match will be the one that gets precedence for access and updates.
                cWriteable_JSONConfigProvider wjp = null;
                foreach (var cp in config.Providers.Where(m => m.GetType().Name == nameof(cWriteable_JSONConfigProvider)))
                {
                    // See if the current candidate provider has the section we are updating...
                    cWriteable_JSONConfigProvider jp = (cWriteable_JSONConfigProvider)cp;
                    if (jp.Has_Section(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_SectionName))
                        wjp = jp;
                }
                // We know what provider to update.
                // We can tell it to hold off on changes, while we push them in.
                // Then, we will tell it to save changes at once.

                // Iteratively update the configuration with each value in the config object...
                wjp.WriteOnChange = false;
                foreach (var s in fd)
                {
                    config.GetSection(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_SectionName + ":" + s.Key).Value = s.Value;
                }
                wjp.WriteOnChange = true;
                if (wjp.Save() != 1)
                {
                    // Failed to save changes.
                    return -2;
                }
                // Config was saved.

                // Reload config from disk...
                config.Reload();
                
                return 1;
            }
            catch (Exception e)
            {
                return -3;
            }
        }

        /// <summary>
        /// Attempts to retrieve the configuration folder path from appsettings.json.
        /// </summary>
        /// <param name="tempstr"></param>
        /// <returns></returns>
        public static int Get_ConfigFolder_from_LocalAppConfig(ref string tempstr)
        {
            var ap = Get_AppPaths();
            if(ap == null)
            {
                return -1;
            }

            if (string.IsNullOrEmpty(ap.AppConfigPath))
                return -1;
            if (ap.AppConfigPath == "")
                return -1;

            tempstr = ap.AppConfigPath;
            return 1;
        }
        static public int Set_ConfigFolder_in_LocalAppConfig(string defaultpath)
        {
            try
            {
                // Set the builder to look in the exe folder...
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);

                // Tell it to load the file with our app paths...
                builder.AddWriteableJsonFile(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_ConfigFile, optional: true, reloadOnChange: true);
                IConfigurationRoot config = builder.Build();

                var sect = config.GetSection(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_SectionName);

                // Map config data to a model...
                OGA.SharedKernel.Config.structs.Config_AppPaths_v2 ap = new OGA.SharedKernel.Config.structs.Config_AppPaths_v2();
                sect.Bind(ap);

                // Do the update...
                ap.AppConfigPath = defaultpath;

                // Flatten the updated config struct to a dictionary...
                var fd = OGA.AppSettings.Writeable.JSONConfig.cJSONFlattener.Parse(ap);

                // Look for the correct provider to update...
                // In order to locate the provider furthest down the list, we will keep assigning matches to our section name.
                // And, the last match will be the one that gets precedence for access and updates.
                cWriteable_JSONConfigProvider wjp = null;
                foreach (var cp in config.Providers.Where(m => m.GetType().Name == nameof(cWriteable_JSONConfigProvider)))
                {
                    // See if the current candidate provider has the section we are updating...
                    cWriteable_JSONConfigProvider jp = (cWriteable_JSONConfigProvider)cp;
                    if (jp.Has_Section(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_SectionName))
                        wjp = jp;
                }
                // We know what provider to update.
                // We can tell it to hold off on changes, while we push them in.
                // Then, we will tell it to save changes at once.

                // Iteratively update the configuration with each value in the config object...
                wjp.WriteOnChange = false;
                foreach (var s in fd)
                {
                    config.GetSection(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_SectionName + ":" + s.Key).Value = s.Value;
                }
                wjp.WriteOnChange = true;
                if (wjp.Save() != 1)
                {
                    // Failed to save changes.
                    return -2;
                }
                // Config was saved.

                // Reload config from disk...
                config.Reload();
                
                return 1;
            }
            catch (Exception e)
            {
                return -3;
            }
        }

        /// <summary>
        /// Attempts to retrieve the logging folder path from appsettings.json.
        /// </summary>
        /// <param name="tempstr"></param>
        /// <returns></returns>
        public static int Get_LoggingFolder_from_LocalAppConfig(ref string tempstr)
        {
            var ap = Get_AppPaths();
            if (ap == null)
            {
                return -1;
            }

            if (string.IsNullOrEmpty(ap.LogPath))
                return -1;
            if (ap.LogPath == "")
                return -1;

            tempstr = ap.LogPath;
            return 1;
        }
        static public int Set_LoggingFolder_in_LocalAppConfig(string defaultpath)
        {
            try
            {
                // Set the builder to look in the exe folder...
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);

                // Tell it to load the file with our app paths...
                builder.AddJsonFile(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_ConfigFile, optional: true, reloadOnChange: true);
                IConfigurationRoot config = builder.Build();

                var sect = config.GetSection(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_SectionName);

                // Map config data to a model...
                OGA.SharedKernel.Config.structs.Config_AppPaths_v2 ap = new OGA.SharedKernel.Config.structs.Config_AppPaths_v2();
                sect.Bind(ap);

                // Do the update...
                ap.LogPath = defaultpath;

                // Save updates to appconfig...
                sect.Value = Newtonsoft.Json.JsonConvert.SerializeObject(ap);

                return 1;
            }
            catch (Exception e)
            {
                return -2;
            }
        }

        static private OGA.SharedKernel.Config.structs.Config_AppPaths_v2 Get_AppPaths()
        {
            try
            {
                // Set the builder to look in the exe folder...
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);

                // Tell it to load the file with our app paths...
                builder.AddJsonFile(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_ConfigFile, optional: true, reloadOnChange: true);
                IConfigurationRoot config = builder.Build();

                var sect = config.GetSection(OGA.SharedKernel.Config.structs.Config_AppPaths_v2.CONSTANT_SectionName);

                // Map config data to a model...
                OGA.SharedKernel.Config.structs.Config_AppPaths_v2 ap = new OGA.SharedKernel.Config.structs.Config_AppPaths_v2();
                sect.Bind(ap);

                return ap;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
