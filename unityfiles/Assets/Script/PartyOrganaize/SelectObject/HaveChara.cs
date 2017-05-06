using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class HaveChara : MonoBehaviour
{
    public List<GameObject> CharaImage;
    public GameObject TextObject = null;
    string unitDescription = "";
    public int touchId = -1;

    /* 利用するオブジェクトを生成する */
    public void GenerateObject(bool[] isObtainChara)
    {
        // キャンバスの取得
        GameObject parentObj = transform.FindChild("Characters").gameObject;

        // 外す
        int drawPosId = 0;
        GameObject obj = GenerateReturnObj(drawPosId, parentObj);

        // キャラの配置
        drawPosId++;
        for (int i = 1; i <= Variables.Unit.Num; i++)
        {
            // 獲得していないキャラクターであれば終了
            if (!isObtainChara[i]) continue;

            // 表示オブジェクトの内容をセット
            obj = GenerateFaceObject(i, drawPosId, parentObj);

            // 追加
            CharaImage.Add(obj);

            drawPosId++;
        }
    }

    private GameObject GenerateFaceObject(int charaId, int drawPosId, GameObject parentObj)
    {
        // 表示オブジェクトの内容をセット
        GameObject obj = Utility._Object.MyInstantiate(
            "Prefabs\\Party\\HaveCharaButton",
            parentObj,
            CharacterStatus.LoadPlayerCharacterStringStatus(charaId, _ST._faceGraphicPath),
            new Vector2(144, 144));
        obj.GetComponent<RectTransform>().localPosition = Id2Pos(drawPosId);
        obj.name = "" + charaId;
        // ボタンの中身のセット(イベントハンドラの登録)
        Button b = obj.GetComponent<Button>();
        b.onClick.AddListener(() => this.onClick(obj));
        // トリガーイベントを追加する(OnEneter)
        EventTrigger et = obj.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { onEnter(obj); });
        et.triggers.Add(entry);
        return obj;
    }

    private GameObject GenerateReturnObj(int drawPosId, GameObject parentObj)
    {
        // 表示オブジェクトの内容をセット
        GameObject obj = Utility._Object.MyInstantiate(
            "Prefabs\\Party\\HaveCharaButton",
            parentObj,
            "Images\\Face\\c999",
            new Vector2(144, 144));
        obj.GetComponent<RectTransform>().localPosition = Id2Pos(drawPosId);
        obj.name = "" + (-3);
        // ボタンの中身のセット(イベントハンドラの登録)
        Button b = obj.GetComponent<Button>();
        b.onClick.AddListener(() => this.onClick(obj));
        // トリガーイベントを追加する(OnEneter)
        EventTrigger et = obj.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { onEnter(obj); });
        et.triggers.Add(entry);
        return obj;
    }

    // Imageの場所
    private Vector2 Id2Pos(int i)
    {
        int x = -472 + (i % 7) * 156;
        int y = 128 - (i / 7) * 156;
        return new Vector2(x, y);
    }



    public void onClick(GameObject obj)
    {
        touchId = int.Parse(obj.name);
    }
    public void onEnter(GameObject obj)
    {
        Debug.Log(obj);
        int id = int.Parse(obj.name);
        if (id < 0) return;

        // 文字列表示
        Debug.Log(TextObject);
        unitDescription = CharacterStatus.LoadPlayerCharacterStringStatus(id, _ST._description);
    }

    private void OnGUI()
    {
        if (TextObject == null)
        {
            // キャンバスの取得
            GameObject parentObj = transform.FindChild("Characters").gameObject;
            // テキストオブジェクトの生成
            TextObject = new GameObject("DescritionText");
            TextObject.transform.SetParent(parentObj.transform, false);
            TextObject.AddComponent<Text>();
            TextObject.GetComponent<Text>().fontSize = 40;
            TextObject.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            TextObject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            TextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1280,80);
            TextObject.GetComponent<RectTransform>().localPosition = new Vector3(0,-300,0);
        }
        TextObject.GetComponent<Text>().text = unitDescription;
    }
}
