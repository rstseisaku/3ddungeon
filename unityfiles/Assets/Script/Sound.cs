using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {
    public static AudioClip[] audioClip;
    public static AudioSource audioSource;
    public static AudioSource audioSourceBGM;
    public static int playingBgmId = -1;

    public void LoadSound()
    {
        audioClip = new AudioClip[Variables.SE.SeFolderPath.Length];
        audioSource = gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < Variables.SE.SeFolderPath.Length; i++)
        {
            audioClip[i] = (AudioClip)Resources.Load(Variables.SE.SeFolderPath[i]);
        }
        audioSourceBGM = gameObject.AddComponent<AudioSource>();
        audioSourceBGM.playOnAwake = false;
        audioSourceBGM.loop = true;
    }

    public void PlaySe(Variables.SE.SeName seId)
    {
        audioSource.PlayOneShot(audioClip[(int)seId]);
    }

    public void PlayBgm(Variables.BGM.BgmName bgmId)
    {
        // 同じ曲を流そうとした場合は終了
        if( (int)bgmId == playingBgmId) { return; }

        // 流していた曲を停止
        if (audioSourceBGM.isPlaying)
        {
            Debug.Log("なんでここにくるの？？？");
            audioSourceBGM.Stop();
            audioSourceBGM.clip = null;
        }

        // 新しく曲をロードして再生
        AudioClip ac = (AudioClip)Resources.Load(Variables.BGM.BgmFolderPath[(int)bgmId]);
        audioSourceBGM.clip = ac;
        audioSourceBGM.Play();

        playingBgmId = (int)bgmId;
        }
}
