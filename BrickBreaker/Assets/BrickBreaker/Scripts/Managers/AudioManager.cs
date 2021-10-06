using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using MyTools;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class manage all the audio in the game, both music and sfx, for all scenes.
    - Any code from any scenetype can call the mothods of this class.
    */


    // - - - - GENERAL AUDIO - - - -

    // General management
    public static AudioSource GameAudioSource;
    private static AudioManager instance;

    // Music management
    private static AudioSource musicSource;
    private static AudioClip[] songsClips = new AudioClip[1];

    // SFX Managerment
    private static AudioMixerGroup sfxMixer;


    void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
        {
            instance = this;
            FindResources();
        }
    }


    // - - - - MAIN MANAGEMENT - - - -

    #region Start functions 

    private void FindResources()
    {
        // Get audio mixers
        sfxMixer = SearchTools.TryLoadResource("Audio/SFX_Mixer") as AudioMixerGroup;

        // Get audio sources
        var sfxChild = instance.gameObject.transform.GetChild(0);
        GameAudioSource = SearchTools.TryGetComponent<AudioSource>(sfxChild.gameObject);
        var musicChild = instance.gameObject.transform.GetChild(1);
        musicSource = SearchTools.TryGetComponent<AudioSource>(musicChild.gameObject);

        // Music clips
        Array.Resize(ref songsClips, SceneManager.sceneCountInBuildSettings);
        songsClips[0] = SearchTools.TryLoadResource("Audio/Music/(m2) main menu music") as AudioClip;
        songsClips[2] = SearchTools.TryLoadResource("Audio/Music/(m1) arcade_music_loop") as AudioClip;
        if (songsClips.Length >= 3)
        {
            for (int i = 3; i < songsClips.Length; i++)
                songsClips[i] = songsClips[2];
        }
    }

    #endregion

    #region SFX methods

    /// <summary>
    /// <para>Try to play a certain sfx in a specific audio source.</para> 
    /// <para>If there is another clip playing in one of this sources then go to the next audio source to play the next clip, never stop the other when using this method.</para> 
    /// </summary>
    public static void PlayAudio_WithoutInterruption(ref AudioSource[] sources, AudioClip clip, GameObject objectOfSources, bool inLoop, float volume)
    {
        AudioSource source = null;

        var tempLength = sources.Length;

        for (int i = 0; i < tempLength; i++)
        {
            // Check if that audio source exists
            if (sources[i] == null)
            {
                print($"No audio sources found with name {nameof(sources)}");
                return;
            }

            if (i == tempLength - 1)
            {
                //make another source if all the sources are playing a sfx
                if (sources[i].isPlaying)
                {
                    Array.Resize(ref sources, sources.Length + 1);
                    sources[i + 1] = objectOfSources.AddComponent<AudioSource>();
                    sources[i + 1].outputAudioMixerGroup = sfxMixer;
                    source = sources[i + 1];
                    goto Play;
                }
            }
            else
            {
                // Dont play the new sfx on an audiosource that is already playing
                if (sources[i].isPlaying)
                    continue;
            }

            source = sources[i];
        }

        Play:
        PlayAudio(source, clip, inLoop, volume);
    }

    /// <summary>
    /// <para>Try to play a certain sfx in a specific audio source.</para> 
    /// <para>If there is another clip playing in this source then stop it and change it to the new one to play, if you don't want this but have multiple sfx of the same kind then use the overload that uses 'audioSource[]' instead.</para>
    /// </summary>
    /// <param name="source">Audio source in which the audio is going to come from.</param>
    /// <param name="clip">Which audio clip.</param>
    public static void PlayAudio(AudioSource source, AudioClip clip, bool inLoop, float volume)
    {
        // Error handler
        if (source == null)
        {
            print($"No audio source found with name {nameof(source)}");
            return;
        }

        // Volume
        if( (volume >= 0) && (volume <= 1) )
            source.volume = volume;

        // Loop
        if ((inLoop == true) && (source.clip == clip) && source.isPlaying)
            return;
        source.loop = inLoop;

        // Play audio
        if (source.isPlaying)
            StopAudio(source);
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// Stop any audio coming from an specific audio source.
    /// </summary>
    public static void StopAudio(AudioSource source)
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
    public static void StopAudio(AudioSource source, AudioClip clip)
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

    #endregion

    #region Music methods

    /// <summary>
    /// Play the song of the level just entered in the music audio source.
    /// </summary>
    public static void PlayLevelSong(int actualScene)
    {
        //Look if we have the clip, if not then dont play the music
        if (!songsClips[actualScene] && (actualScene != 1))
        {
            print("song clip not found for scene index: " + actualScene);
            return;
        }

        // Set desired volume for each music
        if (actualScene == 0)
            musicSource.volume = 1f;
        else if (actualScene > 1)
            musicSource.volume = 0.2f;

        // Select level song
        musicSource.clip = songsClips[actualScene];

        // Try to play the level song
        musicSource.Play();
    }

    /// <summary>
    /// Stop the audio sources of this gameObject, remember those are NOT all the audio sources but the main ones.
    /// </summary>
    public static void StopAllAudio()
    {
        StopAllGeneralSFX();
        StopLevelSong();
    }

    /// <summary>
    /// Stop the general audio source.
    /// </summary>
    public static void StopAllGeneralSFX()
    {
        if (GameAudioSource == null)
            return;
        if (GameAudioSource.isPlaying)
            GameAudioSource.Stop();
    }

    /// <summary>
    /// Stop the music audio source.
    /// </summary>
    public static void StopLevelSong()
    {
        if (musicSource == null)
            return;
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    #endregion

}