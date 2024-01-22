using System;

namespace Katuusagi.GenericEnhance
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SourceArgumentType : Attribute
    {
        public string ArgumentTypeName { get; private set; }

        public SourceArgumentType(Type argumentType)
        {
            ArgumentTypeName = argumentType.FullName;
        }

        public SourceArgumentType(string argumentTypeName)
        {
            ArgumentTypeName = argumentTypeName;
        }
    }
}
