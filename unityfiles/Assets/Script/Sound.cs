using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {
    public AudioClip audioClip;
    public AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioClip = (AudioClip)Resources.Load(Variables.SE.SeFolderPath);
    }

    public void PlaySe()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
