using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace OGA.AppSettings.Writeable_Tests.HelperClasses
{
    /// <summary>
    /// Base class of a typical appsettings.json configuration file.
    /// Most properties in this class are spectators, but are included to simulate a real-world config being updated.
    /// </summary>
    public class AppSettingsConfigRoot
    {
        public string AllowedHosts {  get; set; }

        public BuildDataConfig BuildData {  get; set; }

        public LoggingConfig Logging {  get; set; }

        public PathsConfig Paths {  get; set; }

        public string Avatar_Store {  get; set; }

        public SQLServerConfig SQLServerConfig { get; set; }


        public AppSettingsConfigRoot()
        {
            this.AllowedHosts = "";

            this.BuildData = new BuildDataConfig();
            this.Logging = new LoggingConfig();
            this.Paths = new PathsConfig();

            this.Avatar_Store = "";

            this.SQLServerConfig = new SQLServerConfig();
        }

        public static AppSettingsConfigRoot DeepCopy(AppSettingsConfigRoot cfg)
        {
            var nv = new AppSettingsConfigRoot();
            nv.AllowedHosts = cfg.AllowedHosts;
            nv.BuildData = BuildDataConfig.DeepCopy(cfg.BuildData);
            nv.Logging = LoggingConfig.DeepCopy(cfg.Logging);
            nv.Paths = PathsConfig.DeepCopy(cfg.Paths);
            nv.Avatar_Store = cfg.Avatar_Store;
            nv.SQLServerConfig = SQLServerConfig.DeepCopy(cfg.SQLServerConfig);

            return nv;
        }
    }

    /// <summary>
    /// This class includes some typical properties of a build.
    /// As well, it includes properties, of each handled type, that will be updated and verified.
    /// </summary>
    public class BuildDataConfig
    {
        public string RepoType {  get; set; }

        public string Source_Revision {  get; set; }

        public string Source_SolutionSubFolder {  get; set; }

        public string Source_URL {  get; set; }


        public string stringVal {  get; set; }

        public int intVal {  get; set; }

        public float floatVal {  get; set; }

        public bool boolVal {  get; set; }

        public Guid guidVal {  get; set; }

        public DateTime datetimeVal {  get; set; }

        public byte[] bytesVal {  get; set; }

        public TimeSpan timespanVal {  get; set; }

        public Uri uriVal {  get; set; }


        public BuildDataConfig()
        {
            this.RepoType = "";
            this.Source_Revision = "";
            this.Source_SolutionSubFolder = "";
            this.Source_URL = "";

            this.stringVal = "";
            this.bytesVal = new byte[0];
        }

        static public BuildDataConfig DeepCopy(BuildDataConfig cfg)
        {
            var nv = new BuildDataConfig();
            nv.RepoType = cfg.RepoType;
            nv.Source_Revision = cfg.Source_Revision;
            nv.Source_SolutionSubFolder = cfg.Source_SolutionSubFolder;
            nv.Source_URL = cfg.Source_URL;

            nv.stringVal = cfg.stringVal;
            nv.intVal = cfg.intVal;
            nv.floatVal = cfg.floatVal;
            nv.boolVal = cfg.boolVal;
            nv.guidVal = cfg.guidVal;
            nv.datetimeVal = cfg.datetimeVal;
            nv.bytesVal = cfg.bytesVal;
            nv.timespanVal = cfg.timespanVal;
            nv.uriVal = cfg.uriVal;

            return nv;
        }
    }

    public class LoggingConfig
    {
        public LogLevelConfig LogLevel {  get; set; }


        public LoggingConfig()
        {
            this.LogLevel = new LogLevelConfig();
        }

        static public LoggingConfig DeepCopy(LoggingConfig cfg)
        {
            var nv = new LoggingConfig();
            nv.LogLevel = LogLevelConfig.DeepCopy(cfg.LogLevel);

            return nv;
        }
    }

    public class LogLevelConfig
    {
        public string Default {  get; set; }

        public string Microsoft {  get; set; }

        [JsonProperty(PropertyName = "Microsoft.AspNetCore.Http.Connections")]
        public string Microsoft_AspNetCore_Http_Connections {  get; set; }

        [JsonProperty(PropertyName = "Microsoft.AspNetCore.SignalR")]
        public string Microsoft_AspNetCore_SignalR {  get; set; }

        public string System {  get; set; }


        public LogLevelConfig()
        {
            this.Default = "";
            this.Microsoft = "";
            this.Microsoft_AspNetCore_Http_Connections = "";
            this.Microsoft_AspNetCore_SignalR = "";
            this.System = "";
        }

        static public LogLevelConfig DeepCopy(LogLevelConfig cfg)
        {
            var nv = new LogLevelConfig();
            nv.Default = cfg.Default;
            nv.Microsoft = cfg.Microsoft;
            nv.Microsoft_AspNetCore_Http_Connections = cfg.Microsoft_AspNetCore_Http_Connections;
            nv.Microsoft_AspNetCore_SignalR = cfg.Microsoft_AspNetCore_SignalR;
            nv.System = cfg.System;

            return nv;
        }
    }

    public class PathsConfig
    {
        public string CommonConfigPath {  get; set; }

        public string AppConfigPath {  get; set; }

        public string LogPath {  get; set; }


        public PathsConfig()
        {
            this.CommonConfigPath = "";
            this.AppConfigPath = "";
            this.LogPath = "";
        }

        static public PathsConfig DeepCopy(PathsConfig cfg)
        {
            var nv = new PathsConfig();
            nv.CommonConfigPath = cfg.CommonConfigPath;
            nv.AppConfigPath = cfg.AppConfigPath;
            nv.LogPath = cfg.LogPath;

            return nv;
        }
    }

    public class SQLServerConfig
    {
        public string ConnectionString {  get; set; }


        public SQLServerConfig()
        {
            this.ConnectionString = "";
        }

        static public SQLServerConfig DeepCopy(SQLServerConfig cfg)
        {
            var nv = new SQLServerConfig();
            nv.ConnectionString = cfg.ConnectionString;

            return nv;
        }
    }
}
