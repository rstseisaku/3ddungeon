using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {
    GameObject soundObj;
    Sound sound;

    public void Start()
    {
        SoundManager.SceneChangePlaySound(Variables.BGM.BgmName.title);
    }


    // 探索処理開始
    public void Adventure()
    {
        SoundManager.PlaySe(Variables.SE.SeName.system_dec);
        StartCoroutine("StartAdventure");
    }

    // パーティー編成
    public void Organaize()
    {
        SoundManager.PlaySe(Variables.SE.SeName.system_dec);
        StartCoroutine("MoveScene", "PartyOrganaize");
    }

    // ガチャ
    public void Gacha()
    {
        SoundManager.PlaySe(Variables.SE.SeName.system_dec);
        StartCoroutine("MoveScene", "Gacha");
    }

    public void Compose()
    {
        SoundManager.PlaySe(Variables.SE.SeName.system_dec);
    }

    // ショップ
    public void Shop()
    {
        SoundManager.PlaySe(Variables.SE.SeName.system_dec);
    }

    /* 探索開始 */
    private IEnumerator StartAdventure()
    {
        // 探索開始
        GameObject obj = GameObject.Find(Variables.Save.Name); ; // パーティーオブジェクトを探す
        mSaveData saveData = obj.GetComponent<mSaveData>();
        
        // パーティ選択画面を表示
        yield return DecideEditParty.Loop(saveData.GetSaveParty(), "【戻る】");
        int id = DecideEditParty.editPartyId;
        if ( id >= 0)
            saveData.GetSaveParty().mainParty = id;
        else
            yield break;

        // セーブデータオブジェクトを破棄して
        // パーティオブジェクトを生成
        saveData.StartAdventure();

        yield return MoveScene("TES");
    }

    private IEnumerator MoveScene(string ScenePath)
    {
        yield return Utility._Scene.MoveScene(ScenePath, "Images\\Background\\Black",90);
    }
}
