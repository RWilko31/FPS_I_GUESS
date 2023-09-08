using UnityEngine;
using UnityEngine.InputSystem;

public delegate void gameEvent();

static class currentInput
{
    public static Vector2 xyMove;
    public static Vector2 xyLook;
    public static standingPosition standingPos;
}
static class gameState
{
    public static bool inGame = false;
    public static bool paused = false;
    public static int area = -1;
}
enum standingPosition
{
     standing = 1,
     crouch = 2,
     prone = 3
}
public class InputController : MonoBehaviour
{
    /// Reads the player input from the inputManager and executes a corresponding function
    /// Simply subscribe any scripts requiring input to the corresponding event to trigger a function on the input

    //-----variables-----\\

    //character control events
    public event gameEvent jump, doubleJump, sprint, sprintCancelled, sprintToggle, changePosition, slide, item1, item2, item3, item4, start, select, message, fire, fireCancelled, aim, aimCancelled, lethal, tactical, reload, action, swap;
    //Menu control events
    public event gameEvent resume, cancel, leftTab, rightTab, messageMenu, submit, navigate, inventory;

    #region Input
    InputManager Controls;
    PlayerInput playerInput;
    [Header("Controller scaler")]
    [SerializeField] private float controllerScaler = 14;
    [Header("Cursor")]
    public bool hideCursor = false;
    public bool isLocked = false;
    #endregion

    private bool proneLatch = false; //latches when prone is pressed so a crouch input isn't detected while releasing the button

    //-----functions-----\\

    private void Awake()
    {
        //get input
        Controls = new InputManager();
        playerInput = GetComponent<PlayerInput>();

        //assign Inputs
        ReadInput();
    }
    private void Update()
    { CursorLockAndUnlockState(); }
    private void OnEnable() //Enables user input 
    {
        if(Controls == null) Controls = new InputManager();
        if (!gameState.inGame)
        {
            Controls.Menu.Enable();
            Controls.Default.Disable();
        }
        else
        {
            Controls.Menu.Disable();
            Controls.Default.Enable();
        }
    } 
    private void OnDisable() { Controls.Default.Disable(); Controls.Menu.Disable(); } //Disables user input
    public string GetControlScheme()
    {
        if (!playerInput) return "Null";
        return playerInput.currentControlScheme; 
    }
    public void CursorLockAndUnlockState()
    {   //Locks and hides the cursor if in game or not using keyboard & mouse 

        //hides cursor when using controller
        if (playerInput.currentControlScheme == "Keyboard & Mouse") { hideCursor = false; }
        else { hideCursor = true; }

        //checks if cursor should be visible when using mouse
        if (hideCursor || ((!gameState.paused) && gameState.inGame))
        { if (!isLocked) { isLocked = true; Cursor.visible = false; } }

        //Frees cursor
        else if (isLocked) { isLocked = false; Cursor.visible = true; }
    }
    public void PauseCharacterInput(string mode)
    {
        //disable controls
        Controls.Menu.Disable();
        Controls.Default.Disable();

        //Enable selected controls
        switch (mode)
        {
            case "Player":
                Controls.Default.Enable(); 
                playerInput.SwitchCurrentActionMap("Default");
                break;

            case "Menu":
                Controls.Menu.Enable(); 
                playerInput.SwitchCurrentActionMap("Menu");
                break;
        }
    }
    private void ReadInput()
    {   //Set default input at start of game

        //Player (Default) Controls
        Controls.Default.Move.performed += ctx => Move(ctx);
        Controls.Default.Move.canceled += ctx => MoveCancelled(ctx);
        Controls.Default.Look.performed += ctx => Look(ctx);
        Controls.Default.Look.canceled += ctx => LookCancelled(ctx);
        Controls.Default.Jump.performed += ctx => Jump(ctx);
        Controls.Default.Jump.canceled += ctx => JumpCancelled(ctx);
        //Controls.Default.JumpHeld.performed += ctx => inputJump = false; //Stops continuous jumping
        Controls.Default.Sprint.performed += ctx => Sprint(ctx);
        Controls.Default.SprintToggle.performed += ctx => SprintToggle(ctx); //toggle sprint on/off for joysticks
        Controls.Default.Sprint.canceled += ctx => SprintCancelled(ctx);
        Controls.Default.Crouch.performed += ctx => CrouchInput(ctx);
        Controls.Default.Crouch.canceled += ctx => CrouchCancelled(ctx);
        Controls.Default.Prone.performed += ctx => Prone(ctx);
        Controls.Default.InstantProne.performed += ctx => InstantProne(ctx); //Allows remapping crouch on keyboard
        Controls.Default.Slide.performed += ctx => Slide(ctx); //allows remapping slide on keyboard
        Controls.Default.DoubleJump.performed += ctx => DoubleJump(ctx);
        Controls.Default.Item1.performed += ctx => Item1(ctx);
        Controls.Default.Item2.performed += ctx => Item2(ctx);
        Controls.Default.Item3.performed += ctx => Item3(ctx);
        Controls.Default.Item4.performed += ctx => Item4(ctx);
        Controls.Default.Select.performed += ctx => Select(ctx);
        Controls.Default.Start.performed += ctx => StartButton(ctx);
        Controls.Default.Message.performed += ctx => Message(ctx);
        Controls.Default.Fire.performed += ctx => Fire(ctx);
        Controls.Default.Fire.canceled += ctx => FireCancelled(ctx);
        Controls.Default.Aim.performed += ctx => Aim(ctx);
        Controls.Default.Aim.canceled += ctx => AimCanceled(ctx);
        Controls.Default.Lethal.performed += ctx => Lethal(ctx);
        Controls.Default.Tactical.performed += ctx => Tactical(ctx);
        Controls.Default.Reload.canceled += ctx => ReloadCanceled(ctx);
        Controls.Default.Action.performed += ctx => Action(ctx);
        //Controls.Default.Lethal.canceled += ctx => { if (isGrappling) AscendGrapple = false; };
        Controls.Default.Swap.performed += ctx => Swap(ctx);

        //Menu Controls
        Controls.Menu.Resume.performed += ctx => Resume(ctx);
        Controls.Menu.Cancel.canceled += ctx => Cancel(ctx);
        Controls.Menu.LeftTab.performed += ctx => LeftTab(ctx);
        Controls.Menu.RightTab.performed += ctx => RightTab(ctx);
        Controls.Menu.Message.performed += ctx => MessageMenu(ctx);
        Controls.Menu.Submit.performed += ctx => Submit(ctx);
        Controls.Menu.Navigate.performed += ctx => Navigate(ctx);
        Controls.Menu.Inventory.performed += ctx => Inventory(ctx);
    }

