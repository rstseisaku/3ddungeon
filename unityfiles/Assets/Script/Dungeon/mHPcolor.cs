using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mHPcolor : MonoBehaviour {
    
    //残りHP(現在HP)後々どっかから取ってくる
    public int remainingHP;
    //最大HP後々どっかから取ってくる
    public int maxHP;
    //HPを文章として出すための文字列
    private string HP;
    //色
    private Color HPcolor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //文字列に変更
        HP = remainingHP.ToString() + " / \n " + maxHP.ToString();
        
        //HPの割合に応じて色を変更
        //50~100%：緑
        if ( (float)(remainingHP /  maxHP) >= 0.5 )
        {
            HPcolor = Color.green;
        }
        //25~50%：黄色
        else if ( (float)(remainingHP /  maxHP) >= 0.25 )
        {
            HPcolor = Color.yellow;
        }
        //0~25%：赤
        else
        {
            HPcolor = Color.red;
        }
    }

    void OnGUI() {
        this.gameObject.GetComponent<Text>().text = HP;
        this.gameObject.GetComponent<Text>().color = HPcolor;
    }
}
