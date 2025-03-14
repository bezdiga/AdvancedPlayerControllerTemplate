using System;
using HatchStudio.Motion.Data;

namespace HatchStudio.Motion
{
    public interface IMotionDataHandler
    {
        DataType GetData<DataType>() where DataType : IMotionData;
        void RegisterBehaviour<T>(MotionDataChangedDelegate changedCallback);
        void UnregisterBehaviour<T>(MotionDataChangedDelegate changedCallback);
    }
    public delegate void MotionDataChangedDelegate(IMotionDataHandler dataHandler, bool forceUpdate);
}