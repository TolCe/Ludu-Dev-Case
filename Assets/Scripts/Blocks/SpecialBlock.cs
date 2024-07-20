using UnityEngine.EventSystems;

public class SpecialBlock : BlockElement, IClickable
{
    private void Start()
    {
        SetClickEvents();
    }

    public void SetClickEvents()
    {
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { OnPointerUp(); });
        EventTrigger.triggers.Add(pointerUp);
    }

    private void OnPointerUp()
    {
        Explode();
    }

    private void Explode()
    {
        if (!GameManager.Instance.PlayState || !InputController.Instance.CanGetInput)
        {
            return;
        }

        SpecialBlockData blockData = BlockData as SpecialBlockData;
        BlocksController.Instance.EnableSpecialBlock(AttachedTile, blockData.SpecialType);
    }
}
