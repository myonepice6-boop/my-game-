using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class InputManager : MonoBehaviour
{
   PlayerControls playerControls;

    AnimatorManager animatorManager;
    PlayerMovement playerMovement;
    public Vector2 movementInput;
    public Vector2 cameraMovementInPut;

    public float vertecalInput;

    public float horizontalInput;

    public float cameraInputX;
    public float cameraInputY;

    public float movementAmount;

    [Header("Input Buttons Flag")]
    public bool bInput;
    public bool jumpInput;

    public bool fireInput;
    public bool reloadInput;
    public bool scopeInput;


    private void Awake()
    {
        animatorManager=GetComponent<AnimatorManager>();
        playerMovement=GetComponent<PlayerMovement>();
    }
    private void OnEnable()
    {
        if(playerControls ==null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.CameraMovement.performed += i => cameraMovementInPut = i.ReadValue<Vector2>();
            playerControls.PlayerActions.B.performed += i => bInput = true;
            playerControls.PlayerActions.B.canceled += i => bInput = false;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.Fire.performed += i => fireInput = true;
            playerControls.PlayerActions.Fire.canceled += i => fireInput = false;

            playerControls.PlayerActions.Reload.performed += i => reloadInput = true;

            playerControls.PlayerActions.Scope.performed += i => scopeInput = true;
            playerControls.PlayerActions.Scope.canceled += i => scopeInput = false;


        }

        playerControls.Enable();
    }

    private void Fire_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }
    //Cursor
    private void OnDisable()
    {
        playerControls.Disable();
    }
    public void HandelAllInputes()
    {
        HandelMovementInput();
        HandelSprintingInput();
        HandelJumpingInput();
    }

    private void HandelMovementInput()
    {
        vertecalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraMovementInPut.x;
        cameraInputY = cameraMovementInPut.y;

        movementAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(vertecalInput));
        animatorManager.ChangeAnimatorValue(0,movementAmount,playerMovement.isSprinting);

    }

    private void HandelSprintingInput()
    {
        if(bInput&&movementAmount>0.5f)
        {
            playerMovement.isSprinting=true;

        }
        else
        {
            playerMovement.isSprinting = false;
        }
    }

    private void HandelJumpingInput()
    {
if(jumpInput)
        {
            jumpInput=false;
            playerMovement.HandelJumping();
        }
    }
}
