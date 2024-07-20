public class GameManager : Singleton<GameManager>
{
    public bool PlayState { get; private set; }

    public void StartGame()
    {
        PlayState = true;
        BaseGameEvents.CallLevelStart();
    }

    public void RestartGame()
    {
        PlayState = false;
        BaseGameEvents.CallRestart();
    }

    public void FinishGame(bool isSuccess)
    {
        PlayState = false;
        BaseGameEvents.CallLevelFinished(isSuccess);
    }
}
