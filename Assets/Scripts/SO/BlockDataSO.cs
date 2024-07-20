using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockDataContainer", menuName = "Block Data Container")]
public class BlockDataContainerSO : ScriptableObject
{
    public ColorBlockDataContainer ColorBlockDataContainer;
    public SpecialBlockDataContainer SpecialBlockDataContainer;
    public ObstacleBlockDataContainer ObstacleBlockDataContainer;

    public float ChanceToGenerateObstaclePercentile = 5f;

    public ColorBlockData GetColorData(Enums.BlockColors color)
    {
        return ColorBlockDataContainer.ColorBlockDataList.First(x => x.Color == color);
    }

    public SpecialBlockData GetSpecialData(Enums.SpecialTypes type)
    {
        return SpecialBlockDataContainer.SpecialBlockDataList.First(x => x.SpecialType == type);
    }

    public ObstacleBlockData GetObstacleData(Enums.ObstacleTypes type)
    {
        return ObstacleBlockDataContainer.ObstacleBlockDataList.First(x => x.ObstacleType == type);
    }
}

[Serializable]
public class ColorBlockDataContainer : BlockDataContainer
{
    public List<ColorBlockData> ColorBlockDataList;
}

[Serializable]
public class SpecialBlockDataContainer : BlockDataContainer
{
    public List<SpecialBlockData> SpecialBlockDataList;
}

[Serializable]
public class ObstacleBlockDataContainer : BlockDataContainer
{
    public List<ObstacleBlockData> ObstacleBlockDataList;
}

[Serializable]
public class BlockDataContainer
{
    public BlockElement Prefab;
}

[Serializable]
public class ColorBlockData : BlockData
{
    public Enums.BlockColors Color;
}

[Serializable]
public class SpecialBlockData : BlockData
{
    public Enums.SpecialTypes SpecialType;
}

[Serializable]
public class ObstacleBlockData : BlockData
{
    public Enums.ObstacleTypes ObstacleType;
    public int CountToBreak;
}

[Serializable]
public class BlockData
{
    public Enums.BlockType BlockType;
    public Sprite Icon;
}
