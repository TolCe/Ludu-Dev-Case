using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _topRestartButton;

    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _movesText;

    private void OnEnable()
    {
        _startButton.onClick.AddListener(OnStartButtonClick);
        _restartButton.onClick.AddListener(OnRestartButtonClick);
        _topRestartButton.onClick.AddListener(OnRestartButtonClick);

        _startButton.gameObject.SetActive(true);
        _restartButton.gameObject.SetActive(false);

        BaseGameEvents.EventOnLevelFinished += OnLevelFinished;
    }
    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(OnStartButtonClick);
        _restartButton.onClick.RemoveListener(OnRestartButtonClick); ;
        _topRestartButton.onClick.RemoveListener(OnRestartButtonClick);

        BaseGameEvents.EventOnLevelFinished -= OnLevelFinished;
    }

    private void OnLevelFinished(bool isSuccess)
    {
        _restartButton.gameObject.SetActive(true);
    }

    private void OnStartButtonClick()
    {
        _startButton.gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    private void OnRestartButtonClick()
    {
        _startButton.gameObject.SetActive(true);
        _restartButton.gameObject.SetActive(false);
        GameManager.Instance.RestartGame();
    }

    public void WriteTime(int timeLeft)
    {
        _timerText.text = $"{timeLeft}s";
    }

    public void WriteMoves(int movesLeft)
    {
        _movesText.text = $"{movesLeft}";
    }
}
