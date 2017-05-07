using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    static GameObject soundObj;
    static Sound sound;

    public static void SceneChangePlaySound(Variables.BGM.BgmName id)
    {
        soundObj = GameObject.Find(Variables.SE.soundObjName);
        if (soundObj == null)
        {
            soundObj = new GameObject(Variables.SE.soundObjName);
            sound = soundObj.AddComponent<Sound>();
            sound.LoadSound();
            DontDestroyOnLoad(soundObj);
        }
        else
        {
            sound = soundObj.GetComponent<Sound>();
        }

        if (Variables.__Debug.isNotBGMPlay) return;

        // このシーンで流すBGMを設定
        sound.PlayBgm(id);
    }

    public static void PlaySe(Variables.SE.SeName seId)
    {
        sound.PlaySe(seId);
    }
}
