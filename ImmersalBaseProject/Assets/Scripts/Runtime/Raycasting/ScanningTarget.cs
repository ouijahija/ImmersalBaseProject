using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScanningTarget : MonoBehaviour, IRaycastSelectable
{
    public Material scanningMaterial;
    public State state = State.unscanned;
    public float totalScanTime = 3;

    public Slider progressSlider;
    public AudioSource audioSource;
    public bool destroyAfterScan = true;

    public UnityEvent onScannStarted;
    public UnityEvent onFinishedScanning;


    [Header("SFX")]
    public AudioClip startScanning;
    public AudioClip inProgressLoop;
    public AudioClip success;
    public AudioClip cancel;

    new Renderer renderer;
    Material[] defaultMaterials;
    Material[] scanningMaterials;

    float progress = 0;

    public enum State
    {
        unscanned, inProgress, finished
    }

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        progressSlider.gameObject.SetActive(false);
    }

    public void OnRaycastReceived()
    {
        if (state == State.unscanned)
        {
            StartScanning();
        }
    }

    private void Update()
    {
        if (progress >= totalScanTime)
        {
            FinishScanning();
            return;
        }

        if (state == State.inProgress)
        {
            if (!audioSource.isPlaying)
                PlaySound(inProgressLoop, true);

            progress += Time.deltaTime;

            progressSlider.value = Mathf.Clamp01(progress / totalScanTime);
        }
    }

    private void StartScanning()
    {
        onScannStarted?.Invoke();
        PlaySound(startScanning);

        progressSlider.gameObject.SetActive(true);
        state = State.inProgress;

        //Start Scanning Process
        if (defaultMaterials == null)
            defaultMaterials = renderer.materials;

        if (scanningMaterials == null)
        {
            scanningMaterials = new Material[defaultMaterials.Length + 1];
            defaultMaterials.CopyTo(scanningMaterials, 0);
            scanningMaterials[defaultMaterials.Length] = scanningMaterial;
        }
        renderer.materials = scanningMaterials;
    }

    void FinishScanning()
    {
        PlaySound(success);

        progressSlider.gameObject.SetActive(false);
        renderer.materials = defaultMaterials;
        state = State.finished;
        progress = 0;

        MessageBox.ShowMessage("Scan complete: " + name);
        onFinishedScanning?.Invoke();

        if (destroyAfterScan)
            StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    void CancelScanning()
    {
        PlaySound(cancel);

        progressSlider.gameObject.SetActive(false);
        renderer.materials = defaultMaterials;
        state = State.unscanned;
        progress = 0;
    }

    void PlaySound(AudioClip clip, bool looping = false)
    {
        if (audioSource.isPlaying)
            StopSound();

        audioSource.clip = clip;
        audioSource.loop = looping;

        audioSource.Play();
    }

    void StopSound()
    {
        audioSource.Stop();
    }

    private void OnBecameInvisible()
    {
        if (state == State.inProgress)
        {
            CancelScanning();
        }
    }
}
