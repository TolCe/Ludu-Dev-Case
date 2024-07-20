using UnityEngine;

public class MovesController : Singleton<MovesController>
{
    [SerializeField] private GameSettingsSO _gameSettingsData;
    private int _movesLeft;

    private void OnEnable()
    {
        BaseGameEvents.EventOnLevelStart += OnLevelStart;
    }
    private void OnDisable()
    {
        BaseGameEvents.EventOnLevelStart -= OnLevelStart;
    }

    private void OnLevelStart()
    {
        _movesLeft = _gameSettingsData.StartMoveCount;
        UIManager.Instance.WriteMoves(_movesLeft);
    }

    public void DecreaseMoves()
    {
        _movesLeft--;

        UIManager.Instance.WriteMoves(_movesLeft);

        if (!CheckIfMoveLeft())
        {
            GameManager.Instance.FinishGame(false);
        }
    }

    private bool CheckIfMoveLeft()
    {
        return _movesLeft > 0;
    }
}
