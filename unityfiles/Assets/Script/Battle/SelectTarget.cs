using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTarget : MonoBehaviour {
    public int selectId = -1;
    public int mouseOverId = 0;
    GameObject[] buttonObj;


    public void SetParameter( CharacterBase[] ecd )
    {
        // ボタンオブジェクトのインスタンス化
        buttonObj = new GameObject[ecd.Length];

        // ボタンオブジェクト取得
        buttonObj[0] = gameObject.transform.FindChild("Button").gameObject;
        buttonObj[0].transform.SetParent(this.transform);
        buttonObj[0].name = "" + 0;
        buttonObj[0].transform.FindChild("Text").GetComponent<Text>().text =
             ecd[0].charaName;

        for (int i = 1; i < ecd.Length; i++)
        {
            buttonObj[i] = (GameObject)Instantiate(buttonObj[0]);
            buttonObj[i].transform.SetParent(this.transform, false);
            buttonObj[i].GetComponent<RectTransform>().localPosition +=
               new Vector3(0, -i * 88, 0);
            buttonObj[i].name = "" + i;
            buttonObj[i].transform.FindChild("Text").GetComponent<Text>().text =
                ecd[i].charaName;
        }
    }

    public void MyOnClick( Button obj )
    {
        if( obj.name == "Return")
        {
            selectId = -2; // キャンセル
        }
        selectId = int.Parse(obj.name);
    }

    public void MyMouseEnter(Button obj)
    {
        mouseOverId = int.Parse(obj.name);
    }
}
