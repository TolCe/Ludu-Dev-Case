using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettingsSO : ScriptableObject
{
    public int StartMoveCount = 10;
    public float LevelDuration = 60f;
}
