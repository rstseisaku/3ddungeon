using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAction : MonoBehaviour {
    public int selectId;

	// Use this for initialization
	void Start () {
        selectId = -1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void myOnClick( int buttonId )
    {
        // 押された ID を格納する
        selectId = buttonId;
    }
}
