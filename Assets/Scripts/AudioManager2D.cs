using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//plans for the future: 
//create a dictionary driven option

[RequireComponent(typeof(AudioSource))]
public class AudioManager2D: MonoBehaviour
{
    public static AudioManager2D audioManager2DInstance;

    //there are 4 seperate audio streams here, make sure they are assigned
    [Header("audio sources")] 
    [SerializeField] private AudioSource soundAudioSrc;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioSource ambientAudioSource;
    [SerializeField] float masterVolume = 1;
    [SerializeField] bool changeOtherSources = false;
    [SerializeField] bool playOnStart = false;
    [SerializeField] string startingAudio = "";


    [Header("extra effects config")]
    [SerializeField] float fadeAudioEffectSpeed = 1;

    [Header("audio to play")]
    [SerializeField] Audio[] audioArray;

    
    //the index for the audio array to track what is playing
    //-1 means nothing is playing in that
    private int currentSoundIndex = -1;
    private int currentBGMIndex = -1;
    private int currentUISoundIndex = -1;
    private int currentAmbientIndex = -1;


    //singleton pattern
    private void Awake()
    {
        if (audioManager2DInstance == null)
        {
            audioManager2DInstance = this;
        }

        if (audioManager2DInstance != this)
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        if (playOnStart) {
            Play(startingAudio);
        }   
    }


    //called when changed volume
    public void SetVolume(float vol)
    {

        this.masterVolume = vol / 100;
        UpdateAudioSrcVolume(currentSoundIndex, Audio.AudioType.Sound);
        UpdateAudioSrcVolume(currentBGMIndex, Audio.AudioType.BGM);
        UpdateAudioSrcVolume(currentUISoundIndex, Audio.AudioType.UISounds);
        UpdateAudioSrcVolume(currentAmbientIndex, Audio.AudioType.Ambient);

        //this also needs to change audio in audio sources outside
        //not the best method
        if (!changeOtherSources) return;
        var audioSrcList = FindObjectsOfType<AudioSource>();
        foreach (var audioSrc in audioSrcList)
        {
            if (audioSrc != soundAudioSrc && audioSrc != bgmAudioSource && audioSrc != uiAudioSource && audioSrc != ambientAudioSource  )
                audioSrc.volume = masterVolume;
        }
    }


    private void UpdateAudioSrcVolume(int audioIndex, Audio.AudioType type)
    {
       
        if (audioIndex >= 0)
        {
            switch (audioArray[audioIndex].audioType)
            {
                case Audio.AudioType.Sound:
                    audioArray[audioIndex].updateVolume(soundAudioSrc, this.masterVolume);
                    break;
                case Audio.AudioType.BGM:
                    audioArray[audioIndex].updateVolume(bgmAudioSource, this.masterVolume);
                    break;
                case Audio.AudioType.UISounds:
                    audioArray[audioIndex].updateVolume(uiAudioSource, this.masterVolume);
                    break;
                case Audio.AudioType.Ambient:
                    audioArray[audioIndex].updateVolume(ambientAudioSource, this.masterVolume);
                    break;
                default:
                    Debug.LogError($"the audio clip {audioArray[audioIndex].audioName} does not have a audio type assigned \n or there is no update volume option for that type yet");
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case Audio.AudioType.Sound:
                    soundAudioSrc.volume = masterVolume;
                    break;
                case Audio.AudioType.BGM:
                    bgmAudioSource.volume = masterVolume;
                    break;
                case Audio.AudioType.UISounds:
                    uiAudioSource.volume = masterVolume;
                    break;
                case Audio.AudioType.Ambient:
                    ambientAudioSource.volume = masterVolume;
                    break;
                default:
                    Debug.LogError($"the audio clip {audioArray[audioIndex].audioName} does not have a audio type assigned \n or there is no update volume option for that type yet");
                    break;
            }
        }
    }

    //force all audio to stop
    public void StopAllAudio()
    {
        //this is the solution to manage fading for now
        StopCoroutine("FadeAudioEffect");
        soundAudioSrc.Stop();
        bgmAudioSource.Stop();
        uiAudioSource.Stop();
        ambientAudioSource.Stop();
    }

