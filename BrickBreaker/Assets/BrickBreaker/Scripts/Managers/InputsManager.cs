using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MyTools;
using System.Collections;
using UnityEngine.InputSystem;

public class InputsManager : MonoBehaviour
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
        SetInputMask();
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
        if (gameplayInputsDisabled || !(GameManager.currentSceneType == GameManager.SceneType.gameplay) )
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

    #region Inputs management functions

    /// <summary>
    /// This function set the callbacks for the inputs. Read the input actions and store the results in certain variables for afterward management.
    /// </summary>
    private static void InputCallBacks()
    {
        //Read menu inputs
        input.ActionMap.Pause.performed += ctx => pause = ctx.ReadValueAsButton();
        input.ActionMap.Menu_GoBack.performed += ctx => returnToMenu = ctx.ReadValueAsButton();

#if UNITY_STANDALONE
        //Read gameplay inputs
        input.ActionMap.LeftMove.performed += ctx => leftMove = ctx.ReadValueAsButton();
        input.ActionMap.RightMove.performed += ctx => rightMove = ctx.ReadValueAsButton();
        input.ActionMap.ReleaseBall.performed += ctx => releaseBall = ctx.ReadValueAsButton();
#endif

#if UNITY_ANDROID
        //Read gameplay inputs
        input.ActionMap.TouchPress.started += ctx => StartScreenTouched();
        input.ActionMap.TouchPress.canceled += ctx => EndScreenTouched();
#endif
    }

    // Change the input device to an specific one. 
    private static void SetInputMask()
    {
        #if UNITY_STANDALONE
                input.bindingMask = InputBinding.MaskByGroup("pc");
        #endif

        #if UNITY_ANDROID
                input.bindingMask = InputBinding.MaskByGroup("android");
        #endif

        //input.bindingMask = new InputBinding { groups = controlScheme };
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

    // - - - - MENU - - - -

    #region Menu

    /// <summary>
    /// Manage what happens with the triggers related to the menu management.
    /// </summary>
    private void MenuInputs()
    {
        // Pause
        if (pause)
        {
            pause = false;
            LevelManager.pauseTrigger = true;
        }
        // Return to previous menu
        else if (returnToMenu)
        {
            returnToMenu = false;
            UI_Manager.returnTrigger = true;
        }
        // Button trigger for gamepad
        else if (triggerMenuButton)
        {
            triggerMenuButton = false;
#if UNITY_STANDALONE
            Button currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (currentButton != null)
                currentButton.onClick.Invoke();
#endif
        }
    }

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
            paddle = GameObject.Find(Paddle.paddlePath);
            yield return new WaitForSecondsRealtime(0.1f);
        }

        paddleCode = paddle.GetComponent<Paddle>();
        InputsReady = true;
    }

    #endregion

    #region android functions

#if UNITY_ANDROID

    private static void StartScreenTouched()
    {
        //print("start touching the screen");

        // Set the paddle movement and the ball release inputs for each case
        Vector2 touchPos = input.ActionMap.TouchPosition.ReadValue<Vector2>();
        var screenWidth = Screen.width;

        if(!rightMove)
        {
            if ((touchPos.x > screenWidth * 2f / 3f))
            {
                rightMove = true;
                return;
            }
        }
        if(!leftMove)
        {
            if ((touchPos.x < screenWidth / 3f) && (touchPos.x > 0))
            {
                leftMove = true;
                return;
            }
        }
        if(!releaseBall)
        {
            releaseBall = true;
        }
    }

    private static void EndScreenTouched()
    {
        //print("stopped touching the screen");
        
        // Disable movement triggers
        leftMove = rightMove = false;
    }

#endif

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
                GameObject ballGO = SearchTools.TryFind(Ball.ballName);
                Ball ball = SearchTools.TryGetComponent<Ball>(ballGO);
                if (ball)
                    ball.ReleaseBall();
            }
        }
    }

#endregion

}