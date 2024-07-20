using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridMovementController : Singleton<GridMovementController>
{
    [SerializeField] private GridMovementDataSO _gridMovementData;

    public void FindElementToExchange(BlockElement firstElement, Vector2 direction)
    {
        TileElement targetGridElement = GridGenerator.Instance.GetTileAtPosition(firstElement.AttachedTile.Position + direction);

        if (targetGridElement == null)
        {
            return;
        }
        if (targetGridElement.AttachedBlock == null)
        {
            return;
        }
        if (targetGridElement.AttachedBlock.BlockData.BlockType == Enums.BlockType.Obstacle)
        {
            return;
        }

        MovesController.Instance.DecreaseMoves();

        ExchangeElements(firstElement, targetGridElement.AttachedBlock);
    }

    private async void ExchangeElements(BlockElement firstBlock, BlockElement secondBlock)
    {
        InputController.Instance.ToggleInput(false);

        TileElement firstBlockTargetTile = secondBlock.AttachedTile;
        TileElement secondBlockTargetTile = firstBlock.AttachedTile;

        await SwitchMoveBlocks(firstBlock, firstBlockTargetTile, secondBlock, secondBlockTargetTile);

        bool reverse = true;

        if (GridMatchController.Instance.CheckMatchCount(firstBlockTargetTile) || GridMatchController.Instance.CheckMatchCount(secondBlockTargetTile))
        {
            reverse = false;
        }

        if (reverse)
        {
            await SwitchMoveBlocks(firstBlock, secondBlockTargetTile, secondBlock, firstBlockTargetTile);

            InputController.Instance.ToggleInput(true);

            return;
        }

        await MatchBlocks();
    }

    private async Task SwitchMoveBlocks(BlockElement firstBlock, TileElement firstBlockTargetTile, BlockElement secondBlock, TileElement secondBlockTargetTile)
    {
        firstBlock.MoveBlock(firstBlockTargetTile, _gridMovementData.BlocksSwitchDuration);
        await secondBlock.MoveBlock(secondBlockTargetTile, _gridMovementData.BlocksSwitchDuration);
    }

    public async Task MatchBlocks()
    {
        while (true)
        {
            foreach (TileElement element in GridGenerator.Instance.TileElementList)
            {
                if (GridMatchController.Instance.CheckMatchCount(element))
                {
                    GridMatchController.Instance.CheckAllMatches(element);
                }
            }

            for (int i = 0; i < GridGenerator.Instance.GridWidth; i++)
            {
                await ActivateGravity(i);
            }

            int generationCount = await BlocksController.Instance.GenerateNewBlocks();

            if (generationCount <= 0)
            {
                break;
            }

            if (!GameManager.Instance.PlayState)
            {
                break;
            }
        }

        InputController.Instance.ToggleInput(true);
        GridMatchController.Instance.CheckAllTilesForAMatch();
    }

    public async Task ActivateGravity(int column)
    {
        for (int i = GridGenerator.Instance.GridHeight - 1; i >= 0; i--)
        {
            TileElement tile = GridGenerator.Instance.GetTileAtPosition(new Vector2(column, i));
            if (tile.AttachedBlock != null)
            {
                if (tile.AttachedBlock.BlockData.BlockType == Enums.BlockType.Obstacle)
                {
                    continue;
                }
                TileElement targetTile = GridGenerator.Instance.GetEmptyTileBottom(tile);
                if (targetTile == null)
                {
                    continue;
                }
                if (targetTile.Row > i)
                {
                    await tile.AttachedBlock.MoveBlock(targetTile, _gridMovementData.BlocksRainDuration);
                }
            }
        }
    }
}
