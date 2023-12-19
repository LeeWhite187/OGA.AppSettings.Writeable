using System;
using System.Collections.Generic;
using System.Text;

namespace OGA.AppSettings.Writeable_Tests
{
    public class ClasswithPropertyTypes
    {
        public string stringval { get; set; }

        public int intval { get; set; }
        public float floatval { get; set; }
        public bool boolval { get; set; }
        public byte[] bytesval { get; set; }
    }


    public class GenericHelper<T>
    {
        /// <summary>
        /// Added a string property, as a control, to verify parsing works for it, within a generic.
        /// </summary>
        public string controlval { get; set; }

        public T data { get; set; }

        public GenericHelper(T val)
        {
            this.data = val;
        }
    }
}
