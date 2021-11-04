using System;
using System.IO;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaybackManager))]
public class ARCorePlaybackManager : MonoBehaviour
{
    private ARPlaybackManager _arPlaybackManager;
    [SerializeField] private ARSession _arSession;
    private bool setPlaybackDataset;
    private float timeout;

    private string _lastRecordingFile;
    private Uri _recordingUri;
    
    private void Start()
    {
        _arPlaybackManager = GetComponent<ARPlaybackManager>();
    }

    public void StartPlayback()
    {
        Debug.Log("Starting playback of recording: ");
        _lastRecordingFile = GetLastRecordingFile(Application.persistentDataPath);
        Debug.LogWarning(_lastRecordingFile);
        
        _recordingUri = new Uri(_lastRecordingFile);
        setPlaybackDataset = true;

        // Pause the current session.
        _arSession.enabled = false;

        // Set a timeout for retrying playback retrieval.
        timeout = 10f;
    }

    public void TogglePlayback()
    {
        _arSession.enabled = !_arSession.enabled;
    }
    
    private string GetLastRecordingFile(string directory)
    {
        DirectoryInfo dir = new DirectoryInfo(directory);
        FileInfo[] info = dir.GetFiles("*.mp4*");

        string lastRecordingFile = String.Empty;
        foreach (FileInfo f in info)
        { Debug.LogWarning(f.ToString());
           lastRecordingFile = f.ToString();
        }

        return lastRecordingFile;

    }

    void Update()
    {
        PlaybackResult result;
        if (setPlaybackDataset)
        {
            result = _arPlaybackManager.SetPlaybackDatasetUri(_recordingUri);
            if (result == PlaybackResult.ErrorPlaybackFailed  || result == PlaybackResult.SessionNotReady)
            {
                Debug.LogWarning("Playback error");
                // Try to set the dataset again in the next frame.
                timeout -= Time.deltaTime;
                
            }
            else
            {
                Debug.LogWarning("Playback Result: " + result);
                // Do not set the timeout if the result is something other than ErrorPlaybackFailed.
                timeout = -1f;
            }

            if (timeout < 0.0f)
            {
                setPlaybackDataset = false;
                if (result == PlaybackResult.OK)
                {
                    Debug.LogWarning("Playback succeeded");
                  
                }
                else
                {
                    Debug.LogWarning("Playback failed");
                }
                _arSession.enabled = true;
                // If playback is successful, proceed as usual.
                // If playback is not successful, handle the error appropriately.
            }
        }

      
    }

  
}
