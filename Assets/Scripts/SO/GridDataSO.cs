using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Grid Data")]
public class GridDataSO : ScriptableObject
{
    public int RowSize;
    public int ColumnSize;

    public float ElementSizeX = 100f;
    public float ElementSizeY = 100f;

    public float ElementSpacingX = 5f;
    public float ElementSpacingY = 5f;

    public TileElement TileElementPrefab;

    public int ColorVariety = 6;
    public int MatchSize = 2;
    public int CountToGenerateRocket = 4;
    public int CountToGenerateBomb = 5;
}
