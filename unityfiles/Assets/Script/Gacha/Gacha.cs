using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * パーティー情報を管理するクラス
 * ( 全てのシーンで利用 )
 */
class Gacha : MonoBehaviour
{
    GameObject canvas;
    GameObject resultObj;
    int getCharaResultId;
    mSaveData saveData;

    IEnumerator Start()
    {
        canvas = GameObject.Find("GachaCanvas");
        /* セーブデータが存在すれば読み込む */
        saveData = GameObject.Find(Variables.Save.Name).GetComponent<mSaveData>();
        yield return saveData.WaitLoad();

        StartCoroutine("MainLoop");
    }

    /*
     * ================================================
     * ゲームメインループ
     * ================================================
     */
    IEnumerator MainLoop()
    {
        yield return 0;
    }


    public void onClick( GameObject obj)
    {
        switch (obj.name)
        {
            case "Gacha":
                StartCoroutine("PlayGacha");
                break;
            case "Return":
                StartCoroutine("Return");
                break;
            case "ResultEnd":
                StartCoroutine("ResultEnd");
                break;
            default:
                break;
        }
    }

    /* 戻る */
    IEnumerator Return()
    {
        yield return Utility._Scene.MoveScene("Base");
    }

    /* ガチャを回す */
    IEnumerator PlayGacha()
    {
        /* 引くキャラのID */
        getCharaResultId = Random.Range(1, Variables.Unit.Num + 1);

        /* データを持ってくる */
        /* ( 一部ロード ⇒ 全体ロード＆部品読み込みにすべき) */
        string charaStand = CharacterStatus.LoadPlayerCharacterStringStatus(
            getCharaResultId, _ST._standGraphicPath);
        string description = CharacterStatus.LoadPlayerCharacterStringStatus(
            getCharaResultId, _ST._description);

        /* リザルトオブジェクト */
        // :透明のボタン
        resultObj = Utility._Object.MyGenerateImage(
            Variables.Gacha.BackgroundPath,
            canvas,
            new Vector2(1280, 960));
        resultObj.name = "ResultEnd";
        resultObj.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        Button bt = resultObj.AddComponent<Button>();
        bt.onClick.AddListener(() => this.onClick(resultObj));
        // :キャラ画像(親子関係を持たせると被せてもクリック可能)
        GameObject charaObj = Utility._Object.MyGenerateImage(
            charaStand,
            resultObj,
            new Vector2(360, 720));
        charaObj.GetComponent<Image>().color = Color.white;
        // :説明文
        GameObject textObj = Utility._Object.MyInstantiate(
            "Prefabs\\Gacha\\GachaText",
            resultObj,
            new Vector2(0,-200));
        textObj.GetComponent<Text>().text = description;

        /* 実際の入手処理 */
        saveData.GetObtainChara().isObtainChara[getCharaResultId] = true;

        yield return 0;
    }

    IEnumerator ResultEnd()
    {
        /* 結果オブジェクトを消すだけ */
        Destroy(resultObj);

        yield return 0;
    }

}