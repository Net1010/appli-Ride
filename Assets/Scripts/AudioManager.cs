using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
    public AudioSource bgm;
    public float bgmVol;

    private string menuBgm;

    private IEnumerator fadeCoroutine;
    private bool fadeRunning;
    void Awake()
    {
        if (instance == null) 
        { 
            instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        else { Destroy(gameObject); }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop ;
        }
        fadeRunning = false;
        bgmVol = 0;
        menuBgm = "CozyAfternoon";
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game") {
            if (bgm == null) { Play(menuBgm, true); }
        }
    }
    public Sound FindTrack(string name)
    {
        return Array.Find(sounds, sound => sound.name == name);
    }
    public void Play(string name, bool music = false)
    {
        Sound s = FindTrack(name);
        if (s == null) {
            Debug.LogWarning("Sound " + name + " not found");
            return; 
        }
        else {
            if (music) {
                //stops previous bgm if it's still fading out
                if (fadeRunning) { InterruptBGM(); }
                bgm = s.source;
                bgmVol = bgm.volume;
                s.source.Play();
                //fade music in
                fadeRunning = true;
                fadeCoroutine = FadeAudio(2f, 0f, bgmVol);
                StartCoroutine(fadeCoroutine);
            }
            else { s.source.Play(); }
        }
    }

    public void PlayClick() { Play("ButtonClick"); }

    //stops previous bgm if it's still fading out
    public void InterruptBGM()
    {
        StopCoroutine(fadeCoroutine);
        bgm.Stop();
        bgm.volume = bgmVol;
    }
    public void SwitchToMenuBGM()
    {
        bgm.Stop();
        bgm.volume = bgmVol;
        Play(menuBgm, true);
    }
    public void Endtrack() {
        if (bgm == null) { return; }
        if (fadeRunning) { StopCoroutine(fadeCoroutine); }
        fadeCoroutine = FadeAudio(1f, bgm.volume, 0f);
        StartCoroutine(fadeCoroutine);
        
        
    }
    public void PauseBgmVol()
    {
        if (bgm.volume == bgmVol) { bgm.volume *= 0.3f; } 
        else { bgm.volume = bgmVol; }
    }

    private IEnumerator FadeAudio(float duration, float baseVol, float targetvol)
    {
        fadeRunning = true;
        float i = 0f;
        float rate = 1f / duration;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            bgm.volume = Mathf.Lerp(baseVol, targetvol, i);
            yield return null;
        }
        //if the target is 0 it fades out
        if (targetvol == 0)
        {
            bgm.Stop();
            bgm.volume = bgmVol;
            Play(menuBgm, true);
        }

    }

}
