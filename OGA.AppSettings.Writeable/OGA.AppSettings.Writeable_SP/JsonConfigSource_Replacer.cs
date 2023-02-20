using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace OGA.AppSettings.Writeable
{
    static public class JsonConfigSource_Replacer
    {
        /// <summary>
        /// This method replaces any JSON configuration sources with a writeable JSON configuration source.
        /// This allows any JSON configuration files to be updated from the application.
        /// Call this method from inside a ConfigureAppConfiguration lambda.
        /// </summary>
        /// <param name="config"></param>
        static public void Replace_JSONConfigSources_with_Writeable_Sources(IConfigurationBuilder config)
        {
            // Iterate each configuration source, and replace any JSON sources with a writeable JSON source.
            for (int index = 0; index < config.Sources.Count; index++)
            {
                // Get a reference to the current config source...
                var s = config.Sources[index];

                // Get the source type name...
                string stn = s.GetType().Name;

                // See if it's a JSON source that we need to replace...
                if (stn == nameof(Microsoft.Extensions.Configuration.Json.JsonConfigurationSource))
                {
                    // This is a json config source.
                    // We need to replace it with a writeable json source of the same file.

                    // Create a writeable json provider, and copy config over to it from the readonly provider...
                    var ncs = new OGA.AppSettings.Writeable.JSONConfig.cWriteable_JSONConfigSource();
                    var cs = (Microsoft.Extensions.Configuration.Json.JsonConfigurationSource)s;
                    ncs.FileProvider = cs.FileProvider;
                    ncs.OnLoadException = cs.OnLoadException;
                    ncs.Optional = cs.Optional;
                    ncs.Path = cs.Path;
                    ncs.ReloadDelay = cs.ReloadDelay;
                    ncs.ReloadOnChange = cs.ReloadOnChange;
                    ncs.ResolveFileProvider();

                    // Swap in the writeable provider...
                    config.Sources[index] = ncs;
                }
            }
        }
    }
}
