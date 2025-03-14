using System;

namespace HatchStudios.ToolBox.Utility
{
    public class TypeConstrainAttribute
    {
        
    }
    
    [Flags]
    public enum TypeSettings
    {
        Class = 1,
        Interface = 2
    }
}