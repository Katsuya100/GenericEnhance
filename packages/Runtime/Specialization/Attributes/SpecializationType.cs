using System;

namespace Katuusagi.GenericEnhance
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class SpecializationType : Attribute
    {
        public Type DefaultType { get; private set; }

        public SpecializationType(Type type)
        {
            DefaultType = type;
        }
    }
}
