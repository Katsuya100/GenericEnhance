using System;

namespace Katuusagi.GenericEnhance
{
    [AttributeUsage(AttributeTargets.Method)]
    public class VariadicGenerated : Attribute
    {
        public byte ParameterCount { get; private set; }
        public VariadicGenerated(byte parameterCount)
        {
            ParameterCount = parameterCount;
        }
    }
}
