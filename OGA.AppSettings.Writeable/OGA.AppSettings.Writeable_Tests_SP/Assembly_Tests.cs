using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace OGA.AppSettings.Writeable_Tests
{
    /// <summary>
    /// Added an assembly-wide setup method to ensure that our workaround for base64 storage support for byte arrays is added to configuration builder type handling.
    /// </summary>
    [TestClass]
    public class Assembly_Tests
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            // This call will register type conversion support for byte arrays stored as base64 strings in configuration files (like appsettings.json).
            OGA.AppSettings.Writeable.NET5_Workarounds.NET5_Base64ByteArrayStorageSupport.Add_Base64StorageSupport_forByteArray_in_NET5();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
        }
    }
}
