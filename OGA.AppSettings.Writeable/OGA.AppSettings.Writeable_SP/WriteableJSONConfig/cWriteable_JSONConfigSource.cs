using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace OGA.AppSettings.Writeable.JSONConfig
{
    public class cWriteable_JSONConfigSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new cWriteable_JSONConfigProvider(this);
        }
    }
}
