using Microsoft.Extensions.Options;
using OGA.AppSettings.Writeable.JSONConfig;
using System;
using System.Collections.Generic;
using System.Text;

namespace OGA.AppSettings.Writeable_Tests.HelperClasses
{
    public class Service_Mock_A
    {
        private IWritableOptions<BuildDataConfig> _cfg;

        public Service_Mock_A(IWritableOptions<BuildDataConfig> config)
        {
            this._cfg = config;
        }

        public void UpdateConfigProperty(string propname, object propval)
        {
            if(propname == nameof(BuildDataConfig.RepoType))
                this._cfg.Update(config =>
                {
                    config.RepoType = propval.ToString();
                });
            else if(propname == nameof(BuildDataConfig.Source_Revision))
                this._cfg.Update(config =>
                {
                    config.Source_Revision = propval.ToString();
                });
            else if(propname == nameof(BuildDataConfig.Source_SolutionSubFolder))
                this._cfg.Update(config =>
                {
                    config.Source_SolutionSubFolder = propval.ToString();
                });
            else if(propname == nameof(BuildDataConfig.Source_URL))
                this._cfg.Update(config =>
                {
                    config.Source_URL = propval.ToString();
                });
            else if(propname == nameof(BuildDataConfig.stringVal))
                this._cfg.Update(config =>
                {
                    config.stringVal = propval.ToString();
                });
            else if(propname == nameof(BuildDataConfig.intVal))
                this._cfg.Update(config =>
                {
                    config.intVal = (int)propval;
                });
            else if(propname == nameof(BuildDataConfig.floatVal))
                this._cfg.Update(config =>
                {
                    config.floatVal = (float)propval;
                });
            else if(propname == nameof(BuildDataConfig.boolVal))
                this._cfg.Update(config =>
                {
                    config.boolVal = (bool)propval;
                });
            else if(propname == nameof(BuildDataConfig.guidVal))
                this._cfg.Update(config =>
                {
                    config.guidVal = (Guid)propval;
                });
            else if(propname == nameof(BuildDataConfig.datetimeVal))
                this._cfg.Update(config =>
                {
                    config.datetimeVal = (DateTime)propval;
                });
            else if(propname == nameof(BuildDataConfig.bytesVal))
                this._cfg.Update(config =>
                {
                    config.bytesVal = (byte[])propval;
                });
            else if(propname == nameof(BuildDataConfig.timespanVal))
                this._cfg.Update(config =>
                {
                    config.timespanVal = (TimeSpan)propval;
                });
            else if(propname == nameof(BuildDataConfig.uriVal))
                this._cfg.Update(config =>
                {
                    config.uriVal = (Uri)propval;
                });
        }

        public BuildDataConfig Get_Config()
        {
            return this._cfg.Value;
        }
    }
}
