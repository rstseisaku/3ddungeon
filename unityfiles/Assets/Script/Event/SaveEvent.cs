using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SaveEvent : MonoBehaviour {

    string filepath = "Assets\\Resources\\MapData\\Event\\";
    string scenename;

    public struct enabledevent
    {
        public GameObject eventobject;
        public string eventname;
        public bool called;
    }

    public enabledevent[] eventlist;
    int eventnum;

    // Use this for initialization
    public void Start () {
        scenename = SceneManager.GetActiveScene().name;

        if (System.IO.File.Exists(filepath + scenename + ".txt"))
        {
            string[] buffer = System.IO.File.ReadAllLines(filepath + scenename + ".txt");
            string[] linebuffer;

            eventlist = new enabledevent[buffer.Length];
            eventnum = buffer.Length;

            for (int i = 0; i < eventnum; i++)
            {
                linebuffer = buffer[i].Split(',');
                eventlist[i].eventname = linebuffer[0];
                if (linebuffer[1] == "True")
                    eventlist[i].called = true;
                if (linebuffer[1] == "False")
                    eventlist[i].called = false;
                eventlist[i].eventobject = GameObject.Find(eventlist[i].eventname);
                if(eventlist[i].called == true)
                {
                    eventlist[i].eventobject.SetActive(false);
                }
                
            }
            
        }
        else
        {
            eventnum = this.gameObject.transform.childCount;
            eventlist = new enabledevent[eventnum];
            int i = 0;
            foreach (Transform child in this.gameObject.transform)
            {
                eventlist[i].eventobject = child.gameObject;
                eventlist[i].eventname = child.name;
                eventlist[i].called = false;
                i++;
            }
            SaveState();
        }
		
	}


    public void DisableEvent(string eventname)
    {
        for (int i = 0; i < eventnum; i++)
        {
            if(eventlist[i].eventname == eventname)
            {
                eventlist[i].called = true;
                eventlist[i].eventobject.SetActive(false);
            }
        }
        SaveState();
    }

    public void SaveState()
    {
        string[] writefile = new string[eventnum];
        for(int i = 0; i < eventnum; i++)
        {
            writefile[i] = eventlist[i].eventname;
            writefile[i] += ",";
            if (eventlist[i].called == true)
                writefile[i] += true;
            if (eventlist[i].called == false)
                writefile[i] += false;
        }
        if (System.IO.File.Exists(filepath + scenename + ".txt"))
            System.IO.File.Delete(filepath + scenename + ".txt");
        System.IO.File.WriteAllLines((filepath + scenename + ".txt"), writefile);

    }

}
