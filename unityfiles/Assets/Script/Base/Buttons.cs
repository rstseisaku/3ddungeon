using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {

    // 探索処理開始
	public void Adventure()
    {
        StartCoroutine("StartAdventure");
    }

    // パーティー編成
    public void Organaize()
    {
        StartCoroutine("MoveScene", "PartyOrganaize");
    }

    // ガチャ
    public void Gacha()
    {
        StartCoroutine("MoveScene", "Gacha");
    }

    public void Compose()
    {

    }

    // ショップ
    public void Shop()
    {

    }



    /* 探索開始 */
    private IEnumerator StartAdventure()
    {
        GameObject obj = GameObject.Find(Variables.Save.Name); ; // パーティーオブジェクトを探す
        mSaveData saveData = obj.GetComponent<mSaveData>();
        yield return saveData.WaitLoad();        

        // パーティ選択
        yield return DecideEditParty.Loop(saveData.GetSaveParty());
        int id = DecideEditParty.editPartyId;
        if( id >= 0)
        {
            saveData.GetSaveParty().mainParty = id;
        }
        else
        {
            yield break;
        }

        yield return MoveScene("TES");
    }

    private IEnumerator MoveScene(string ScenePath)
    {
        yield return Utility._Scene.MoveScene(ScenePath, "Images\\Background\\Black",90);
    }
}
