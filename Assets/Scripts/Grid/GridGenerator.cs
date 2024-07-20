using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGenerator : Singleton<GridGenerator>
{
    [SerializeField] private LevelDataContainerSO _levelDataContainer;
    [SerializeField] private GridDataSO _gridData;
    public GridDataSO GridData { get { return _gridData; } }

    [SerializeField] private RectTransform _tileElementContainer;
    [SerializeField] private RectTransform _tileElementFreeContainer;

    private ObjectPool<TileElement> _tileElementsPool;

    public List<TileElement> TileElementList { get; private set; }

    public bool IsPreBuiltLevel { get { return _levelDataContainer.PlayPreBuiltLevels; } }

    public int GridWidth { get; private set; }
    public int GridHeight { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        CreateGridElementsPool();
    }
    private void Start()
    {
        CreateGrid();
        SetGridLayoutFeatures();
    }

    private void CreateGridElementsPool()
    {
        _tileElementsPool = new ObjectPool<TileElement>(_gridData.TileElementPrefab, 50, _tileElementContainer);
    }

    private void SetGridLayoutFeatures()
    {
        float elementSize = _tileElementContainer.sizeDelta.x / GridWidth;
        _tileElementContainer.sizeDelta = 1.1f * elementSize * new Vector2(GridWidth, GridHeight);
    }

    private void CreateGrid()
    {
        GridWidth = _gridData.ColumnSize;
        GridHeight = _gridData.RowSize;

        LevelDataSO levelData = null;

        if (_levelDataContainer.PlayPreBuiltLevels)
        {
            levelData = _levelDataContainer.LevelDataList[Mathf.Clamp(_levelDataContainer.LevelIndex, 0, _levelDataContainer.LevelDataList.Count)];
            GridWidth = levelData.Grid.GridWidth;
            GridHeight = levelData.Grid.GridHeight;
        }

        TileElementList = new List<TileElement>();

        for (int i = 0; i < GridWidth; i++)
        {
            for (int j = 0; j < GridHeight; j++)
            {
                TileElement element = GenerateNewTileElement(j, i);
                TileElementList.Add(element);

                if (_levelDataContainer.PlayPreBuiltLevels)
                {
                    TileVO tile = levelData.Grid.Grid[i * GridHeight + j];
                    BlocksController.Instance.GenerateBlock(element, true, tile.Type, false, tile.Color, tile.SpecialType, tile.ObstacleType);
                }
                else
                {
                    BlocksController.Instance.GenerateBlock(element, true, Enums.BlockType.Regular);
                }
            }
        }
    }

    private TileElement GenerateNewTileElement(int row, int column)
    {
        float elementSize = _tileElementContainer.sizeDelta.x / GridWidth;

        TileElement element = _tileElementsPool.Get();
        element.Initialize(row, column, elementSize, new Vector2(GridWidth, GridHeight));
        return element;
    }

    public TileElement GetTileAtPosition(Vector2 position)
    {
        return TileElementList.FirstOrDefault(x => x.Column == position.x && x.Row == position.y);
    }

    public TileElement GetEmptyTileBottom(int column)
    {
        List<TileElement> tileList = TileElementList.FindAll(x => x.Column == column && x.AttachedBlock == null);
        int maxRowIndex = 0;
        foreach (TileElement element in tileList)
        {
            if (element.Row > maxRowIndex)
            {
                maxRowIndex = element.Row;
            }
        }

        return tileList.FirstOrDefault(x => x.Row == maxRowIndex);
    }

    public TileElement GetEmptyTileBottom(TileElement tile)
    {
        List<TileElement> tileList = TileElementList.FindAll(x => x.Column == tile.Column && x.Row >= tile.Row && x.AttachedBlock != null);
        List<TileElement> emptyTileList = TileElementList.FindAll(x => x.Column == tile.Column && x.Row >= tile.Row && x.AttachedBlock == null);

        tileList = tileList.OrderByDescending(x => x.Row).ToList();
        emptyTileList = emptyTileList.OrderByDescending(x => x.Row).ToList();

        TileElement obstacleTile = tileList.LastOrDefault(x => x.AttachedBlock.BlockData.BlockType == Enums.BlockType.Obstacle);
        int obstacleRow = obstacleTile == null ? -1 : obstacleTile.Row;

        int maxRowIndex = 0;
        if (obstacleRow >= tile.Row)
        {
            foreach (TileElement element in emptyTileList)
            {
                if (element.Row > maxRowIndex && element.Row <= obstacleRow)
                {
                    maxRowIndex = element.Row;
                }
            }
        }
        else
        {
            foreach (TileElement element in emptyTileList)
            {
                if (element.Row > maxRowIndex || (tile.Row < obstacleRow && element.Row < obstacleRow))
                {
                    maxRowIndex = element.Row;
                }
            }
        }

        return emptyTileList.FirstOrDefault(x => x.Row == maxRowIndex);
    }
}
