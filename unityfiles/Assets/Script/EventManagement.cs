using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Variables;


public class EventManagement : MonoBehaviour {
    

    public void Execute (Handler activeevent)
    {
        if(activeevent.type == (int)Event.TYPE.WORD)
        {
            //ちゃんとしたキャンバスに張り付ける
            Debug.Log(activeevent.text);
        }
        if (activeevent.type == (int)Event.TYPE.TRANSITION)
        {
            if(activeevent.thisobject == false)
            Utility.mTransition(activeevent);

            if (activeevent.thisobject == true)
                Utility.mTransition(activeevent, activeevent.transobject);
         }
    }


}
