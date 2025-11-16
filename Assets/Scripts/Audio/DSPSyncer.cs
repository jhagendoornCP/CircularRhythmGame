

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
        SyncDspTime();

        // Debug.Log("DSP syncer says current time: " + currentEstimatedDspTime + " and the audio settings say " + AudioSettings.dspTime);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        lastFilterDspTime = currentFilterDspTime;
        currentFilterDspTime = AudioSettings.dspTime;
        deltaFilterDspTime = currentFilterDspTime - lastFilterDspTime;
    }

    public void SongStartsAt(double time)
    {
        songStartedTime = time;
    }
    
    public double GetDspSongProgress()
    {
        return currentEstimatedDspTime - songStartedTime;
    }

    void SyncDspTime()
    {
        double last = currentEstimatedDspTime;
        // duplicate DSP time, so we update with unscaled delta time
        if (lastFilterDspTime == currentFilterDspTime)
        {
            currentEstimatedDspTime += Time.unscaledDeltaTime;
        }
        else
        {
            lastFilterDspTime = currentFilterDspTime;
            currentEstimatedDspTime = currentFilterDspTime;
        }
        deltaSyncDspTime = currentEstimatedDspTime - last;
    }
}