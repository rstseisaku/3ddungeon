using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {

    // 探索処理開始
	public void Adventure()
    {
        StartCoroutine("MoveScene", "TES");
    }

    // パーティー編成
    public void Organaize()
    {
        StartCoroutine("MoveScene", "PartyOrganaize");
    }

    // ガチャ
    public void Gacha()
    {

    }

    public void Compose()
    {

    }

    // ショップ
    public void Shop()
    {

    }





    private IEnumerator MoveScene(string ScenePath)
    {
        yield return Utility.MoveScene(ScenePath, "Images\\Background\\Black",90);
    }
}
