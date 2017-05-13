using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Variables;
using Utility;

//イベントを受け取るスクリプト
//GameMasterにでも張り付けて使用
public class EventManagement : MonoBehaviour {

    //テキスト描画用キャンバス取得
    GameObject textcanvas;
    private bool scroll = false;
    
    private GameObject buttoncanvas;

    private List<Handler> eventlist;

    private Event runningevent;

    private string eventname;

  /*  public void Start()
    {
        buttoncanvas = GameObject.Find("ButtonCanvas");
        textcanvas = GameObject.Find("TextCanvas");
        textcanvas.gameObject.SetActive(false);
    }*/
    public void Awake()
    {
        buttoncanvas = GameObject.Find("ButtonCanvas");
        textcanvas = GameObject.Find("TextCanvas");
        textcanvas.gameObject.SetActive(false);
    }

    //イベント実行関数
    public IEnumerator Execute (Handler activeevent)
    {
        //イベントの種類に応じて実行
        if(activeevent.type == Handler.EVENTTYPE.WORD)
        {
            //ちゃんとしたキャンバスに張り付ける
            textcanvas.SetActive(true);
            textcanvas.transform.FindChild("Conversation").GetComponent<Text>().text = activeevent.text;
        }
        if (activeevent.type == Handler.EVENTTYPE.TRANSITION)
        {
            //このオブジェクトに対するトランジション
            if(activeevent.thisobject == false)
            _Transition.mTransition(activeevent);
            //画面全体に対するトランジション
            if (activeevent.thisobject == true)
                _Transition.mTransition(activeevent, activeevent.transobject);
         }
        if (activeevent.type == Handler.EVENTTYPE.ENCOUNT)
        {
            //EndEvent();
            StartCoroutine(_Encount.Encount(activeevent.enemygroupID));
        }
        if (activeevent.type == Handler.EVENTTYPE.MOVESCENE)
        {
            StartCoroutine(_Scene.MoveScene(activeevent.movetothisscene));
        }
        if (activeevent.type == Handler.EVENTTYPE.MOVEPOS)
        {
            Map.direction = activeevent.angle;
            Map.movehere = new Vector2(activeevent.moveX, activeevent.moveY);
            //移動した後で一歩進んでいるため無理やり一歩戻す
            //Vector2 offset = new Vector2(Mathf.Sin(activeevent.angle * 2 * Mathf.PI / 360),
              //                           Mathf.Cos(activeevent.angle * 2 * Mathf.PI / 360));
            //Map.movehere -= offset;
            Map.SetPlayer((int)Map.movehere.x, (int)Map.movehere.y, Map.direction);
            GameObject.Find("GameMaster").GetComponent<DungeonMaster>().StopMove();
        }
        yield return 0;
        

    }

    public void StartEvent(Event receivedevent)
    {
        runningevent = receivedevent;

        eventlist = runningevent.eventlist;

        eventname = runningevent.gameObject.name;

        buttoncanvas.gameObject.SetActive(false);

        StartCoroutine(mUpdate());
    }

    public void EndEvent()
    {
        Debug.Log("called");
        buttoncanvas.gameObject.SetActive(true);
        textcanvas.SetActive(false);
        if(runningevent.onlyonce == true)
        {
            this.gameObject.GetComponent<SaveEvent>().DisableEvent(eventname);
        }
        scroll = false;
        Debug.Log("イベント終了");
    }

    private IEnumerator mUpdate()
    {
        int i = 0;
        while (true)
        {
            if (i == 0)
            {
                yield return NextEvent(0);
                i++;
            }
            if (i < eventlist.Count)
            {
                if (eventlist[i].waituntilclick == false)
                {
                    yield return NextEvent(i);
                    i++;
                }
                if (scroll == true)
                {
                    yield return NextEvent(i);
                    i++;
                    scroll = false;
                }

            }

            if (i >= eventlist.Count)
            {
                if (scroll == true)
                {
                    EndEvent();
                    yield break;
                }
            }
            yield return 0;
        }
    }

    private IEnumerator NextEvent(int i)
    {
        yield return Execute(eventlist[i]);
    }

    public void OnClick()
    {
        scroll = true;
    }
}
