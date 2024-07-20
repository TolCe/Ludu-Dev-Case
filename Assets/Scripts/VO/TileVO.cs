using System;

[Serializable]
public class TileVO
{
    public Enums.BlockType Type = Enums.BlockType.None;
    public Enums.BlockColors Color = Enums.BlockColors.None;
    public Enums.SpecialTypes SpecialType = Enums.SpecialTypes.None;
    public Enums.ObstacleTypes ObstacleType = Enums.ObstacleTypes.None;
}
