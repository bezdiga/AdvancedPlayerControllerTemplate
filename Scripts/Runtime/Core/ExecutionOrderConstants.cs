namespace HatchStudio.Manager
{
    public static class ExecutionOrderConstants
    {
        public const int SCRIPTABLE_SINGLETON = -100000;
        public const int SCENE_SINGLETON = -10000;
        public const int BEFORE_DEFAULT_3 = -1000;
        public const int BEFORE_DEFAULT_2 = -100;
        public const int BEFORE_DEFAULT_1 = -10;
        public const int AFTER_DEFAULT_1 = 10;
        public const int AFTER_DEFAULT_2 = 100;
    }
    
    public static partial class TagConstants
    {
        // Tag names
        public const string MAIN_CAMERA = "MainCamera";
        public const string PLAYER = "Player";
        public const string GAME_CONTROLLER = "GameController";
    }
}