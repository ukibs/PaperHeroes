using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject audioObjectPrefab;
    //public AudioClip[] deathClips;

    [HideInInspector] public static AudioManager Instance;

    private AudioSource musicManager;

    //private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        musicManager = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void PlayRandomDeathSound()
    //{
    //    int deathSound = Random.Range(0, deathClips.Length);
    //    AudioSource audioSource = Instantiate(audioObjectPrefab, Vector3.zero, Quaternion.identity).GetComponent<AudioSource>();
    //    audioSource.clip = deathClips[deathSound];
    //    audioSource.Play();
    //    Destroy(audioSource.gameObject, deathClips[deathSound].length);
    //}

    public void PlaySound(AudioClip clip)
    {
        AudioSource audioSource = Instantiate(audioObjectPrefab, Vector3.zero, Quaternion.identity).GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }

    public void PutGameOverClip(AudioClip clip, float volume = 1)
    {
        musicManager.clip = clip;
        musicManager.volume = volume;
        musicManager.loop = false;
        musicManager.Play();
    }
}
