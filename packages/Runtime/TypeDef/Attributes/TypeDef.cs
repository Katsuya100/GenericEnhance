using System;

namespace Katuusagi.GenericEnhance
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public class TypeDef : Attribute
    {
        public Type Type { get; private set; }

        public string TypeName { get; private set; }

        public TypeDef(Type type)
        {
            Type = type;
            TypeName = type.FullName;
        }

        public TypeDef(string name)
        {
            Type = null;
            TypeName = name;
        }
    }
}
