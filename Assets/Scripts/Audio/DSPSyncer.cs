

using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DSPSyncer : MonoBehaviour
{
    public static DSPSyncer instance;

    public double dspStartTime;
    public double lastFilterDspTime;
    public double currentFilterDspTime;
    public double lastFrameDspTime;
    public static double currentEstimatedDspTime;
    public double songStartedTime;
    public double deltaFrameDspTime;
    public double deltaSyncDspTime;
    public double deltaFilterDspTime;
    
    // If currentEstimatedDspTime is more than this amount greater than AudioSettings.dspTime, 
    public double allowEstimatedAndAudioSettingsDesyncAmount = 0.1f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        dspStartTime = AudioSettings.dspTime;
        lastFilterDspTime = currentFilterDspTime = lastFrameDspTime = currentEstimatedDspTime = deltaFrameDspTime = deltaSyncDspTime = deltaFilterDspTime = dspStartTime;
    }

    void Update()
    {
        deltaFrameDspTime = currentEstimatedDspTime - lastFrameDspTime;
        lastFrameDspTime = currentEstimatedDspTime;
        UpdateFilterTime();
        SyncDspTime();

        Debug.Log("DSPSyncer current time (adds delta time): " + currentEstimatedDspTime + ". AudioSettings.dspTime straight is: " + AudioSettings.dspTime);
    }

    public void SongStartsAt(double time)
    {
        songStartedTime = time;
    }
    
    public double GetDspSongProgress()
    {
        return currentEstimatedDspTime - songStartedTime;
    }

    void UpdateFilterTime()
    {
        lastFilterDspTime = currentFilterDspTime;
        currentFilterDspTime = AudioSettings.dspTime;
        deltaFilterDspTime = currentFilterDspTime - lastFilterDspTime;
    }

    void SyncDspTime()
    {
        double last = currentEstimatedDspTime;
        // duplicate DSP time, so we update with unscaled delta time
        if (lastFilterDspTime == currentFilterDspTime)
        {
            Debug.Log("duplicate");
            currentEstimatedDspTime += Time.unscaledDeltaTime;
        }
        else
        {
            Debug.Log("no duplicate");
            lastFilterDspTime = currentFilterDspTime;
            currentEstimatedDspTime = currentFilterDspTime;
        }
        deltaSyncDspTime = currentEstimatedDspTime - last;
    }
}