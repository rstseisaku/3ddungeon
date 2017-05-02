using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventManagement : MonoBehaviour {
    

    public void Execute (Handler active)
    {
        if(active.type == (int)Event.TYPE.WORD)
        {
            Debug.Log(active.text);
        }
    }

}