    #region Default controls
    private void Move(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();
        currentInput.xyMove = value;
    }
    private void MoveCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();
        currentInput.xyMove = value;
    }
    private void Look(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();
        currentInput.xyLook = value;

        if (playerInput.currentControlScheme != "Keyboard & Mouse") 
        { currentInput.xyLook *= 100f * controllerScaler; }
    }
    private void LookCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();
        currentInput.xyLook = Vector2.zero;
    }
    private void Jump(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        currentInput.standingPos = standingPosition.standing;
        jump?.Invoke();
    }
    private void JumpCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        //PS.inputJump = false;
        /*AllowCrouchJump = true;*/
    }
    private void DoubleJump(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        doubleJump?.Invoke();
    }
    private void Sprint(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        sprint?.Invoke();
    }
    private void SprintToggle(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        sprintToggle?.Invoke();
    }
    private void SprintCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        sprintCancelled?.Invoke();
    }
    private void CrouchInput(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        currentInput.standingPos = standingPosition.crouch;
        proneLatch = false;
    }
    private void CrouchCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        if(!proneLatch) changePosition?.Invoke();
    }
    private void Prone(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        currentInput.standingPos = standingPosition.prone;
        changePosition?.Invoke();

        proneLatch = true;
    }
    private void InstantProne(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        currentInput.standingPos = standingPosition.prone;
        changePosition?.Invoke();

        proneLatch = true;
    }
    private void Slide(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        currentInput.standingPos = standingPosition.crouch;
        slide?.Invoke();
    }
    private void Item1(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        item1?.Invoke();
    }
    private void Item2(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        item2?.Invoke();
    }
    private void Item3(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        item3?.Invoke();
    }
    private void Item4(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        item4?.Invoke();
    }
    private void Select(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        select?.Invoke();
    }
    private void StartButton(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        start?.Invoke();
    }
    private void Message(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        message?.Invoke();
    }
    private void Fire(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        fire?.Invoke();
    }
    private void FireCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        fireCancelled?.Invoke();
    }
    private void Aim(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        aim?.Invoke();
    }
    private void AimCanceled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        aimCancelled?.Invoke();
    }
    private void Lethal(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        lethal?.Invoke();        
    }
    private void Tactical(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        tactical?.Invoke();        
    }
    private void ReloadCanceled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        reload?.Invoke();
    }
    private void Action(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        action?.Invoke();
    }
    private void Swap(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        swap?.Invoke();
    }
    #endregion

    #region Menu controls
    private void Resume(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        resume?.Invoke();
    }
    private void Cancel(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        cancel?.Invoke();
    }
    private void LeftTab(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        leftTab?.Invoke();
    }
    private void RightTab(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        rightTab?.Invoke();
    }
    private void MessageMenu(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        messageMenu?.Invoke();
    }
    private void Submit(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        submit?.Invoke();
    }
    private void Navigate(InputAction.CallbackContext ctx)
    {
         var value = ctx.ReadValue<Vector2>();
        navigate?.Invoke();
    }
    private void Inventory(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();
        inventory?.Invoke();
    }
    #endregion
}

