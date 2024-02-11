namespace Game
{
    public interface IGameListener
    {
        public LoadingPriority Priority { get; }
    }

    public interface IGameInitializeListener : IGameListener
    {
        bool IsInitialized { get; }
        void OnGameInitialize();
    }

    public interface IGameStartListener : IGameListener
    {
        void OnGameStart();
    }

    public interface IGameFinishListener : IGameListener
    {
        void OnGameFinish();
    }

    public interface IGamePauseListener : IGameListener
    {
        void OnGamePause();
    }

    public interface IGameResumeListener : IGameListener
    {
        void OnGameResume();
    }

    public interface IGameUpdateListener : IGameListener
    {
        void OnUpdate(float deltaTime);
    }

    public interface IGameFixedUpdateListener : IGameListener
    {
        void OnFixedUpdate(float fixedDeltaTime);
    }

    public interface IGameLateUpdateListener : IGameListener
    {
        void OnLateUpdate(float deltaTime);
    }
}
