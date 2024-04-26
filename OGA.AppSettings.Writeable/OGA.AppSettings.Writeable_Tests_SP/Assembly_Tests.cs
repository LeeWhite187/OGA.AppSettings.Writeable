using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace OGA.AppSettings.Writeable_Tests
{
    /// <summary>
    /// Added an assembly-wide setup method to ensure that our workaround for base64 storage support for byte arrays is added to configuration builder type handling.
    /// This class also calls the test assembly base methods, to setup logging and such.
    /// </summary>
    [TestClass]
    public class Assembly_Tests : OGA.Testing.Lib.TestAssembly_Base
    {
        #region Test Assembly Setup / Teardown

        /// <summary>
        /// This initializer calls the base assembly initializer.
        /// </summary>
        /// <param name="context"></param>
        [AssemblyInitialize]
        static public void TestAssembly_Initialize(TestContext context)
        {
            // This call will register type conversion support for byte arrays stored as base64 strings in configuration files (like appsettings.json).
            OGA.AppSettings.Writeable.NET5_Workarounds.NET5_Base64ByteArrayStorageSupport.Add_Base64StorageSupport_forByteArray_in_NET5();

            TestAssemblyBase_Initialize(context);
        }

        /// <summary>
        /// This cleanup method calls the base assembly cleanup.
        /// </summary>
        [AssemblyCleanup]
        static public void TestAssembly_Cleanup()
        {
            TestAssemblyBase_Cleanup();
        }

        #endregion
    }
}
