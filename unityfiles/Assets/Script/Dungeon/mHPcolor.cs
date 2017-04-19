using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mHPcolor : MonoBehaviour {

    public int RemainingHP;
    public int MaxHP;
    private string HP;
    private Color HPcolor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        HP = RemainingHP.ToString() + " / \n " + MaxHP.ToString();
        if ((float)RemainingHP / (float) MaxHP >= 0.5)
        {
            HPcolor = Color.green;
        }
        else if ((float)RemainingHP / (float) MaxHP >= 0.25)
        {
            HPcolor = Color.yellow;
        }
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
