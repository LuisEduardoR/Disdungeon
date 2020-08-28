using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    public static AudioController instance;

    [System.Serializable]
    private class Sound {

        public string id;
        public AudioClip clip;

    }

    [SerializeField] private Sound[] sounds;
    private Dictionary<string, AudioClip> dictionary;

    [SerializeField] private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        if(instance != null) {

            Debug.Log("AudioController: More than one instance of this script was found!");
            return;

        }

        instance = this;

        // Builds the sound dictionary.
        dictionary = new Dictionary<string, AudioClip>();
        for(int i = 0; i < sounds.Length; i++) {

            dictionary.Add(sounds[i].id, sounds[i].clip);

        }

    }

    // Plays a sound.
    public void Play(string id)
    {
        
        source.Stop();
        source.clip = dictionary[id];
        source.Play();

    }
}
