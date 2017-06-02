using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mDecPartyObj : MonoBehaviour {
    public bool isDecided = false;
    public int touchedId = -1;

    GameObject[] partyViewObj;

    /* 利用しているオブジェクトを生成＆アドレスを取得 */
    public void GenerateObject(SaveParty saveParty)
    {
        partyViewObj = new GameObject[Variables.Party.Num];

        for (int i = 0; i < Variables.Party.Num; i++)
        {
            string FilePath = "Prefabs\\Party\\PartyView";
            partyViewObj[i] = Utility._Object.MyInstantiate(FilePath, this.gameObject);
            GameObject child;

            for (int j = 0; j < Variables.Party.CharaNumPerParty; j++)
            {
                // コオブジェクトにアクセス
                string str = "Chara" + (j + 1);
                child = partyViewObj[i].transform.Find(str).gameObject;
                // パーティキャラクタの情報を読み取る
                int charaId = saveParty.partyCharacterId[i, j];
                if (charaId < 0)
                {
                    // 子オブジェクトに画像を貼り付ける(パーティ顔グラ)
                    child.GetComponent<Image>().sprite = Utility._Image.MyGetSprite("Images\\Face\\c999");
                    continue;
                }
                // 子オブジェクトに画像を貼り付ける(パーティ顔グラ)
                child.GetComponent<Image>().sprite = Utility._Image.MyGetSprite(
                    CharacterStatus.LoadPlayerCharacterStringStatus(charaId, _ST._faceGraphicPath));
            }

            // パーティ編成用のボタン(非表示で)
            child = partyViewObj[i].transform.Find("Button").gameObject;
            child.GetComponent<Button>().onClick.AddListener(() => this.onClick(child)); // DecideEditParty.onClick(child)
            child.name = "" + i;
            // 場所指定
            partyViewObj[i].GetComponent<RectTransform>().localPosition =
                new Vector2(Variables.Party.DecEditPartyPosX[i], Variables.Party.DecEditPartyPosY[i]);
        }
    }

    public void SetReturnString(string str)
    {
        GameObject child = transform.Find("Return").gameObject.transform.Find("Text").gameObject;
        child.GetComponent<Text>().text = str;
    }

    public void SetDescribeString(string str)
    {
        GameObject child = transform.Find("Describe").gameObject;
        child.GetComponent<Text>().text = str;
    }

    public void onClick(GameObject obj)
    {
        if( obj.name == "Return")
        {
            touchedId = -2;
            isDecided = true;
            return;
        }
        touchedId = int.Parse(obj.name);
        isDecided = true;
    }

}
