using UnityEngine;

namespace HatchStudio.Motion.Data
{
    public abstract class DataMotionBehaviour<DataType> : MotionBehaviour where DataType : IMotionData
    {
        protected DataType Data;
        
        protected virtual void OnDataChanged(DataType data) { }
        
        protected virtual DataType GetDataFromPreset(IMotionDataHandler dataHandler)
        {
            return dataHandler.GetData<DataType>();
        }
        
        protected sealed override void OnEnable()
        {
            base.OnEnable();
            GetComponent<IMotionDataHandler>().RegisterBehaviour<DataType>(DataChanged);
        }

        protected sealed override void OnDisable()
        {
            base.OnDisable();
            GetComponent<IMotionDataHandler>().UnregisterBehaviour<DataType>(DataChanged);
        }
        
        private void DataChanged(IMotionDataHandler dataHandler, bool forceUpdate)
        {
            var prevData = Data;
            Data = GetDataFromPreset(dataHandler);

            if (Data == null)
            {
                SetTargetPosition(Vector3.zero);
                SetTargetRotation(Vector3.zero);
            }

            if (forceUpdate || !CompareData(prevData, Data))
                OnDataChanged(Data);
        }

        private static bool CompareData(IMotionData data1, IMotionData data2)
        {
            if (data1 is Object data1Obj)
            {
                if (data2 is Object data2Obj)
                    return data1Obj == data2Obj;

                return false;
            }

            return data1 == data2;
        }
    }
}