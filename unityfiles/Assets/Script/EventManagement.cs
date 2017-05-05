using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Variables;
using Utility;

//イベントを受け取るスクリプト
//GameMasterにでも張り付けて使用
public class EventManagement : MonoBehaviour {
    //イベント実行関数
    public void Execute (Handler activeevent)
    {
        //イベントの種類に応じて実行
        if(activeevent.type == (int)Event.TYPE.WORD)
        {
            //ちゃんとしたキャンバスに張り付ける
            Debug.Log(activeevent.text);
        }
        if (activeevent.type == (int)Event.TYPE.TRANSITION)
        {
            //このオブジェクトに対するトランジション
            if(activeevent.thisobject == false)
            _Transition.mTransition(activeevent);
            //画面全体に対するトランジション
            if (activeevent.thisobject == true)
                _Transition.mTransition(activeevent, activeevent.transobject);
         }
        if (activeevent.type == (int)Event.TYPE.ENCOUNT)
        {
        }
        if (activeevent.type == (int)Event.TYPE.MOVESCENE)
        {
            Debug.Log("fuck");
            StartCoroutine(_Scene.MoveScene(activeevent.moveto));
        }
    }
}
