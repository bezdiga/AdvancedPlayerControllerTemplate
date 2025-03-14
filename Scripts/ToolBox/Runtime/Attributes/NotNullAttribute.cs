using UnityEngine;

namespace HatchStudios.ToolBox
{
    public class NotNullAttribute : PropertyAttribute
    {
        public NotNullAttribute() : this("Variable has to be assigned.")
        {
        }

        public NotNullAttribute(string label)
        {
            Label = label;
        }

        public string Label { get; private set; }
    }
}
