using System;

namespace Runtime.BehaviourTree
{
    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        public readonly string description;
        public DescriptionAttribute(string description) 
        {
            this.description = description;
        }
    }
    
    [AttributeUsage(AttributeTargets.All)]
    public class NameAttribute : Attribute
    {
        public readonly string name;
        public NameAttribute(string name) 
        {
            this.name = name;
        }
    }
        
    [AttributeUsage(AttributeTargets.All)]
    public class CategoryAttribute : Attribute
    {
        public readonly string category;
        public CategoryAttribute(string category) 
        {
            this.category = category;
        }
    }
}