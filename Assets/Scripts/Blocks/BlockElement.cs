using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockElement : MonoBehaviour
{
    public BlockData BlockData { get; private set; }

    public TileElement AttachedTile { get; private set; }

    public EventTrigger EventTrigger;
    [SerializeField] private Image _iconImage;

    protected Vector3 InitialInputPosition;

    public virtual void Initialize(BlockData blockData, TileElement attachedGridElement)
    {
        BlockData = blockData;
        _iconImage.sprite = BlockData.Icon;

        SetAttachedGridElement(attachedGridElement);
        SnapToTile();

        gameObject.SetActive(true);
    }

    private void SetAttachedGridElement(TileElement attachedGridElement)
    {
        AttachedTile = attachedGridElement;
        AttachedTile.SetAttachedBlockElement(this);
    }

    public void SnapToTile()
    {
        transform.SetParent(AttachedTile.transform, false);
        transform.localPosition = Vector3.zero;
    }

    public void DeactivateBlock()
    {
        TileElement attachedTile = AttachedTile;
        ClearAttachment();
        BlocksController.Instance.DiscardBlock(this, attachedTile);
    }

    private void ClearAttachment()
    {
        AttachedTile.ClearAttachment(this);
        AttachedTile = null;
    }

    public async Task MoveBlock(TileElement targetTile, float duration)
    {
        ClearAttachment();
        SetAttachedGridElement(targetTile);
        await transform.DOMove(targetTile.transform.position, duration).AsyncWaitForCompletion();
        SnapToTile();
    }
}
