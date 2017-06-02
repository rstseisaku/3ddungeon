using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAction : MonoBehaviour {
    public int selectId;
    public int mouseOverId = -1;
    GameObject[] buttonObj;

	// Use this for initialization
	void Start () {
        selectId = -1;
    }
	

    public void SetParameter( Vector3 vec)
    {
        // ボタンオブジェクトのインスタンス化
        buttonObj = new GameObject[3];

        // ボタンオブジェクト取得
        buttonObj[0] = gameObject.transform.Find("AtkButton").gameObject;
        buttonObj[1] = gameObject.transform.Find("UniButton").gameObject;
        buttonObj[2] = gameObject.transform.Find("MagButton").gameObject;

        // 親オブジェクトに自分自身を設定(一緒に破棄されて欲しい)
        buttonObj[0].transform.SetParent(this.transform);
        buttonObj[1].transform.SetParent(this.transform);
        buttonObj[2].transform.SetParent(this.transform);

        // ボタンの表示・非表示
        for (int i = 0; i < 3; i++)
        {
            buttonObj[i].GetComponent<Button>().interactable = 
                ( vec[i] >= 1 );
        }
    }

    public void myOnClick( int buttonId )
    {
        // 押された ID を格納する
        SoundManager.PlaySe(Variables.SE.SeName.system_dec);
        selectId = buttonId;
    }

    public void MyMouseEnter(Button obj)
    {
        if (obj.name == "AtkButton") mouseOverId = 0;
        if (obj.name == "UniButton") mouseOverId = 1;
        if (obj.name == "MagButton") mouseOverId = 2;
    }
}
