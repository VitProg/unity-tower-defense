using System;

namespace td.common
{
    [AttributeUsage (AttributeTargets.Struct)]
    public class GenerateProviderAttribute : Attribute
    {
        private string _path;
    
        public GenerateProviderAttribute()
        {
        }
    
        public GenerateProviderAttribute(string path)
        {
            _path = path;
        }
    }
}