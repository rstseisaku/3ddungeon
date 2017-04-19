using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAction : MonoBehaviour {
    // new を使わないためのダミーオブジェクト(new を使うと Warnigが出る)
    public int selectId;
    GameObject[] buttonObj;

	// Use this for initialization
	void Start () {
        selectId = -1;
    }
	

    public void SetParameter( Vector3 vec)
    {
        // ボタンオブジェクトのインスタンス化
        buttonObj = new GameObject[3];
        buttonObj[0] = new GameObject();
        buttonObj[1] = new GameObject();
        buttonObj[2] = new GameObject();

        // ボタンオブジェクトを
        buttonObj[0] = gameObject.transform.FindChild("AtkButton").gameObject;
        buttonObj[1] = gameObject.transform.FindChild("UniButton").gameObject;
        buttonObj[2] = gameObject.transform.FindChild("MagButton").gameObject;

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
        selectId = buttonId;
    }
}
