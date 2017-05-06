using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class mPartyObject : MonoBehaviour
{
    // パーティーオブジェクト
    GameObject[] partyCharacterObj;
    GameObject[] partyCharaFrameObj;
    Image[] standImage;
    Text[] hpText;
    Image[] elementImage;
    Text partyNameText;
    public int touchId = -1;

    /* 初期化 */
    // ( オブジェクトのアドレスを取得 )
    // ( 外部スクリプトから，インスタンス化した際に利用 )
    public void FindObjectAddress()
    {
        // オブジェクトを探す
        partyCharacterObj = new GameObject[5];
        partyCharaFrameObj = new GameObject[5];
        hpText = new Text[5];
        standImage = new Image[5];
        elementImage = new Image[5];
        partyNameText = transform.FindChild("Text").gameObject.GetComponent<Text>();
        GameObject obj = transform.FindChild("Character").gameObject;
        for (int i = 0; i < partyCharacterObj.Length; i++)
        {
            string str = "Chara" + (i + 1);
            partyCharacterObj[i] = obj.transform.FindChild(str).gameObject;
            partyCharaFrameObj[i] = partyCharacterObj[i].transform.FindChild("Frame").gameObject;
            hpText[i] = partyCharaFrameObj[i].transform.FindChild("Text").gameObject.GetComponent<Text>();
            elementImage[i] = partyCharaFrameObj[i].transform.FindChild("Element").gameObject.GetComponent<Image>();
            standImage[i] = partyCharacterObj[i].GetComponent<Image>();
        }
    }


    // クリックされた際のイベント
    public void onClick( GameObject obj)
    {
        // 戻るボタン
        if( obj.name == "Return")
        {
            touchId = -2;
            return;
        }

        // クリックされたキャラのIDを取得
        string objName = obj.name;
        objName = objName.Replace("Chara", "");
        touchId = int.Parse(objName) - 1;
    }


    // パーティ編成画面用のオブジェクトへアクセスするインタフェースを提供
    public void SetPartyName( string name) { partyNameText.text = name; }
    public void SetHpText(int partyId, int hp) { hpText[partyId].text = "HP  " + hp;  }
    public void SetStandImage(int partyId, string standGraFilePath) 
    {
        standImage[partyId].sprite = Utility._Image.MyGetSprite(standGraFilePath);
    }
    public void SetElement( int partyId, int element)
    {
        elementImage[partyId].sprite = Utility._Image.MyGetSprite("Images/Icon/icon" + element);
    }
    public void CharacterSetActive(int partyId, bool isActive) { partyCharacterObj[partyId].SetActive(isActive); }
    public void FrameSetActive(int partyId, bool isActive) { partyCharaFrameObj[partyId].SetActive(isActive); }
}
