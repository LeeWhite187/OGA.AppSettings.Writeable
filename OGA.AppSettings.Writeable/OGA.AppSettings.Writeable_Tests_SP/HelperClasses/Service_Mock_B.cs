using Microsoft.Extensions.Options;
using OGA.AppSettings.Writeable.JSONConfig;
using System;
using System.Collections.Generic;
using System.Text;

namespace OGA.AppSettings.Writeable_Tests.HelperClasses
{
    public class Service_Mock_B
    {
        private IWritableOptions<BuildDataConfig> _cfg;

        public Service_Mock_B(IWritableOptions<BuildDataConfig> config)
        {
            this._cfg = config;
        }

        public BuildDataConfig Get_Config()
        {
            return this._cfg.Value;
        }
    }
}
