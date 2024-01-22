using System;

namespace Katuusagi.GenericEnhance
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SpecializationMethod : Attribute
    {
        public string DefaultMethodName { get; private set; }
        public SpecializeAlgorithm Algorithm { get; private set; }

        public SpecializationMethod(string defaultMethodName, SpecializeAlgorithm algorithm = SpecializeAlgorithm.VirtualStrategy)
        {
            DefaultMethodName = defaultMethodName;
            Algorithm = algorithm;
        }
    }
}
