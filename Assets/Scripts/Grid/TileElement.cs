using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileElement : MonoBehaviour
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    public BlockElement AttachedBlock { get; private set; }

    [SerializeField] private RectTransform _rectTransform;
    public RectTransform RectTransform { get { return _rectTransform; } }

    public Vector2 Position { get { return new Vector2(Column, Row); } }

    public void Initialize(int row, int column, float elementSize, Vector2 totalSize)
    {
        Row = row;
        Column = column;

        gameObject.SetActive(true);

        _rectTransform.sizeDelta = elementSize * Vector2.one;
        transform.localPosition = elementSize * ((column - (totalSize.x * 0.5f) + 0.5f) * Vector2.right + (row - (totalSize.y * 0.5f) + 0.5f) * Vector2.down);
    }

    public void SetAttachedBlockElement(BlockElement attachedBlockElement)
    {
        AttachedBlock = attachedBlockElement;
    }

    public void ClearAttachment(BlockElement blockElement)
    {
        if (AttachedBlock == blockElement)
        {
            AttachedBlock = null;
        }
    }
}
