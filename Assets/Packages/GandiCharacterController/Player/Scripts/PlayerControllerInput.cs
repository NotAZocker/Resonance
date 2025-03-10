using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(FirstPersonController))]
public class PlayerControllerInput : MonoBehaviour
{
    //settings
    [SerializeField]
    private bool lockCursor = true;
    [SerializeField]
    private float mouseSensitivity = 2.0f;
    [SerializeField]
    private bool toggleSprint = true;
    [SerializeField]
    private bool toggleCrouch = true;

    //globals
    private FirstPersonController ownController;


    void Awake()
    {
        ownController = GetComponent<FirstPersonController>();
    }


    void Start()
    {
        //ownController.revokeIsSprinting = toggleSprint;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    void Update()
    {
        //camera rotation
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 MouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            ownController.InputRotation(MouseMovement * mouseSensitivity);
        }

        //movement
        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        ownController.InputMovement(movementInput);

        //jump
        if (Input.GetButtonDown("Jump"))
        {
            ownController.InputJump();
        }
        return;
        //sprint
        if (toggleSprint)
        {
            if (Input.GetButtonDown("Sprint"))
            {
                ownController.InputSprint(!ownController.IsSprinting);
            }
            if (movementInput.magnitude < 0.01)
			{
                ownController.InputSprint(false);
            }
        }
        else
        {
            if (Input.GetButtonDown("Sprint"))
            {
                ownController.InputSprint(true);
            }
            else if (Input.GetButtonUp("Sprint"))
            {
                ownController.InputSprint(false);
            }
        }

        //crouch
        if (toggleCrouch)
        {
            if (Input.GetButtonDown("Crouch"))
            {
                ownController.InputCrouch(!ownController.IsCrouching);
            }
        }
        else
        {
            if (Input.GetButtonDown("Crouch"))
            {
                ownController.InputCrouch(true);
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                ownController.InputCrouch(false);
            }
        }

    }
}
