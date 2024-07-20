using UnityEngine;
using UnityEngine.EventSystems;

public class ColorBlock : BlockElement, IClickable
{
    private void Start()
    {
        SetClickEvents();
    }

    public void SetClickEvents()
    {
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { OnPointerDown(); });
        EventTrigger.triggers.Add(pointerDown);
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { OnPointerUp(); });
        EventTrigger.triggers.Add(pointerUp);
    }

    private void OnPointerDown()
    {
        InitialInputPosition = Input.mousePosition;
    }

    private void OnPointerUp()
    {
        InputController.Instance.GetInput(this, InitialInputPosition);
    }
}
