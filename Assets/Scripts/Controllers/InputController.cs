using UnityEngine;

public class InputController : Singleton<InputController>
{
    [SerializeField] private float _inputDistanceThresholdMultiplier = 0.05f;
    private float _inputDistanceThreshold { get { return Screen.width * _inputDistanceThresholdMultiplier; } }
    public bool CanGetInput { get; private set; }

    private void OnEnable()
    {
        BaseGameEvents.EventOnLevelStart += EventOnLevelStart;
        BaseGameEvents.EventOnLevelFinished += OnLevelFinished;
    }
    private void OnDisable()
    {
        BaseGameEvents.EventOnLevelStart -= EventOnLevelStart;
        BaseGameEvents.EventOnLevelFinished -= OnLevelFinished;
    }

    private void EventOnLevelStart()
    {
        CanGetInput = true;
    }

    private void OnLevelFinished(bool isSuccess)
    {
        CanGetInput = false;
    }

    public void GetInput(BlockElement initialBlock, Vector3 initialPosition)
    {
        if (!CanGetInput || !GameManager.Instance.PlayState)
        {
            return;
        }

        Vector3 inputVector = Input.mousePosition - initialPosition;

        if (inputVector.magnitude < _inputDistanceThreshold)
        {
            return;
        }

        Vector2 direction = Vector2.zero;

        if (Mathf.Abs(inputVector.x) >= Mathf.Abs(inputVector.y))
        {
            inputVector.y = 0;
        }
        else
        {
            inputVector.x = 0;
            inputVector.y *= -1;
        }

        direction = inputVector.normalized;
        GridMovementController.Instance.FindElementToExchange(initialBlock, direction);
    }

    public void ToggleInput(bool toggle)
    {
        CanGetInput = toggle;
    }
}
