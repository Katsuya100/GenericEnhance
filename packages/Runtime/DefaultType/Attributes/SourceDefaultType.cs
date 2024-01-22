using System;

namespace Katuusagi.GenericEnhance
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SourceDefaultType : Attribute
    {
        public Type DefaultType { get; private set; }

        public SourceDefaultType(Type defaultType)
        {
            DefaultType = defaultType;
        }
    }
}
