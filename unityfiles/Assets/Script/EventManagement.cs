using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Variables;
using Utility;


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
            _Transition.mTransition(activeevent);

            if (activeevent.thisobject == true)
                _Transition.mTransition(activeevent, activeevent.transobject);
         }
    }


}
