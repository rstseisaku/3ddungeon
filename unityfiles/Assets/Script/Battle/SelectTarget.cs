using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTarget : MonoBehaviour {
    public int selectId = -1;
    public int mouseOverId = 0;
    GameObject[] buttonObj;


    public void SetParameter( BaseCharacter[] ecd )
    {
        // ボタンオブジェクトのインスタンス化
        buttonObj = new GameObject[ecd.Length];

        // ボタンオブジェクト取得
        buttonObj[0] = gameObject.transform.FindChild("Button").gameObject;
        buttonObj[0].transform.SetParent(this.transform);
        buttonObj[0].name = "" + 0;
        buttonObj[0].transform.FindChild("Text").GetComponent<Text>().text =
             ecd[0].cs.charaName;

        for (int i = 1; i < ecd.Length; i++)
        {
            buttonObj[i] = (GameObject)Instantiate(buttonObj[0]);
            buttonObj[i].transform.SetParent(this.transform, false);
            buttonObj[i].GetComponent<RectTransform>().localPosition += id2Pos(i);
            buttonObj[i].name = "" + i;
            buttonObj[i].transform.FindChild("Text").GetComponent<Text>().text =
                ecd[i].cs.charaName;
        }
        // ボタン0の座標をずらすのは最後(起点座標になっているため)
        buttonObj[0].GetComponent<RectTransform>().localPosition += id2Pos(0);
    }

    public void SetEnableButtonFromKnockout( BaseCharacter[] enemyBc)
    {
        for (int i = 0; i < enemyBc.Length; i++)
            buttonObj[i].GetComponent<Button>().interactable =
                !enemyBc[i].isknockout;
    }

    public void MyOnClick( Button obj )
    {
        if( obj.name == "Return")
        {
            selectId = -2; // キャンセル
            return;
        }
        selectId = int.Parse(obj.name);
    }

    public void MyMouseEnter(Button obj)
    {
        mouseOverId = int.Parse(obj.name);
    }

    private Vector3 id2Pos(int i)
    {
        int x = -100 + 200 * (i % 2);
        int y = (-i / 2) * 80;
        return new Vector3(x, y, 0);
    }
}
