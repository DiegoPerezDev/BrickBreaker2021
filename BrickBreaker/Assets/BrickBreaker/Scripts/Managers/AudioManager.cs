using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using MyTools;

public class AudioManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class manage all the audio in the game, both music and sfx, for all scenes.
    */


    // - - - - GENERAL AUDIO - - - -

    // General management
    public static AudioSource generalAudioSource;
    private static AudioManager instance;
    public static bool audioReady;

    // Music management
    private static AudioSource musicSource;
    private static AudioClip[] songsClips = new AudioClip[1];

    // UI audios
    [HideInInspector] public enum UiAudioNames { button, pause, unPause }
    public static AudioClip[] uiClips = new AudioClip[Enum.GetNames(typeof(UiAudioNames)).Length];


    // - - - - GAMEPLAY - - - -

    // Player audio
    [HideInInspector] public enum PlayerAudioNames { PlayerJump, PlayerDash, PlayerMeleeAttack, PlayerRangedAttack, PlayerWallSlide, PlayerWeaponChange, NovaHurt, NovaDeath }
    public static AudioClip[] playerClips = new AudioClip[Enum.GetNames(typeof(PlayerAudioNames)).Length];
    public static AudioSource playerMoveSource, playerVoiceSource, playerAttackSource;

    // Interactables audio
    [HideInInspector] public enum InteractablesAudioNames { OpenBox, ItemGather }
    public static AudioClip[] interactablesClips = new AudioClip[Enum.GetNames(typeof(PlayerAudioNames)).Length];

    // Enemy1 audio
    [HideInInspector] public enum Enemy1AudioNames { Enemy1Attack, Enemy1Hurt, Enemy1Death }
    public static AudioClip[] enemy1Clips = new AudioClip[Enum.GetNames(typeof(PlayerAudioNames)).Length];


    void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
        {
            //Singleton
            instance = this;

            // Get audio clips
            FindAllResources();

            // Get audio source
            GameObject AudioManagerGO = SearchTools.TryFindInGameobject(instance.transform.parent.gameObject, "AudioManager");
            generalAudioSource = SearchTools.TryGetComponent<AudioSource>(AudioManagerGO);
        }
    }


    // - - - - MAIN MANAGEMENT - - - -

    #region Start functions 

    private void FindAllResources()
    {
        FindGeneralResources();
        FindGameplayResources();
    }

    private void FindGeneralResources()
    {
        // UI sfx
        string[] uiClipsPaths = { "Button", "Pause", "UnPause" };
        foreach (UiAudioNames audioClip in Enum.GetValues(typeof(UiAudioNames)))
        {
            uiClips[(int)audioClip] = SearchTools.TryLoadResource("Assets/Resources/Audio/UI/" + uiClipsPaths[(int)audioClip]) as AudioClip;
        }

        // Music audio
        Array.Resize(ref songsClips, SceneManager.sceneCountInBuildSettings);
        musicSource = SearchTools.TryGetComponent<AudioSource>(this.gameObject);
        songsClips[0] = SearchTools.TryLoadResource("Assets/Resources/Audio/Music/MainMenu") as AudioClip;
        songsClips[2] = SearchTools.TryLoadResource("Assets/Resources/Audio/Music/Level") as AudioClip;
        if (musicSource == null)
        {
            print("music source not found in the AudioManager!");
            Debug.Break();
        }
    }

    private void FindGameplayResources()
    {
        string[] playerClipsPaths = { "PlayerJump", "PlayerDash", "PlayerMeleeAttack", "PlayerRangedAttack", "PlayerWallSlide", "PlayerWeaponChange", "NovaHurt", "NovaDeath" };
        string[] interactablesClipsPaths = { "OpenBox", "ItemGather" };
        string[] enemy1ClipsPaths = { "Enemy1Attack", "Enemy1Hurt", "Enemy1Death" };

        // Player audios
        foreach (PlayerAudioNames audioClip in Enum.GetValues(typeof(PlayerAudioNames)))
            playerClips[(int)audioClip] = Resources.Load("Audio/Characters/Player/" + playerClipsPaths[(int)audioClip]) as AudioClip;

        // Interactables audios
        foreach (InteractablesAudioNames audioClip in Enum.GetValues(typeof(InteractablesAudioNames)))
            interactablesClips[(int)audioClip] = Resources.Load("Audio/LevelDev/Interactables/" + interactablesClipsPaths[(int)audioClip]) as AudioClip;

        // Enemy1 audios
        foreach (Enemy1AudioNames audioClip in Enum.GetValues(typeof(Enemy1AudioNames)))
            enemy1Clips[(int)audioClip] = Resources.Load("Audio/Characters/Enemy1/" + enemy1ClipsPaths[(int)audioClip]) as AudioClip; ;
    }

    #endregion

    #region SFX and music methods

    /// <summary>
    /// Try to play a certain sfx in a specific audio source.
    /// </summary>
    /// <param name="source">Audio source in which the audio is going to come from.</param>
    /// <param name="clip">Which audio clip.</param>
    /// <param name="inLoop">Play it on loop or not.</param>
    public static void PlaySFX(AudioSource source, AudioClip clip, bool inLoop)
    {
        StopAudioSource(source);
        if (source != null)
        {
            if (inLoop)
            {
                //Skip the play if it's already playing while in loop
                if ((source.clip == clip) && source.isPlaying)
                    return;
                source.loop = true;
            }
            else
                source.loop = false;
            source.clip = clip;
            source.Play();
        }
        else
        {
            print("No audio source found");
        }
    }

    /// <summary>
    /// Stop any audio coming from an specific audio source.
    /// </summary>
    public static void StopAudioSource(AudioSource source)
    {
        if (source != null)
        {
            if (source.loop)
                source.loop = false;
            if (source.isPlaying)
                source.Stop();
        }
    }

    /// <summary>
    /// Stop an specific audio coming from an also specified audio source.
    /// </summary>
    public static void StopSFX(AudioSource source, AudioClip clip)
    {
        if (source != null)
        {
            if (source.clip == clip)
            {
                if (source.loop)
                    source.loop = false;
                if (source.isPlaying)
                    source.Stop();
            }
        }
    }

    /// <summary>
    /// Play the song of the level just entered in the music audio source.
    /// </summary>
    public static void PlayLevelSong(int actualScene)
    {
        //Look if we have the clip, if not then dont play the music
        if (!songsClips[actualScene])
        {
            print("song clip not found for scene index: " + actualScene);
            return;
        }

        // Select level song
        musicSource.clip = songsClips[actualScene];

        // Try to play the level song
        musicSource.Play();
    }

    /// <summary>
    /// Stop the music audio source.
    /// </summary>
    public static void StopLevelSong()
    {
        if (musicSource == null)
        {
            print("dont have musicSource here");
            return;
        }
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    #endregion


    // - - - - GAMEPLAY - - - -

    #region Start functions

    /// <summary>
    /// <para>Set all the components needed por the gameplay scenes, specifically audio sources.</para> 
    /// <para> This method should be called when making a transition to a gameplay scene from the GameManager class.</para> 
    /// </summary>
    public static void SetGameplay()
    {
        // Get player audio sources
        int count = 0;
        GameObject player = GameObject.Find("Characters/Player");
        if (player)
        {
            foreach (AudioSource audioSource in player.GetComponents<AudioSource>())
            {
                switch (count)
                {
                    case 0:
                        playerMoveSource = audioSource;
                        break;

                    case 1:
                        playerVoiceSource = audioSource;
                        break;

                    case 2:
                        playerAttackSource = audioSource;
                        break;
                }
                count++;
            }
        }

        // Tell the 'GameManager' that all the audio is already set
        audioReady = true;
    }
    
    #endregion

}