using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    /// Reads the player input from the inputManager and executes a corresponding function
    /// Simply add any variable or function to be triggered or updated into the functions

    //-----variables-----\\

    #region Input
    InputManager Controls;
    PlayerInput playerInput;
    public bool hideCursor = false;
    public bool isLocked = false;
    #endregion

    #region Scripts
    GameDataFile GDFile;
    MainMenuScript MMScript;
    CharacterMovementV2 PS;
    WeaponAndItemScript AS;
    InteractableObjects IOScript;
    #endregion

    //-----functions-----\\

    private void Awake()
    {
        //get scripts
        GDFile = FindObjectOfType<GameDataFile>();
        PS = GDFile.player.GetComponent<CharacterMovementV2>();
        AS = GDFile.player.GetComponent<WeaponAndItemScript>();
        MMScript = GDFile.MenuContainer.GetComponent<MainMenuScript>();
        IOScript = GDFile.player.GetComponent<InteractableObjects>();

        //get input
        Controls = new InputManager();
        playerInput = GetComponent<PlayerInput>();

        //assign Inputs
        ReadInput();
    }
    private void Update()
    {
        CursorLockAndUnlockState();
    }
    private void OnEnable() { Controls.Menu.Enable(); Controls.Default.Disable(); } //Enables user input 
    private void OnDisable() { Controls.Default.Disable(); Controls.Menu.Disable(); } //Disables user input
    public void CursorLockAndUnlockState()
    {   //Locks and hides the cursor if in game or not using keyboard & mouse 

        //hides cursor when using controller
        if (playerInput.currentControlScheme == "Keyboard & Mouse") { hideCursor = false; }
        else { hideCursor = true; }

        //checks if cursor should be visible when using mouse
        if (hideCursor || ((!GDFile.isPaused && !GDFile.tempPause) && GDFile.MenuContainer.GetComponent<MainMenuScript>().inGame))
        { if (!isLocked) { isLocked = true; Cursor.visible = false; } }

        //Frees cursor
        else if (isLocked) { isLocked = false; Cursor.visible = true; }
    }
    public void PauseCharacterInput()
    {
        if (GDFile.isPaused || GDFile.tempPause) { Controls.Default.Disable(); Controls.Menu.Enable(); playerInput.SwitchCurrentActionMap("Menu"); }
        else { Controls.Menu.Disable(); Controls.Default.Enable(); playerInput.SwitchCurrentActionMap("Default"); }
        //Debug.Log(playerInput.currentActionMap);
    }
    private void ReadInput()
    {   //Set default input at start of game

        //Player (Default) Controls
        Controls.Default.Move.performed += ctx => Move(ctx);
        //Controls.Default.MoveJoystick.performed += ctx => xyMoveJoystick = ctx.ReadValue<Vector2>();
        Controls.Default.Move.canceled += ctx => MoveCancelled(ctx);
        Controls.Default.Look.performed += ctx => Look(ctx);
        Controls.Default.Jump.performed += ctx => Jump(ctx);
        Controls.Default.Jump.canceled += ctx => JumpCancelled(ctx);
        //Controls.Default.JumpHeld.performed += ctx => inputJump = false; //Stops continuous jumping
        Controls.Default.Sprint.performed += ctx => Sprint(ctx);
        Controls.Default.SprintToggle.performed += ctx => SprintToggle(ctx); //toggle sprint on/off for joysticks
        Controls.Default.Sprint.canceled += ctx => SprintCancelled(ctx);
        Controls.Default.Crouch.performed += ctx => Crouch(ctx);
        Controls.Default.Crouch.canceled += ctx => CrouchCancelled(ctx);
        Controls.Default.Prone.performed += ctx => Prone(ctx);
        Controls.Default.InstantProne.performed += ctx => InstantProne(ctx); //Allows remapping crouch on keyboard
        Controls.Default.Slide.performed += ctx => Slide(ctx); //allows remapping slide on keyboard
        Controls.Default.DoubleJump.performed += ctx => DoubleJump(ctx);
        Controls.Default.Item1.performed += ctx => Item1(ctx);
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

        //playerScript
        PS.xyMove = value;
        //PS.useJoystick = false; 
    }
    private void MoveCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();

        //playerScript
        PS.xyMove = Vector2.zero;
    }
    private void Look(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();

        //playerScript
        PS.xyLook = ctx.ReadValue<Vector2>(); 
        if (playerInput.currentControlScheme != "Keyboard & Mouse") 
        { PS.xyLook *= Time.unscaledDeltaTime * 100f; }
    }
    private void Jump(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        PS.inputJump = true;
    }
    private void JumpCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        PS.inputJump = false;
        /*AllowCrouchJump = true;*/
    }
    private void DoubleJump(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        PS.doubleJump = true;
    }
    private void Sprint(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        if (PS.canStand) 
        { 
            PS.moveCamPos = PS.standingCamPos; 
            PS.inputSprint = true; 
        }
    }
    private void SprintToggle(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        if (PS.canStand) 
        { 
            PS.inputSprint = !PS.inputSprint; 
            if (PS.inputSprint) { PS.moveCamPos = PS.standingCamPos; } 
        }
    }
    private void SprintCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        PS.inputSprint = false;
    }
    private void Crouch(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        PS.inputCrouch = true; 
        PS.inCrouch = true; 
        PS.crouchLatch.x = 1f;
    }
    private void CrouchCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        PS.inputCrouch = false; 
        PS.crouchLatch.y = PS.crouchLatch.x; 
        PS.crouchLatch.x = 0f;
    }
    private void Prone(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        PS.inputCrouch = false; 
        PS.ProneSlideSwitch();
    }
    private void InstantProne(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        if (PS.inputSprint && PS.inJump) { PS.DolphinDive(); } 
        else if (!PS.inSlide && !PS.inputSprint) PS.Prone();
    }
    private void Slide(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        PS.inputCrouch = false; 
        if (PS.isGroundedJump && !PS.inCrouch && !PS.inProne && !PS.slideCoolDownBool && (PS.angle < PS.slideAngle || PS.rb.velocity.y <= 0f)) 
        { PS.Slide(); }
    }
    private void Item1(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        //PS.ScreenSize();
    }
    private void Item3(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //AttackScript
        AS.SwitchToGrapple();
    }
    private void Item4(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        if (!PS.AGcoolDownBool) { PS.AntiGravity(); }
    }
    private void Select(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //menu
        MMScript.OpenInventory();

        ////cursor
        //CursorLockAndUnlockState(); 
        //isLocked = !isLocked;
    }
    private void StartButton(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        if (GDFile.MenuContainer.GetComponent<MainMenuScript>().inGame) GDFile.PauseGame();
    }
    private void Message(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        if (GDFile.ActiveClient) GDFile.MenuContainer.GetComponent<MainMenuScript>().ShowMesssageGUI();
    }
    private void Fire(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //AttackScript
        if (!AS.isGrappling) AS.CheckWeapon();
    }
    private void FireCancelled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //AttackScript
        AS.fullAutoShoot = false; 
        if (AS.isGrappling) AS.StopGrapple();
    }
    private void Aim(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //AttackScript
        AS.Aim();
    }
    private void AimCanceled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //AttackScript
        AS.StopAim();
    }
    private void Lethal(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //AttackScript
        if (AS.isGrappling) 
        {
            //*ADD* make initial force smaller the closer you are to the point
            AS.rb.AddForce((AS.grapplePoint - AS.player.position) * AS.grappleSpeed * 0.15f, ForceMode.Impulse);
            AS.AscendGrapple = true; 
        }
        else AS.ThrowLethal();
    }
    private void ReloadCanceled(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //AttackScript
        AS.ReloadCheck();
        AS.buttonHeld = false;
    }
    private void Action(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //AttackScript
        AS.buttonHeld = true; 
        IOScript.Interact();
    }
    private void Swap(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //AttackScript
        AS.StopAim(); 
        AS.cancelReload = true; 
        if (!AS.allowSwap) AS.no_Swap++; 
        else StartCoroutine(AS.SwitchGun());
    }
    #endregion

    #region Menu controls
    private void Resume(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //menuScript
        if(MMScript.PauseMenuContainer.activeSelf) MMScript.ResumeGame();
    }
    private void Cancel(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        if (MMScript.InterfaceContainer.activeSelf) MMScript.CloseInterface();
        else MMScript.CancelBackSwitch();
    }
    private void LeftTab(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        MMScript.LeftTab();
    }
    private void RightTab(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        MMScript.RightTab();
    }
    private void MessageMenu(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        MMScript.ShowMesssageGUI();
    }
    private void Submit(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //playerScript
        MMScript.EditText = true;
        MMScript.SelectLast();
        if (MMScript.MessageContainer.activeSelf) { MMScript.EnterMessage(); }
    }
    private void Navigate(InputAction.CallbackContext ctx)
    {
         var value = ctx.ReadValue<Vector2>();

        //playerScript
        MMScript.OnInputField();
        MMScript.SelectOnNavigate();
        /*if (MMScript.inDropDown) 
          { MMScript.autoScrollDropDown(ctx.ReadValue<Vector2>());}*/
    }
    private void Inventory(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        //menu
        MMScript.CloseInterface();
    }
    #endregion
}

