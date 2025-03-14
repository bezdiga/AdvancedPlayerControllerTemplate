using HatchStudio.Utilities;
using UnityEngine;

namespace HatchStudio.Manager
{
    [CreateAssetMenu(menuName = OPTIONS_MENU_PATH + "Graphics Options", fileName = nameof(GraphicsOptions))]
    public class GraphicsOptions : UserOptions<GraphicsOptions>
    {
        [SerializeField]
        private Option<Vector2Int> _resolution = new();

        [SerializeField]
        private Option<int> _frameRateCap = new();

        [SerializeField]
        private Option<int> _quality = new();

        [SerializeField]
        private Option<EquatableEnum<FullScreenMode>> _fullscreenMode = new();

        [SerializeField]
        private Option<int> _vSyncMode = new();

        [SerializeField]
        private Option<float> _fieldOfView = new();

        public const int MAX_FRAMERATE = 360;
        public const float MAX_FOV = 100f;
        public const float MIN_FOV = 50f;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => CreateInstance();

        public Option<EquatableEnum<FullScreenMode>> FullscreenMode => _fullscreenMode;
        public Option<Vector2Int> Resolution => _resolution;
        public Option<int> FrameRateCap => _frameRateCap;
        public Option<float> FieldOfView => _fieldOfView;
        public Option<int> VSyncMode => _vSyncMode;
        public Option<int> Quality => _quality;

        protected override void Apply()
        {
            Application.targetFrameRate = _frameRateCap.Value;
            
            if (Application.isPlaying)
                Screen.SetResolution(_resolution.Value.x, _resolution.Value.y, _fullscreenMode.Value);
            
            QualitySettings.vSyncCount = _vSyncMode.Value;
            QualitySettings.SetQualityLevel(_quality.Value, true);
        }

        protected override void OnDefaultLoaded()
        {
            Reset();
            SaveToFile();
            Apply();
        }

        private void Reset()
        {
            var currentResolution = Screen.currentResolution;
            _resolution.Value = new Vector2Int(currentResolution.width, currentResolution.height);
            _quality.Value = QualitySettings.GetQualityLevel();
            _vSyncMode.Value = QualitySettings.vSyncCount;
            _frameRateCap.Value = Application.targetFrameRate;
            _fullscreenMode.Value = FullScreenMode.ExclusiveFullScreen;
            _fieldOfView.Value = 80f;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                _fieldOfView.Value = Mathf.Clamp(_fieldOfView.Value, 30f, 100f);
        }
#endif
    }
}