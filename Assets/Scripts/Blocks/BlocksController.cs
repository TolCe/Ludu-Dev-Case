using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BlocksController : Singleton<BlocksController>
{
    [SerializeField] private BlockDataContainerSO _blockDataContainer;

    [SerializeField] private RectTransform _discardedBlockElementsContainer;

    private ObjectPool<BlockElement> _colorBlockPool;
    private ObjectPool<BlockElement> _specialBlockPool;
    private ObjectPool<BlockElement> _obstacleBlockPool;

    private Vector2[] neighborDirections = new Vector2[4] { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    protected override void Awake()
    {
        base.Awake();

        CreateBlockElementsPool();
    }
    private void OnEnable()
    {
        BaseGameEvents.EventOnRestart += OnRestart;
    }
    private void OnDisable()
    {
        BaseGameEvents.EventOnRestart -= OnRestart;
    }

    private void OnRestart()
    {
        RegenerateAllBlocks();
    }

    private void CreateBlockElementsPool()
    {
        _colorBlockPool = new ObjectPool<BlockElement>(_blockDataContainer.ColorBlockDataContainer.Prefab, 50, _discardedBlockElementsContainer);
        _specialBlockPool = new ObjectPool<BlockElement>(_blockDataContainer.SpecialBlockDataContainer.Prefab, 5, _discardedBlockElementsContainer);
        _obstacleBlockPool = new ObjectPool<BlockElement>(_blockDataContainer.ObstacleBlockDataContainer.Prefab, 2, _discardedBlockElementsContainer);
    }

    public BlockElement GenerateBlock(TileElement attachedTile, bool initialSpawn, Enums.BlockType blockType, bool isRandom = false, Enums.BlockColors color = Enums.BlockColors.None, Enums.SpecialTypes specialType = Enums.SpecialTypes.None, Enums.ObstacleTypes obstacleType = Enums.ObstacleTypes.None)
    {
        BlockElement element = null;

        if (initialSpawn && !GridGenerator.Instance.IsPreBuiltLevel)
        {
            blockType = UnityEngine.Random.Range(0f, 100f) < _blockDataContainer.ChanceToGenerateObstaclePercentile ? Enums.BlockType.Obstacle : blockType;
        }

        BlockData data = null;
        switch (blockType)
        {
            case Enums.BlockType.Regular:

                element = _colorBlockPool.Get();
                data = isRandom ? GetRandomColorData() : GetUniqueBlockData(blockType, attachedTile, color, specialType, obstacleType);

                break;
            case Enums.BlockType.Special:

                element = _specialBlockPool.Get();
                data = GetUniqueBlockData(blockType, attachedTile, color, specialType, obstacleType);

                break;
            case Enums.BlockType.Obstacle:

                obstacleType = Enums.ObstacleTypes.Rock;
                element = _obstacleBlockPool.Get();
                data = GetUniqueBlockData(blockType, attachedTile, color, specialType, obstacleType);

                break;
        }

        element.Initialize(data, attachedTile);

        return element;
    }

    private ColorBlockData GetRandomColorData()
    {
        int colorVariation = Mathf.Clamp(GridGenerator.Instance.GridData.ColorVariety + 1, 2, Enum.GetNames(typeof(Enums.BlockColors)).Length + 1);
        Enums.BlockColors color = (Enums.BlockColors)UnityEngine.Random.Range(1, colorVariation);

        return _blockDataContainer.GetColorData(color);
    }

    private BlockData GetUniqueBlockData(Enums.BlockType type, TileElement attachedGridElement, Enums.BlockColors color = Enums.BlockColors.None, Enums.SpecialTypes specialType = Enums.SpecialTypes.None, Enums.ObstacleTypes obstacleType = Enums.ObstacleTypes.None)
    {
        BlockData blockData = null;

        switch (type)
        {
            case Enums.BlockType.Regular:

                if (color == Enums.BlockColors.None)
                {
                    blockData = GetUniqueColorData(attachedGridElement);
                }
                else
                {
                    blockData = GetColorData(color);
                }

                break;
            case Enums.BlockType.Special:

                blockData = GetSpecialData(specialType);

                break;
            case Enums.BlockType.Obstacle:

                blockData = GetObstacleData(obstacleType);

                break;
        }

        return blockData;
    }

    private ColorBlockData GetUniqueColorData(TileElement attachedGridElement)
    {
        int colorVariation = Mathf.Clamp(GridGenerator.Instance.GridData.ColorVariety + 1, 2, Enum.GetNames(typeof(Enums.BlockColors)).Length + 1);

        List<int> colorIndexList = new List<int>();

        for (int i = 0; i < colorVariation; i++)
        {
            colorIndexList.Add(i);
        }

        for (int i = 0; i < neighborDirections.Length; i++)
        {
            ColorBlockData blockData = GetBlockDataAtPosition(new Vector2(attachedGridElement.Column, attachedGridElement.Row) + neighborDirections[i]) as ColorBlockData;
            if (blockData != null)
            {
                colorIndexList.Remove((int)blockData.Color);
            }
        }

        Enums.BlockColors color = (Enums.BlockColors)colorIndexList[UnityEngine.Random.Range(1, colorIndexList.Count)];

        return _blockDataContainer.GetColorData(color);
    }

    private ColorBlockData GetColorData(Enums.BlockColors color)
    {
        return _blockDataContainer.GetColorData(color);
    }

    private SpecialBlockData GetSpecialData(Enums.SpecialTypes specialType)
    {
        return _blockDataContainer.GetSpecialData(specialType);
    }

    private ObstacleBlockData GetObstacleData(Enums.ObstacleTypes obstacleType)
    {
        return _blockDataContainer.GetObstacleData(obstacleType);
    }

    public void DiscardBlock(BlockElement blockElement, TileElement attachedTile)
    {
        blockElement.transform.SetParent(_discardedBlockElementsContainer, false);
        switch (blockElement.BlockData.BlockType)
        {
            case Enums.BlockType.Regular:

                _colorBlockPool.Return(blockElement);

                break;
            case Enums.BlockType.Special:

                _specialBlockPool.Return(blockElement);

                break;
            case Enums.BlockType.Obstacle:

                _obstacleBlockPool.Return(blockElement);

                break;
        }

        if (blockElement.BlockData.BlockType == Enums.BlockType.Regular)
        {
            GridMatchController.Instance.CheckNeighborForObstacle(attachedTile);
        }

        ParticleController.Instance.PlayParticleAtPosition(attachedTile.RectTransform.position);
    }

    public BlockData GetBlockDataAtPosition(Vector2 position)
    {
        TileElement tileElement = GridGenerator.Instance.GetTileAtPosition(position);
        return tileElement?.AttachedBlock?.BlockData;
    }

    public async Task<int> GenerateNewBlocks()
    {
        int generatedAmount = 0;
        for (int i = 0; i < GridGenerator.Instance.GridWidth; i++)
        {
            TileElement tile = GridGenerator.Instance.GetTileAtPosition(new Vector2(i, 0));
            TileElement emptyTileAtBottom = GridGenerator.Instance.GetEmptyTileBottom(tile);

            if (emptyTileAtBottom != null)
            {
                for (int j = 0; j <= emptyTileAtBottom.Row; j++)
                {
                    generatedAmount++;
                    GenerateBlock(tile, false, Enums.BlockType.Regular, true);
                    await GridMovementController.Instance.ActivateGravity(tile.Column);
                }
            }
        }

        return generatedAmount;
    }

    public bool CheckForSpecialBlocks(int matchCount, TileElement tileElement)
    {
        if (matchCount >= GridGenerator.Instance.GridData.CountToGenerateBomb)
        {
            GenerateBlock(tileElement, false, Enums.BlockType.Special, false, Enums.BlockColors.None, Enums.SpecialTypes.Bomb);

            return true;
        }
        else if (matchCount >= GridGenerator.Instance.GridData.CountToGenerateRocket)
        {
            float randomChance = UnityEngine.Random.Range(0f, 100f);
            if (randomChance > 50f)
            {
                GenerateBlock(tileElement, false, Enums.BlockType.Special, false, Enums.BlockColors.None, Enums.SpecialTypes.RocketH);
            }
            else
            {
                GenerateBlock(tileElement, false, Enums.BlockType.Special, false, Enums.BlockColors.None, Enums.SpecialTypes.RocketV);
            }

            return true;
        }

        return false;
    }

    public void CheckForObstacleBlocks(TileElement tileElement)
    {
        if (UnityEngine.Random.Range(0f, 100f) < _blockDataContainer.ChanceToGenerateObstaclePercentile)
        {
            GenerateBlock(tileElement, false, Enums.BlockType.Obstacle, false, Enums.BlockColors.None, Enums.SpecialTypes.None, Enums.ObstacleTypes.Rock);
        }
    }

    public void EnableSpecialBlock(TileElement specialAttachedTile, Enums.SpecialTypes specialType)
    {
        List<TileElement> blocksToClear = new List<TileElement>();

        switch (specialType)
        {
            case Enums.SpecialTypes.RocketH:

                for (int i = 0; i < GridGenerator.Instance.GridWidth; i++)
                {
                    blocksToClear.Add(GridGenerator.Instance.GetTileAtPosition(new Vector2(i, specialAttachedTile.Row)));
                }

                break;
            case Enums.SpecialTypes.RocketV:

                for (int i = 0; i < GridGenerator.Instance.GridHeight; i++)
                {
                    blocksToClear.Add(GridGenerator.Instance.GetTileAtPosition(new Vector2(specialAttachedTile.Column, i)));
                }

                break;
            case Enums.SpecialTypes.Bomb:

                Vector2 startPos = new Vector2(specialAttachedTile.Column, specialAttachedTile.Row);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        TileElement tile = GridGenerator.Instance.GetTileAtPosition(startPos + new Vector2(j, i));
                        blocksToClear.Add(tile);
                    }
                }

                break;
        }

        foreach (TileElement element in blocksToClear)
        {
            if (element?.AttachedBlock != null)
            {
                if (element.AttachedBlock.BlockData.BlockType != Enums.BlockType.Obstacle)
                {
                    element.AttachedBlock?.DeactivateBlock();
                }
            }
        }

        GridMovementController.Instance.MatchBlocks();
    }

    public void RegenerateAllBlocks()
    {
        foreach (TileElement tile in GridGenerator.Instance.TileElementList)
        {
            tile.AttachedBlock?.DeactivateBlock();
        }

        for (int i = 0; i < GridGenerator.Instance.GridWidth; i++)
        {
            for (int j = 0; j < GridGenerator.Instance.GridHeight; j++)
            {
                Instance.GenerateBlock(GridGenerator.Instance.GetTileAtPosition(new Vector2(i, j)), false, Enums.BlockType.Regular);
            }
        }
    }
}
