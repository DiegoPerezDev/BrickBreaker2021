using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public sealed class InputsManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class manage all of the inputs of the user, reads them and sends the results to other codes by changing values of some other variables.
    - This input manager works with Unitys new input system that uses 'input actions'
    - This code separates the inputs between menu inputs and gameplay inputs so we can disable the gameplay inputs when the game is paused or in menu.
    */


    // - - - - MAIN MANAGEMENT - - - -

    // General stuff
    private static InputActionControlG input;
    private static InputsManager instance;
    public static bool InputsReady;

    // Menu inputs
    public static bool pause, returnToMenu, triggerMenuButton, smartPhone_BackButton, menuNavigationButtons;


    // - - - - GAMEPLAY - - - - 

    // Player actions
    private static Paddle paddleCode;
    public static bool leftMove, rightMove, releaseBall;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            input = new InputActionControlG();
        }
        else
            Destroy(this);
    }
    
    void Start()
    {
        SetInputMask();
        InputCallBacks();
    }

    void Update()
    {
        // Don't use any input on loading sceen
        if (GameManager.loadingScene || GameManager.settingScene)
            return;

        // Inputs for menu management, it's in all scenes
        MenuInputs();

        //Inputs for gameplay scenes
        if (UI_Manager.inMenu || !LevelManager.playing)
                return;

        MovementInputs();
        ReleaseBallInput();
    }

    void OnEnable()
    {
        if(input != null)
            input.ActionMap.Enable();
        // Never disable the input action map because this script is non destroyable, always present in all scenes
    }


    // - - - - MAIN MANAGEMENT - - - -

    #region Inputs management functions

    // Change the input device to an specific one, ignoring other posible inputs. 
    private static void SetInputMask()
    {
        #if UNITY_STANDALONE
                input.bindingMask = InputBinding.MaskByGroup("pc");
        #endif

        #if UNITY_ANDROID
                input.bindingMask = InputBinding.MaskByGroup("smartphone");
        #endif
    }

    /// <summary>
    /// This function set the callbacks for the inputs. Read the input actions and store the results in certain variables for afterward management.
    /// </summary>
    private static void InputCallBacks()
    {

#if UNITY_STANDALONE
        //Read menu inputs
        input.ActionMap.Pause.performed += ctx => pause = ctx.ReadValueAsButton();
        input.ActionMap.Menu_GoBack.performed += ctx => returnToMenu = ctx.ReadValueAsButton();
        input.ActionMap.MenuNavigationButtons.performed += ctx => menuNavigationButtons = ctx.ReadValueAsButton();

        //Read gameplay inputs
        input.ActionMap.LeftMove.performed += ctx => leftMove = ctx.ReadValueAsButton();
        input.ActionMap.RightMove.performed += ctx => rightMove = ctx.ReadValueAsButton();
        input.ActionMap.ReleaseBall.performed += ctx => releaseBall = ctx.ReadValueAsButton();
#endif

#if UNITY_ANDROID
        //Read gameplay inputs
        input.ActionMap.TouchPress.started += ctx => StartScreenTouched();
            input.ActionMap.TouchPress.canceled += ctx => EndScreenTouched();
        //Read menu inputs
        input.ActionMap.Back.performed += ctx => smartPhone_BackButton = ctx.ReadValueAsButton();
#endif
    }

    /// <summary>
    /// <para>Set false all the trigger variables, those variables that stores the values read from the input actions.</para>
    /// <para>For evading unwanted actions because the triggers could save an active input when disabling the inputs.</para>
    /// </summary>
    public static void DisableTriggers()
    {
        // - MAIN MANAGEMENT - 
        pause = returnToMenu = triggerMenuButton = menuNavigationButtons = smartPhone_BackButton = false;

        // - GAMEPLAY -
        leftMove = rightMove = false;
        releaseBall = false;
    }

    #endregion

    // - - - - MENU - - - -

    #region Menu

    private void MenuInputs()
    {
        // Pause
        if (pause)
        {
            pause = false;
            if (!MainMenu_Manager.inMainMenu)
            {
                returnToMenu = false;
                LevelManager.pauseTrigger = true;
            }
        }
        // Return to previous menu
        if (returnToMenu)
        {
            returnToMenu = pause = false;
            UI_Manager.returnTrigger = true;
        }
        // Button trigger for gamepad
        else if (triggerMenuButton)
        {
            triggerMenuButton = false;
            // Press selected button.
            Button currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (currentButton != null)
                currentButton.onClick.Invoke();
        }
        else if (menuNavigationButtons)
        {
            menuNavigationButtons = false;
            if(UI_Manager.inMenu)
                UI_Navigation.SelectFirstButton();
        }
        else if(smartPhone_BackButton)
        {
            smartPhone_BackButton = false;
            if(!LevelManager.paused)
                LevelManager.PauseMenu(true, false);
        }
    }

    #endregion

    // - - - - GAMEPLAY - - - -

    #region Gameplay Start functions

    /// <summary>
    /// Prepare everything that this codes needs to manage all that involves the gameplay. Completely depends on what happens in the gameplay.
    /// </summary>
    public static void SetGameplay() => instance.StartCoroutine(GameplaySettingCor());

    private static IEnumerator GameplaySettingCor()
    {
        GameObject paddle = null;
        while (paddle == null)
        {
            paddle = GameObject.Find(Paddle.paddlePath);
            yield return new WaitForSecondsRealtime(0.2f);
        }
        paddleCode = paddle.GetComponent<Paddle>();

        DisableTriggers();
        InputsReady = true;
    }

    #endregion

    #region Smartphone functions

    private static void StartScreenTouched()
    {
        // Set the paddle movement and the ball release inputs for each case
        Vector2 touchPos = input.ActionMap.TouchPosition.ReadValue<Vector2>();
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        if(!rightMove)
        {
            if ( (touchPos.x > screenWidth * 3f / 5f) && (touchPos.y < (screenHeight / 2f)) )
            {
                rightMove = true;
                return;
            }
        }
        if(!leftMove)
        {
            if ((touchPos.x < screenWidth * 2 / 5f) && (touchPos.y < (screenHeight  / 2f)) )
            {
                leftMove = true;
                return;
            }
        }
    }

    private static void EndScreenTouched()
    {
        // Disable movement triggers
        leftMove = rightMove = false;
    }

    #endregion

    #region Gameplay inputs management

    private void MovementInputs()
    {
        //Horizontal movement
        if (rightMove)
        {
            paddleCode.moveDirR = true;
            paddleCode.moveBool = true;
        }
        else if (leftMove)
        {
            paddleCode.moveDirR = false;
            paddleCode.moveBool = true;
        }
        else if (paddleCode.moveBool)
        {
            paddleCode.moveDirR = null;
            paddleCode.moveBool = false;
        }
    }

    private void ReleaseBallInput()
    {
        // Ball release when its lock on the paddle
        if (releaseBall)
        {
            // Reset input values so it wont constantly repeat the action
            releaseBall = false;

            // Release the ball if it hasn't been released
            if (!Ball.ballReleased)
            {
                Ball.ballReleased = true;

                // desable launch button
                Gameplay_UI.launchButton.interactable = false;

                // release ball
                GameObject ballGO = GameObject.Find(Ball.ballPath);
                Ball ball = ballGO.GetComponent<Ball>();
                ball.ReleaseBall();
            }
        }
    }

    #endregion

}