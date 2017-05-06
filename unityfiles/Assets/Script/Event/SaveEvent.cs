using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SaveEvent : MonoBehaviour {

    string scenename;

    public struct enabledevent
    {
        public string eventname;
        public bool called;
    }

    public enabledevent[] eventlist;

	// Use this for initialization
	void Start () {
        scenename = SceneManager.GetActiveScene().name;

        if (System.IO.File.Exists(scenename + ".txt"))
        {
            //eventlist = System.IO.File.ReadAllLines(scenename + ".txt");
        }
        else
        {
            int num = this.gameObject.transform.childCount;
            eventlist = new enabledevent[num];
            int i = 0;
            foreach (Transform child in this.gameObject.transform)
            {
                eventlist[i].eventname = child.name;
                eventlist[i].called = false;
                Debug.Log(eventlist[i].eventname + "   " + eventlist[i].called);
            }
        }
		
	}
	

}
