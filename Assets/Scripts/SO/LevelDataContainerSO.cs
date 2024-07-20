using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelContainer", menuName = "Level Container")]
public class LevelDataContainerSO : ScriptableObject
{
    public List<LevelDataSO> LevelDataList;
    public bool PlayPreBuiltLevels;
    public int LevelIndex;
}
