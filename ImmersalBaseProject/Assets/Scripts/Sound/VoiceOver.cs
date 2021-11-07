using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class VoiceOver : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource source;

    public UnityEvent OnFinished;

    bool playing;

    private void Update()
    {
        if (playing && !source.isPlaying)
        {
            mixer.SetFloat("AmbientVolume", 0);
            playing = false;

            //Stoped Playing
            OnFinished?.Invoke();
            gameObject.SetActive(false);
        }
        if (!playing && source.isPlaying)
        {
            mixer.SetFloat("AmbientVolume", -10);
            playing = true;
        }
    }
}
