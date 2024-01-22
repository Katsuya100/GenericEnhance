using System;

namespace Katuusagi.GenericEnhance
{
    [AttributeUsage(AttributeTargets.Method)]
    public class VariadicGeneric : Attribute
    {
        public byte TypeParameterMin { get; private set; }
        public byte TypeParameterMax { get; private set; }

        public VariadicGeneric(byte typeParameterMax)
        {
            TypeParameterMax = typeParameterMax;
        }

        public VariadicGeneric(byte typeParameterMin, byte typeParameterMax)
        {
            TypeParameterMin = typeParameterMin;
            TypeParameterMax = typeParameterMax;
        }
    }
}
