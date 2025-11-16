using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    // tick and tock (in case you cant read)
    [SerializeField] private AudioClip tick;
    [SerializeField] private AudioClip tock;

    private AudioSource source;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        source = GetComponent<AudioSource>();
    }

    public void LoadSong(AudioClip song)
    {
        source.clip = song;
    }

    public void PlaySong()
    {
        Debug.Log("Starting the song");
        // will need to chang ein the firustre lol !!
        source.Play();
    }

    public void PlayTick() => source.PlayOneShot(tick);
    public void PlayTock() => source.PlayOneShot(tock);

}
