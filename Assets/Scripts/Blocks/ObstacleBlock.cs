using UnityEngine;

public class ObstacleBlock : BlockElement
{
    private ObstacleBlockData _obstacleBlockData;

    private int _currentHealth;

    public override void Initialize(BlockData blockData, TileElement attachedGridElement)
    {
        base.Initialize(blockData, attachedGridElement);

        _obstacleBlockData = BlockData as ObstacleBlockData;
        _currentHealth = _obstacleBlockData.CountToBreak;
    }

    public void DecreaseHealth()
    {
        _currentHealth--;

        if (_currentHealth <= 0)
        {
            BreakObstacle();
        }
    }

    private void BreakObstacle()
    {
        DeactivateBlock();
    }
}
