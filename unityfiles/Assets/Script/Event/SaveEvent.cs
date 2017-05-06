using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SaveEvent : MonoBehaviour {

    string scenename;

    string[] eventlist;

	// Use this for initialization
	void Start () {
        scenename = SceneManager.GetActiveScene().name;

        if (System.IO.File.Exists(scenename + ".txt"))
        {
            eventlist = System.IO.File.ReadAllLines(scenename + ".txt");
        }
        else
        {
            int num = this.gameObject.transform.childCount;
            eventlist = new string[num];
            int i = 0;
            foreach (Transform child in this.gameObject.transform)
            {
                eventlist[i] = child.name;
            }
            Debug.Log(num);
        }
		
	}
	

}
