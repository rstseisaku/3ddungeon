using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {

	// Use this for initialization 
	void Start () {
        if (!Variables.__Debug.isDrawFPS) Destroy(this.gameObject);
        StartCoroutine("_FPS");
	}

    IEnumerator _FPS()
    {
        while (true)
        {
            GetComponent<Text>().text = "FPS: " + Utility._FPS.GetFPS();
            yield return 0;
        }
    }

}
