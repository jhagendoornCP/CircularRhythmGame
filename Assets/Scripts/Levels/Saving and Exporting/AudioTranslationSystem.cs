
// Converts from file to AudioClip
using System.Collections;
using EventStreams;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class AudioTranslationSystem : MonoBehaviour
{
    [SerializeField] private AudioClipEventStream loadedAudioStream;
    public void InitializeAudioClip(string fileLocation)
    {
        StartCoroutine(LoadAudioClip(fileLocation));
    }

    private IEnumerator LoadAudioClip(string fileLocation)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file://{fileLocation}", AudioType.OGGVORBIS))
        {
            ((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }
}