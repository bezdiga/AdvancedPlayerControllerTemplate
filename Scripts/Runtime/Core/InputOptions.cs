using HatchStudio.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = OPTIONS_MENU_PATH + "Input Options", fileName = nameof(InputOptions))]
public class InputOptions : UserOptions<InputOptions>
{
    [SerializeField]
    private Option<bool> _invertMouse = new(false);

    [SerializeField]
    private Option<float> _mouseSensitivity = new(0.5f);
        
    [SerializeField]
    private Option<bool> _autoRun = new(false);
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() => CreateInstance();
    
    public Option<float> MouseSensitivity => _mouseSensitivity;
    public Option<bool> InvertMouse => _invertMouse;
    public Option<bool> AutoRun => _autoRun;
        
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
            _mouseSensitivity.Value = Mathf.Clamp(_mouseSensitivity.Value, 0f, 1f);
    }
#endif
}
