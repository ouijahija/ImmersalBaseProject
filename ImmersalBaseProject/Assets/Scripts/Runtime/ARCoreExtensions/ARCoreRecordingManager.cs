using System;
using System.IO;
using Google.XR.ARCoreExtensions;
using UnityEngine;

[RequireComponent(typeof(ARRecordingManager))]
public class ARCoreRecordingManager : MonoBehaviour
{
    private ARRecordingManager _arRecordingManager;
    private bool _recording;

    private string _recordingName = "session_recording_0";
    
    private void Awake()
    {
        _arRecordingManager = GetComponent<ARRecordingManager>();
    }
    
    public void ToggleRecording()
    {
        var recordingStatus = _arRecordingManager.RecordingStatus;
        Debug.Log("Current recording status: " + recordingStatus);

        _recording = !_recording;
        if (_recording)
            StartRecording();
        else
            StopRecording();
    }

    private void StartRecording()
    {
        Debug.Log("Starting Recording of Session");
        
        ARCoreRecordingConfig recordingConfig = ScriptableObject.CreateInstance<ARCoreRecordingConfig>();
        string path = Application.persistentDataPath + "/Recordings";
        var dateTime = DateTime.Now.ToString("_dd_MM_yyyy__hh_mm");
       /* if (!Directory.Exists(path))
            Directory.CreateDirectory(path);*/
        Uri recordingUri = new Uri(Application.persistentDataPath + "/" +  _recordingName + dateTime + ".mp4");
        recordingConfig.Mp4DatasetUri = recordingUri;

        Debug.Log("Storing video in : " + recordingConfig.Mp4DatasetFilepath);
        var recordingResult = _arRecordingManager.StartRecording(recordingConfig);
        Debug.Log("Start Recording Result: " + recordingResult);
    }

    private void StopRecording()
    {
        Debug.Log("Stopping Recording of Session");
        var recordingResult = _arRecordingManager.StopRecording();
        Debug.Log("Stop Recording Result: " + recordingResult);
    }
}
