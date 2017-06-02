using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ================================================
 *  リザルト画面を扱う
 * ================================================
 */
public class BattleResult : MonoBehaviour
{
    static GameObject resultObj;
    static GameObject[] resultFaceObj;
    static GameObject resultCharas;

    // 勝利シーン
    public static IEnumerator ResultWinScene(BaseCharacter[] bc, GameObject c)
    {
        // 領域を確保
        resultFaceObj = new GameObject[BCV.PLAYER_MAX];

        // リザルトオブジェクトの表示
        resultObj = Utility._Object.MyInstantiate(
            BCV.RESULT_PREFAB,
            c);
        // 顔グラフィックオブジェクトの子供を取得する
        resultCharas = resultObj.transform.Find("Charas").gameObject;

        // 画像を貼り付ける
        for (int i = 0; i < bc.Length; i++)
        {
            String str = "Chara_" + (i + 1);
            resultFaceObj[i] = resultCharas.transform.Find(str).gameObject;
            if (bc[i].ctbFaceObj == null)
            {
                resultFaceObj[i].SetActive(false); // 編成されていない
                continue;
            }
            resultFaceObj[i].GetComponent<Image>().sprite =
                bc[i].ctbFaceObj.faceObj.GetComponent<Image>().sprite;
        }
        // 画像が張り付いていない場所は消す(編成数が4以下)
        for (int i = bc.Length; i < BCV.PLAYER_MAX; i++)
        {
            String str = "Chara_" + (i + 1);
            resultFaceObj[i] = resultCharas.transform.Find(str).gameObject;
            resultFaceObj[i].SetActive(false);
        }

        SoundManager.PlaySe(Variables.SE.SeName.battle_result);
        yield return Utility._Wait.WaitKey();

        // オブジェクト消去
        Destroy(resultObj);

        yield return null;
    }

    // 敗北シーン
    public static IEnumerator ResultLoseScene(BaseCharacter[] bc, GameObject c)
    {
        // リザルトオブジェクトの表示
        resultObj = Utility._Object.MyInstantiate(
            BCV.RESULT_LOSE_PREFAB,
            c);

        yield return Utility._Wait.WaitKey();

        // オブジェクト消去
        Destroy(resultObj);

        yield return 0;
    }

    // フェードアウト処理
    public static IEnumerator ResultFadeout(GameObject c)
    {
        int frame = 30;
        GameObject fade = Utility._Object.MyInstantiate(
            BCV.FACE_IMAGE_PREFAB,
            c,
            "Images\\Background\\Black",
            new Vector2(1280,720));

        // 黒画像を徐々に濃く表示する
        for ( int i=0; i< frame; i++) {
            float alpha = ((1.0f) / frame) * i;
            fade.GetComponent<Image>().color =
                new Color(0.0f, 0.0f, 0.0f, alpha);
            yield return 0;
        }
    }
}