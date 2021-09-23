using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MyTools;
using System.Collections;

public class InputsManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class manage all of the inputs of the user, reads them and sends the results to other codes by changing values of some otheir variables.
    - This input manager works with Unitys new input system that uses 'input actions'
    - This code separates the inputs between menu inputs and gameplay inputs so we can disable the gameplay inputs when the game is paused or in menu.
    */


    // - - - - MAIN MANAGEMENT - - - -

    // General stuff
    private static InputActionControlG input;
    private static InputsManager instance;
    public static bool InputsReady;

    // Input device change
    [HideInInspector] public enum InputDevices { pc, smartphone, none }
    public static InputDevices currentInputDevice;
    public readonly static string[] inputDevicesNames = { "pc", "smartphone" };

    // Menu inputs
    public static bool pause, returnToMenu, triggerMenuButton;


    // - - - - GAMEPLAY - - - - 

    // General use
    public static bool gameplayInputsDisabled;

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
        gameplayInputsDisabled = true;
        InputCallBacks();
    }

    void Update()
    {
        // Don't use any input on loading sceen
        if (GameManager.loadingScene)
            return;

        // Inputs for menu management, it's in all scenes
        MenuInputs();

        // Inputs for gameplay scenes
        if (gameplayInputsDisabled || !GameManager.inGameplay)
            return;
        if (GameManager.currentSceneType == GameManager.SceneType.gameplay)
        {
            MovementInputs();
            ActionInputs();
        }
    }

    void OnEnable()
    {
        if(input != null)
            input.ActionMap.Enable();
        // Never disable the input action map because this script is non destroyable, always present in all scenes
    }


    // - - - - MAIN MANAGEMENT - - - -

    #region Start functions

    /// <summary>
    /// This function set the callbacks for the inputs. Read the input actions and store the results in certain variables for afterward management.
    /// </summary>
    private void InputCallBacks()
    {
        //Read menu inputs
        input.ActionMap.Pause.performed += ctx => pause = ctx.ReadValueAsButton();
        input.ActionMap.Menu_GoBack.performed += ctx => returnToMenu = ctx.ReadValueAsButton();

        //Read gameplay inputs
        input.ActionMap.LeftMove.performed += ctx => leftMove = ctx.ReadValueAsButton();
        input.ActionMap.RightMove.performed += ctx => rightMove = ctx.ReadValueAsButton();
        input.ActionMap.ReleaseBall.performed += ctx => releaseBall = ctx.ReadValueAsButton();
    }

    #endregion

    #region Inputs management functions

    /// <summary>
    /// Manage what happens with the triggers related to the menu management.
    /// </summary>
    private void MenuInputs()
    {
        // Pause
        if (pause)
        {
            pause = false;
            GameManager.pauseTrigger = true;
        }
        // Return to previous menu
        else if (returnToMenu)
        {
            returnToMenu = false;
            GameManager.returnTrigger = true;
        }
        // Button trigger for gamepad
        else if (triggerMenuButton)
        {
            triggerMenuButton = false;
            if (currentInputDevice != InputDevices.pc)
            {
                Button currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                if (currentButton != null)
                    currentButton.onClick.Invoke();
            }
        }
    }

    /// <summary>
    /// <para>Set false all the trigger variables, those variables that stores the values read from the input actions.</para>
    /// <para>For evading unwanted actions because the triggers could save an active input when disabling the inputs.</para>
    /// </summary>
    public static void DisableTriggers()
    {
        // - MAIN MANAGEMENT - 
        pause = returnToMenu = triggerMenuButton = false;

        // - GAMEPLAY -
        leftMove = rightMove = false;
        releaseBall = false;
    }

    #endregion

    #region Input device change functions
    /*
    private static void ChangeInputDevice()
    {
        if (currentInputDevice == InputDevices.pc)
            ChangeInputDevice(InputDevices.smartphone);
        else if (currentInputDevice == InputDevices.smartphone)
            ChangeInputDevice(InputDevices.pc);
    }

    // Change the input device to an specific one. 
    private static void ChangeInputDevice(InputDevices inputDevice)
    {
        string controlScheme = inputDevicesNames[(int)inputDevice];

        if (inputDevice == InputDevices.pc)
        {
            if (currentInputDevice != InputDevices.pc)
            {
                currentInputDevice = InputDevices.pc;
                input.bindingMask = new InputBinding { groups = controlScheme };
                //print("using keyboard now");
            }
        }
        else if (inputDevice == InputDevices.smartphone)
        {
            if (currentInputDevice != InputDevices.smartphone)
            {
                currentInputDevice = InputDevices.smartphone;
                input.bindingMask = new InputBinding { groups = controlScheme };
                //print("using gamepad now");
            }
        }
        else
        {
            print("didn't get a propper input device name or the convertion was wrong.");
        }
    }
    */
    #endregion


    // - - - - GAMEPLAY - - - -

    #region Start functions

    /// <summary>
    /// Prepare everything that this codes needs to manage all that involves the gameplay. Completely depends on what happens in the gameplay.
    /// </summary>
    public static void SetGameplay()
    {
        gameplayInputsDisabled = false;
        DisableTriggers();
        instance.StartCoroutine(SettingCoroutine());
    }

    private static IEnumerator SettingCoroutine()
    {
        GameObject paddle = null;
        while (paddle == null)
        {
            paddle = GameObject.Find("LevelDev/Paddle_(Clone)");
            yield return new WaitForSecondsRealtime(0.1f);
        }

        paddleCode = paddle.GetComponent<Paddle>();
        InputsReady = true;
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

    private void ActionInputs()
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
                GameObject ballGO = SearchTools.TryFind("Ball_(Clone)");
                Ball ball = SearchTools.TryGetComponent<Ball>(ballGO);
                if (ball)
                    ball.ReleaseBall();
            }
        }
    }

    #endregion

}