using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerInputActions.PlayerActions Actions { get; private set; }
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        Actions = inputActions.Player;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
