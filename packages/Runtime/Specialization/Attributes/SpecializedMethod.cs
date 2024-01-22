using System;

namespace Katuusagi.GenericEnhance
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SpecializedMethod : Attribute
    {
        public string MethodName { get; private set; }
        public Type[] BindTypes { get; private set; }

        public SpecializedMethod(string methodName, params Type[] bindTypes)
        {
            MethodName = methodName;
            BindTypes = bindTypes;
        }
    }
}
