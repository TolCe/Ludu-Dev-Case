using System;

public static class BaseGameEvents
{
    public static Action EventOnLevelStart;
    public static void CallLevelStart() => EventOnLevelStart?.Invoke();

    public static Action<bool> EventOnLevelFinished;
    public static void CallLevelFinished(bool isSuccess) => EventOnLevelFinished?.Invoke(isSuccess);

    public static Action EventOnRestart;
    public static void CallRestart() => EventOnRestart?.Invoke();
}
