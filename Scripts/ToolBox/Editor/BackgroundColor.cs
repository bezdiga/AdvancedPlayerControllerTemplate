namespace HatchStudios.ToolBox
{
    using System;
    using UnityEngine;
    
    internal class BackgroundColor : IDisposable
    {
        private Color newBackgroundColor;
        private Color oldBackgroundColor;

        public BackgroundColor(Color newBackgroundColor)
        {
            this.newBackgroundColor = newBackgroundColor;
            Prepare();
        }

        public void Prepare()
        {
            oldBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = newBackgroundColor;
        }

        public void Dispose()
        {
            GUI.backgroundColor = oldBackgroundColor;
        }
    }
}