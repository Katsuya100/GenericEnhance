using System;

namespace Katuusagi.GenericEnhance
{
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class DefaultType : Attribute
    {
        public Type Type { get; private set; }

        public DefaultType(Type type)
        {
            Type = type;
        }
    }
}
