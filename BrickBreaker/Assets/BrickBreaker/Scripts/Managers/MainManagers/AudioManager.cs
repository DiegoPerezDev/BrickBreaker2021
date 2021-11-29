using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public sealed class AudioManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class manage all the audio in the game, for both music and sfx, for all scenes.
    - All code should use audio by using the methods of this class.
    */

    // General management
    public static AudioSource GameAudioSource;
    private static AudioManager instance;

    // Music management
    private static AudioSource musicSource;
    private static AudioClip[] songsClips = new AudioClip[1];

    // SFX Managerment
    public static AudioMixerGroup sfxMixerGroup;
    public static AudioMixer musicMixer, sfxMixer;


    void Awake()
    {
        // Set only one code instance.
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        else
            instance = this;

        // Find components
        musicMixer = Resources.Load<AudioMixer>("Audio/MusicMixer");
        sfxMixer = Resources.Load<AudioMixer>("Audio/SFX_Mixer");
        sfxMixerGroup = Resources.Load<AudioMixerGroup>("Audio/SFX_Mixer");
        GameAudioSource = instance.gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        musicSource = instance.gameObject.transform.GetChild(1).GetComponent<AudioSource>();

        // Find resources
        Array.Resize(ref songsClips, SceneManager.sceneCountInBuildSettings);
        songsClips[0] = Resources.Load<AudioClip>("Audio/Music/(m2) main menu music");
        songsClips[1] = null;
        songsClips[2] = Resources.Load<AudioClip>("Audio/Music/(m1) arcade_music_loop");
        if (songsClips.Length >= 3)
        {
            for (int i = 3; i < songsClips.Length; i++)
                songsClips[i] = songsClips[2];
        }
    }


#region SFX methods

    /// <summary>
    /// <para>Try to play a certain sfx in a specific audio source array.</para> 
    /// <para>If there is another clip playing in one of this sources then go to the next source of the array to play the next clip.</para> 
    /// <para>If all sources are playing then add a new one to the gameobject so it never interrupt a source that is playing.</para> 
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
                    sources[i + 1].outputAudioMixerGroup = sfxMixerGroup;
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
    /// <para>If there is another clip playing in this source then stop it and change it to the new one to play.</para>
    /// <para>If you don't want to interrupt any sfx then use the overload of this method that uses 'audioSource[]' instead.</para>
    /// </summary>
    public static void PlayAudio(AudioSource source, AudioClip clip, bool inLoop, float volume)
    {
        // Error handler
        if (source == null)
        {
            print($"No audio source found with name {nameof(source)}");
            return;
        }

        // Volume
        if( (volume > 0) && (volume <= 1) )
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
    /// <para> Play the song of the level just entered in the music audio source. </para>
    /// <para> This method should be used by a code like a 'gameManager' or 'transitionManager'. </para>
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
            musicSource.volume = 0.9f;
        else if (actualScene > 1)
            musicSource.volume = 0.3f;

        // Select level song
        musicSource.clip = songsClips[actualScene];

        // Try to play the level song
        musicSource.Play();
    }

    /// <summary>
    /// <para> Stop the audio sources of this gameObject, these are the general sfxs like the menu sfx and also the music. </para>
    /// <para> Remember those are NOT all the audio sources but the main ones. </para>
    /// </summary>
    public static void StopAllAudio()
    {
        StopLevelSong();
        StopAllGeneralSFX();
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

    /// <summary>
    /// Stop the general audio source.
    /// </summary>
    private static void StopAllGeneralSFX()
    {
        if (GameAudioSource == null)
            return;
        if (GameAudioSource.isPlaying)
            GameAudioSource.Stop();
    }

    #endregion

    #region volume set

    public static void SetMusicVolume(float volume) => musicMixer.SetFloat("volume", volume);
    public static void SetSFX_Volume(float volume) => sfxMixer.SetFloat("volume", volume);

    #endregion

}