    public void StopSound() {
        soundAudioSrc.Stop();
    }
    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }
    public void StopUISound()
    {
        uiAudioSource.Stop();
    }
    public void StopAmbient()
    {
        ambientAudioSource.Stop();
    }

    public void Stop(int audioTypeIndex)
    {
        if(audioTypeIndex == (int)Audio.AudioType.Sound)
        {
            soundAudioSrc.Stop();
        }
        else if (audioTypeIndex == (int)Audio.AudioType.BGM)
        {
            bgmAudioSource.Stop();
        }
        else if (audioTypeIndex == (int)Audio.AudioType.UISounds)
        {
            uiAudioSource.Stop();
        }
        else if (audioTypeIndex == (int)Audio.AudioType.Ambient)
        {
            ambientAudioSource.Stop();
        }
        else
        {
            Debug.LogError($"the audio index {audioTypeIndex} cis not a valid audio type\nPlease use 1 - 4 ");
        }
    }

    public void SetFadeEffectSpeed(float newEffectSpeed) {
        fadeAudioEffectSpeed = newEffectSpeed;
    }

    public void PlayFade(string audioName) {

        for (int i = 0; i < audioArray.Length; i++)
        {
            //Debug.Log(audioName + " " + audioArray[i].audioName);
            if (audioArray[i].audioName.ToUpper() == audioName.ToUpper())
            {

                switch (audioArray[i].audioType)
                {
                    case Audio.AudioType.Sound:
                        
                        StartCoroutine(FadeAudioEffect(soundAudioSrc, fadeAudioEffectSpeed, audioArray[i].defaultVolume * masterVolume));
                        audioArray[i].Play(soundAudioSrc, masterVolume);
                        currentSoundIndex = i;
                        break;
                    case Audio.AudioType.BGM:
                        
                        StartCoroutine(FadeAudioEffect(bgmAudioSource, fadeAudioEffectSpeed, audioArray[i].defaultVolume * masterVolume));
                        audioArray[i].Play(bgmAudioSource, masterVolume);
                        currentBGMIndex = i;
                        break;
                    case Audio.AudioType.UISounds:
                        
                        StartCoroutine(FadeAudioEffect(uiAudioSource, fadeAudioEffectSpeed, audioArray[i].defaultVolume * masterVolume));
                        audioArray[i].Play(uiAudioSource, masterVolume);
                        currentUISoundIndex = i;
                        break;
                    case Audio.AudioType.Ambient:
                        
                        StartCoroutine(FadeAudioEffect(ambientAudioSource, fadeAudioEffectSpeed, audioArray[i].defaultVolume * masterVolume));
                        audioArray[i].Play(ambientAudioSource, masterVolume);
                        currentAmbientIndex = i;
                        break;
                    default:
                        Debug.LogError($"the audio clip {audioArray[i].audioName} does not have a audio type assigned \n or there is no play option for that type yet");
                        break;
                }
                return;
            }

            
        }
        Debug.LogError($"the audio clip {audioName} cannot be found\n please check that your spelling is correct");

    }

    public void PlayFade(string audioName, float fadeAudioEffectSpeed)
    {

        for (int i = 0; i < audioArray.Length; i++)
        {
            //Debug.Log(audioName + " " + audioArray[i].audioName);
            if (audioArray[i].audioName.ToUpper() == audioName.ToUpper())
            {

                switch (audioArray[i].audioType)
                {
                    case Audio.AudioType.Sound:
                        StopCoroutine("FadeAudioEffect");
                        StartCoroutine(FadeAudioEffect(soundAudioSrc, fadeAudioEffectSpeed, audioArray[i].defaultVolume * masterVolume));
                        audioArray[i].Play(soundAudioSrc, masterVolume);
                        currentSoundIndex = i;
                        break;
                    case Audio.AudioType.BGM:
                        StopCoroutine("FadeAudioEffect");
                        StartCoroutine(FadeAudioEffect(bgmAudioSource, fadeAudioEffectSpeed, audioArray[i].defaultVolume * masterVolume));
                        audioArray[i].Play(bgmAudioSource, masterVolume);
                        currentBGMIndex = i;
                        break;
                    case Audio.AudioType.UISounds:
                        StopCoroutine("FadeAudioEffect");
                        StartCoroutine(FadeAudioEffect(uiAudioSource, fadeAudioEffectSpeed, audioArray[i].defaultVolume * masterVolume));
                        audioArray[i].Play(uiAudioSource, masterVolume);
                        currentUISoundIndex = i;
                        break;
                    case Audio.AudioType.Ambient:
                        StopCoroutine("FadeAudioEffect");
                        StartCoroutine(FadeAudioEffect(ambientAudioSource, fadeAudioEffectSpeed, audioArray[i].defaultVolume * masterVolume));
                        audioArray[i].Play(ambientAudioSource, masterVolume);
                        currentAmbientIndex = i;
                        break;
                    default:
                        Debug.LogError($"the audio clip {audioArray[i].audioName} does not have a audio type assigned \n or there is no play option for that type yet");
                        break;
                }
                return;
            }

            
        }
        Debug.LogError($"the audio clip {audioName} cannot be found\n please check that your spelling is correct");

    }

    //0 = sound, 1 is BGM, 2 is UI, 3 is ambient
    public void StopFade(int audioTypeIndex)
    {

        if (audioTypeIndex == (int)Audio.AudioType.Sound)
        {
            StartCoroutine(FadeAudioEffect(soundAudioSrc, fadeAudioEffectSpeed, soundAudioSrc.volume , false));
        }
        else if (audioTypeIndex == (int)Audio.AudioType.BGM)
        {
            StartCoroutine(FadeAudioEffect(bgmAudioSource, fadeAudioEffectSpeed, bgmAudioSource.volume , false));
        }
        else if (audioTypeIndex == (int)Audio.AudioType.UISounds)
        {
            StartCoroutine(FadeAudioEffect(uiAudioSource, fadeAudioEffectSpeed, uiAudioSource.volume , false));
        }
        else if (audioTypeIndex == (int)Audio.AudioType.Ambient)
        {
            StartCoroutine(FadeAudioEffect(ambientAudioSource, fadeAudioEffectSpeed, ambientAudioSource.volume , false));
        }
        else {
            Debug.LogError($"the audio index {audioTypeIndex} cis not a valid audio type\nPlease use 1 - 4 ");
        }

    }

    public void StopFade(int audioTypeIndex, float fadeAudioEffectSpeed)
    {

        if (audioTypeIndex == (int)Audio.AudioType.Sound)
        {
            StartCoroutine(FadeAudioEffect(soundAudioSrc, fadeAudioEffectSpeed, soundAudioSrc.volume , false));
        }
        else if (audioTypeIndex == (int)Audio.AudioType.BGM)
        {
            StartCoroutine(FadeAudioEffect(bgmAudioSource, fadeAudioEffectSpeed, bgmAudioSource.volume , false));
        }
        else if (audioTypeIndex == (int)Audio.AudioType.UISounds)
        {
            StartCoroutine(FadeAudioEffect(uiAudioSource, fadeAudioEffectSpeed, uiAudioSource.volume , false));
        }
        else if (audioTypeIndex == (int)Audio.AudioType.Ambient)
        {
            StartCoroutine(FadeAudioEffect(ambientAudioSource, fadeAudioEffectSpeed, ambientAudioSource.volume , false));
        }
        else
        {
            Debug.LogError($"the audio index {audioTypeIndex} cis not a valid audio type\nPlease use 1 - 4 ");
        }
    }

    IEnumerator FadeAudioEffect(AudioSource audioSource, float effectSpeed,float MaxVolume, bool increment = true) {
        float currentVolume = 0;
        if (increment)
        {
            currentVolume = 0;
            while (currentVolume < MaxVolume)
            {
                audioSource.volume = currentVolume;
                currentVolume += Time.deltaTime * 0.01f * effectSpeed;
                yield return null;
            }
        }
        else {
            Debug.Log("fading");
            currentVolume = MaxVolume;
            while (currentVolume > 0)
            {
                audioSource.volume = currentVolume;
                currentVolume -= Time.deltaTime * 0.01f * effectSpeed;
                yield return null;
            }
        }

       
        
    }


    //use audio name to play audio, I know it is a string but what choice we have, maybe change to scriptable object when we have time
    public  void Play(string audioName) {
        
        for (int i = 0; i < audioArray.Length; i++)
        {
            //Debug.Log(audioName + " " + audioArray[i].audioName);
            if (audioArray[i].audioName.ToUpper() == audioName.ToUpper())
            {
               
                switch (audioArray[i].audioType)
                {
                    case Audio.AudioType.Sound:
                        StopCoroutine("FadeAudioEffect");
                        audioArray[i].Play(soundAudioSrc, masterVolume);
                        currentSoundIndex = i;
                        break;
                    case Audio.AudioType.BGM:
                        StopCoroutine("FadeAudioEffect");
                        audioArray[i].Play(bgmAudioSource, masterVolume);
                        currentBGMIndex = i;
                        break;
                    case Audio.AudioType.UISounds:
                        StopCoroutine("FadeAudioEffect");
                        audioArray[i].Play(uiAudioSource, masterVolume);
                        currentUISoundIndex = i;
                        break;
                    case Audio.AudioType.Ambient:
                        StopCoroutine("FadeAudioEffect");
                        audioArray[i].Play(ambientAudioSource, masterVolume);
                        currentAmbientIndex = i;
                        break;
                    default:
                        Debug.LogError($"the audio clip {audioArray[i].audioName} does not have a audio type assigned \n or there is no play option for that type yet");
                        break;
                }
                return;
            }
           
            
        }

       
    } 


    //store audio data
    [Serializable]
    public class Audio {
        public string audioName = "";
        public enum AudioType { Sound, BGM, UISounds, Ambient}
        public AudioType audioType = AudioType.Sound;
        public AudioClip clip;
        public bool loop = false;
        public float defaultVolume = 1;
       

        public void Play(AudioSource audioSrc, float volume) {
            audioSrc.clip = clip;
            audioSrc.volume = defaultVolume * volume;
            audioSrc.loop = loop;
           
            
            audioSrc.Play();
            
        }

        public void updateVolume(AudioSource audioSrc, float volume) {
            Debug.Log("update");
            audioSrc.volume = defaultVolume * volume;
        }

    }
}
