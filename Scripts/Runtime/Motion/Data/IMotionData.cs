using UnityEngine;

namespace HatchStudio.Motion.Data
{
    public interface IMotionData
    {
        
    }

    public abstract class MotionData : ScriptableObject, IMotionData
    {
        protected const string MOTION_DATA_MENU_PATH = "Hatch Studio/Motion/Data/";
    }
}