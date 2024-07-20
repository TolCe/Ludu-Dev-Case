using System;
using System.Collections;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{
    [SerializeField] private GameSettingsSO _gameSettingsData;
    private Coroutine _timerRoutine;

    private void OnEnable()
    {
        BaseGameEvents.EventOnLevelStart += OnLevelStart;
        BaseGameEvents.EventOnLevelFinished += OnLevelFinished;
        BaseGameEvents.EventOnRestart += OnRestart;
    }
    private void Start()
    {
        UIManager.Instance.WriteTime(Mathf.CeilToInt(_gameSettingsData.LevelDuration));
    }
    private void OnDisable()
    {
        BaseGameEvents.EventOnLevelStart -= OnLevelStart;
        BaseGameEvents.EventOnLevelFinished -= OnLevelFinished;
        BaseGameEvents.EventOnRestart -= OnRestart;
    }

    private void OnLevelStart()
    {
        _timerRoutine = StartCoroutine(TimerCoroutine());
    }

    private void OnLevelFinished(bool isSuccess)
    {
        if (_timerRoutine != null)
        {
            StopCoroutine(_timerRoutine);
        }
    }

    private void OnRestart()
    {
        if (_timerRoutine != null)
        {
            StopCoroutine(_timerRoutine);
        }
    }

    private IEnumerator TimerCoroutine()
    {
        float timer = _gameSettingsData.LevelDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            UIManager.Instance.WriteTime(Mathf.CeilToInt(timer));

            yield return new WaitForEndOfFrame();
        }

        GameManager.Instance.FinishGame(false);
    }
}
