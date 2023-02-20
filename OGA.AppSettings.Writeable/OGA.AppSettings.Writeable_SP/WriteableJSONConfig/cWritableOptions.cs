using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OGA.AppSettings.Writeable.JSONConfig
{
    /// <summary>
    /// This class is used to retrieve writeable config instance data from DI.
    /// See this reference for usage: https://github.com/LeeWhite187/OGA.AppSettings.Writeable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        void Update(Action<T> applyChanges);
    }
    /// <summary>
    /// This class is used to retrieve writeable config instance data from DI.
    /// See this reference for usage: https://github.com/LeeWhite187/OGA.AppSettings.Writeable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class cWritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly string _sectionname;

        public cWritableOptions(
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            string sectionname)
        {
            _options = options;
            _configuration = configuration;
            _sectionname = sectionname;
        }

        public T Value => _options.CurrentValue;
        public T Get(string name) => _options.Get(name);

        public void Update(Action<T> applyChanges)
        {
            // Get the configuration section to update...
            var ffff = new T();
            _configuration.GetSection(_sectionname).Bind(ffff);

            // Accept the changes from the caller...
            applyChanges(ffff);

            // Flatten the updated config struct to a dictionary...
            var fd = OGA.AppSettings.Writeable.JSONConfig.cJSONFlattener.Parse(ffff);

            // Look for the correct provider to update...
            // In order to locate the provider furthest down the list, we will keep assigning matches to our section name.
            // And, the last match will be the one that gets precedence for access and updates.
            cWriteable_JSONConfigProvider wjp = null;
            foreach(var cp in _configuration.Providers.Where(m => m.GetType().Name == nameof(cWriteable_JSONConfigProvider)))
            {
                // See if the current candidate provider has the section we are updating...
                cWriteable_JSONConfigProvider jp = (cWriteable_JSONConfigProvider)cp;
                if (jp.Has_Section(this._sectionname))
                    wjp = jp;
            }
            // We know what provider to update.
            // We can tell it to hold off on changes, while we push them in.
            // Then, we will tell it to save changes at once.

            // Iteratively update the configuration with each value in the config object...
            wjp.WriteOnChange = false;
            foreach(var s in fd)
            {
                _configuration.GetSection(_sectionname + ":" + s.Key).Value = s.Value;
            }
            wjp.WriteOnChange = true;
            if(wjp.Save() != 1)
            {
                // Failed to save changes.
                throw new System.IO.IOException("Failed to save config file.");
            }
            // Config was saved.

            // Reload config from disk...
            _configuration.Reload();
        }
    }

    /// <summary>
    ///  Original method that writes directly from the update call.
    ///  This method doesn't require a separate write-able options class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class cWritableOptions_Old<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IOptionsMonitor<T> _options;
        private readonly IConfigurationRoot _configuration;
        private readonly string _sectionname;
        private readonly string _file;

        public cWritableOptions_Old(
            IWebHostEnvironment environment,
            IOptionsMonitor<T> options,
            IConfigurationRoot configuration,
            string sectionname,
            string file)
        {
            _environment = environment;
            _options = options;
            _configuration = configuration;
            _sectionname = sectionname;
            _file = file;
        }

        public T Value => _options.CurrentValue;
        public T Get(string name) => _options.Get(name);

        public void Update(Action<T> applyChanges)
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var fileInfo = fileProvider.GetFileInfo(_file);
            var physicalPath = fileInfo.PhysicalPath;

            // Pull in the entire config file...
            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));

            // Get the particular section of config as an object that can be updated...
            // If the section is not present, a new instance will be created.
            var sectionObject = jObject.TryGetValue(_sectionname, out JToken section) ?
                JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());

            // Let the caller make changes to the object...
            applyChanges(sectionObject);

            // Update the in-memory config with the section update...
            jObject[_sectionname] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));

            // Write the entire config back to the file...
            File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));

            // Reload config so the data dictionary will populate with our changes...
            _configuration.Reload();
        }
    }
}