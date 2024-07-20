using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

public class GridMatchController : Singleton<GridMatchController>
{
    [SerializeField] private GridDataSO _gridData;

    private Vector2[] _neighborDirections = new Vector2[4] { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    public bool CheckMatchCount(TileElement tileElement)
    {
        if (CheckHorizontalMatchCount(tileElement) + 1 >= GridGenerator.Instance.GridData.MatchSize || CheckVerticalMatchCount(tileElement) + 1 >= GridGenerator.Instance.GridData.MatchSize)
        {
            return true;
        }

        return false;
    }

    public void CheckAllMatches(TileElement tileElement)
    {
        List<TileElement> allMatchedList = new List<TileElement>()
        {
            tileElement
        };
        List<TileElement> newMatchesList = new List<TileElement>()
        {
            tileElement
        };

        while (true)
        {
            newMatchesList = CheckTileNeighbors(allMatchedList, newMatchesList);
            if (newMatchesList.Count > 0)
            {
                allMatchedList.AddRange(newMatchesList);
            }
            else
            {
                break;
            }
        }

        MatchTiles(tileElement, allMatchedList);
    }

    private int CheckHorizontalMatchCount(TileElement tileElement)
    {
        List<TileElement> matchList = new List<TileElement>();
        TileElement tileToCheck = null;
        for (int i = tileElement.Column - 1; i >= 0; i--)
        {
            Vector2 checkPos = new Vector2(i, tileElement.Row);
            tileToCheck = GridGenerator.Instance.GetTileAtPosition(checkPos);

            if (!CheckTileMatch(tileElement, tileToCheck, matchList))
            {
                break;
            }
        }
        for (int i = tileElement.Column + 1; i < GridGenerator.Instance.GridWidth; i++)
        {
            Vector2 checkPos = new Vector2(i, tileElement.Row);
            tileToCheck = GridGenerator.Instance.GetTileAtPosition(checkPos);

            if (!CheckTileMatch(tileElement, tileToCheck, matchList))
            {
                break;
            }
        }

        return matchList.Count;
    }

    private int CheckVerticalMatchCount(TileElement tileElement)
    {
        List<TileElement> matchList = new List<TileElement>();
        TileElement tileToCheck = null;
        for (int i = tileElement.Row - 1; i >= 0; i--)
        {
            Vector2 checkPos = new Vector2(tileElement.Column, i);
            tileToCheck = GridGenerator.Instance.GetTileAtPosition(checkPos);

            if (!CheckTileMatch(tileElement, tileToCheck, matchList))
            {
                break;
            }
        }
        for (int i = tileElement.Row + 1; i < GridGenerator.Instance.GridHeight; i++)
        {
            Vector2 checkPos = new Vector2(tileElement.Column, i);
            tileToCheck = GridGenerator.Instance.GetTileAtPosition(checkPos);

            if (!CheckTileMatch(tileElement, tileToCheck, matchList))
            {
                break;
            }
        }

        return matchList.Count;
    }

    private bool CheckTileMatch(TileElement tileElement, TileElement tileToCheck, List<TileElement> matchList)
    {
        if (CheckMatch(tileElement, tileToCheck))
        {
            if (!matchList.Contains(tileToCheck))
            {
                matchList.Add(tileToCheck);

                return true;
            }
        }

        return false;
    }

    private int CheckPotentialHorizontalMatchCount(TileElement initialTile, TileElement tileElement, Enums.BlockColors color)
    {
        List<TileElement> matchList = new List<TileElement>();
        TileElement tileToCheck = null;
        for (int i = tileElement.Column - 1; i >= 0; i--)
        {
            Vector2 checkPos = new Vector2(i, tileElement.Row);
            tileToCheck = GridGenerator.Instance.GetTileAtPosition(checkPos);

            if (!CheckPotentialMatch(initialTile, tileToCheck, matchList, color))
            {
                break;
            }
        }
        for (int i = tileElement.Column + 1; i < GridGenerator.Instance.GridWidth; i++)
        {
            Vector2 checkPos = new Vector2(i, tileElement.Row);
            tileToCheck = GridGenerator.Instance.GetTileAtPosition(checkPos);

            if (!CheckPotentialMatch(initialTile, tileToCheck, matchList, color))
            {
                break;
            }
        }

        return matchList.Count;
    }

    private int CheckPotentialVerticalMatchCount(TileElement initialTile, TileElement tileElement, Enums.BlockColors color)
    {
        List<TileElement> matchList = new List<TileElement>();
        TileElement tileToCheck = null;
        for (int i = tileElement.Row - 1; i >= 0; i--)
        {
            Vector2 checkPos = new Vector2(tileElement.Column, i);
            tileToCheck = GridGenerator.Instance.GetTileAtPosition(checkPos);

            if (!CheckPotentialMatch(initialTile, tileToCheck, matchList, color))
            {
                break;
            }
        }
        for (int i = tileElement.Row + 1; i < GridGenerator.Instance.GridHeight; i++)
        {
            Vector2 checkPos = new Vector2(tileElement.Column, i);
            tileToCheck = GridGenerator.Instance.GetTileAtPosition(checkPos);

            if (!CheckPotentialMatch(initialTile, tileToCheck, matchList, color))
            {
                break;
            }
        }

        return matchList.Count;
    }

    private bool CheckPotentialMatch(TileElement initialTile, TileElement tileToCheck, List<TileElement> matchList, Enums.BlockColors color)
    {
        bool matched = false;
        if (tileToCheck != null)
        {
            if (initialTile != tileToCheck)
            {
                if (CheckMatch(tileToCheck, color))
                {
                    if (!matchList.Contains(tileToCheck))
                    {
                        matchList.Add(tileToCheck);
                        matched = true;
                    }
                }
            }
        }

        return matched;
    }

    public void MatchTiles(TileElement tile, List<TileElement> matchedTileList)
    {
        foreach (TileElement element in matchedTileList)
        {
            element.AttachedBlock?.DeactivateBlock();
        }

        if (!BlocksController.Instance.CheckForSpecialBlocks(matchedTileList.Count, tile))
        {
            BlocksController.Instance.CheckForObstacleBlocks(tile);
        }
    }

    private List<TileElement> CheckTileNeighbors(List<TileElement> allMatchList, List<TileElement> previousMatchList)
    {
        List<TileElement> matchedNeighborList = new List<TileElement>();

        foreach (TileElement tile in previousMatchList)
        {
            foreach (Vector2 direction in _neighborDirections)
            {
                TileElement neighborElement = GridGenerator.Instance.GetTileAtPosition(tile.Position + direction);
                if (neighborElement != null)
                {
                    if (!allMatchList.Contains(neighborElement))
                    {
                        if (CheckMatch(tile, neighborElement))
                        {
                            matchedNeighborList.Add(neighborElement);
                        }
                    }
                }
            }
        }

        return matchedNeighborList;
    }

    private bool CheckMatch(TileElement tileElement, TileElement neighborElement)
    {
        if (tileElement?.AttachedBlock?.BlockData.BlockType != Enums.BlockType.Regular || neighborElement?.AttachedBlock?.BlockData.BlockType != Enums.BlockType.Regular)
        {
            return false;
        }

        ColorBlockData firstData = tileElement.AttachedBlock.BlockData as ColorBlockData;
        ColorBlockData secondData = neighborElement.AttachedBlock.BlockData as ColorBlockData;

        return firstData.Color == secondData.Color;
    }

    private bool CheckMatch(TileElement neighborElement, Enums.BlockColors color)
    {
        if (neighborElement?.AttachedBlock?.BlockData.BlockType == Enums.BlockType.Regular)
        {
            ColorBlockData secondData = neighborElement.AttachedBlock.BlockData as ColorBlockData;

            return color == secondData.Color;
        }

        return false;
    }

    public void CheckNeighborForObstacle(TileElement tile)
    {
        foreach (Vector2 direction in _neighborDirections)
        {
            TileElement neighborTile = GridGenerator.Instance.GetTileAtPosition(tile.Position + direction);
            if (neighborTile?.AttachedBlock?.BlockData.BlockType == Enums.BlockType.Obstacle)
            {
                ObstacleBlock obstacleBlock = neighborTile.AttachedBlock as ObstacleBlock;
                obstacleBlock.DecreaseHealth();
            }
        }
    }

    public void CheckAllTilesForAMatch()
    {
        bool hasMatch = false;
        bool hasSpecial = false;

        foreach (TileElement tile in GridGenerator.Instance.TileElementList)
        {
            if (tile.AttachedBlock?.BlockData.BlockType == Enums.BlockType.Special)
            {
                hasSpecial = true;
                break;
            }

            ColorBlockData firstData = tile.AttachedBlock?.BlockData as ColorBlockData;

            if (firstData != null)
            {
                foreach (Vector2 direction in _neighborDirections)
                {
                    //_matchCount = CheckTileNeighbors(tile, firstData.Color).Count + 1;

                    Vector2 pos = tile.Position + direction;
                    TileElement tileToCheck = GridGenerator.Instance.GetTileAtPosition(pos);

                    if (tileToCheck?.AttachedBlock != null)
                    {
                        if (tileToCheck.AttachedBlock.BlockData.BlockType != Enums.BlockType.Obstacle)
                        {
                            if (CheckPotentialHorizontalMatchCount(tile, tileToCheck, firstData.Color) + 1 >= GridGenerator.Instance.GridData.MatchSize
                                || CheckPotentialVerticalMatchCount(tile, tileToCheck, firstData.Color) + 1 >= GridGenerator.Instance.GridData.MatchSize)
                            {
                                hasMatch = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (hasMatch)
            {
                break;
            }
        }

        if (!hasMatch && !hasSpecial)
        {
            GameManager.Instance.FinishGame(false);
        }
    }
}
