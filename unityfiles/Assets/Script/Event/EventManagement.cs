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
    private GameObject canvas;
    private GameObject textcanvas;
    private GameObject imagecanvas;
    private GameObject[] picture = new GameObject[5];
    private bool scroll = false;

    //
    private GameObject gamemaster;
    
    //移動等のボタンを封じるために取得
    private GameObject buttoncanvas;
    private bool istherebuttons = false;
    
    //受け取ったイベントのリストをコピー
    private List<Handler> eventlist;

    //実行中のイベント
    private Event runningevent;

    //イベント名(イベントを持っているGameObjectの名前)
    private string eventname;

    public void Awake()
    {
        //他のボタン無効にするため
        if (GameObject.Find("ButtonCanvas"))
        {
            buttoncanvas = GameObject.Find("ButtonCanvas");
            istherebuttons = true;
        }

        //基本的にイベント用のキャンバスは非表示にしておく
        canvas = GameObject.Find("Canvas");
        textcanvas = canvas.transform.Find("TextCanvas").gameObject;
        imagecanvas = canvas.transform.Find("ImageCanvas").gameObject;
        textcanvas.gameObject.SetActive(false);
        imagecanvas.gameObject.SetActive(false);
        for (int i = 0; i < 5; i++)
        {
            picture[i] = imagecanvas.transform.Find((i+1).ToString()).gameObject;
            picture[i].SetActive(false);
        }

        //プレイヤーの動きを強制的に止めたりするのに使う
        if (GameObject.Find("GameMaster"))
        {
            gamemaster = GameObject.Find("GameMaster");
        }
    }
    
    //イベント実行関数
    public IEnumerator Execute (Handler activeevent)
    {
        //イベントの種類に応じて実行
        if(activeevent.type == Handler.EVENTTYPE.WORD)
        {
            //文章表示ならテキストウィンドウを有効化
            if (activeevent.text != "")
            {
                textcanvas.SetActive(true);
                textcanvas.transform.Find("Frame").Find("Text").GetComponent<Text>().text = activeevent.text;

                //アイコンが設定されているなら表示
                textcanvas.transform.Find("Frame").Find("Icon").GetComponent<Image>().sprite = activeevent.icon;
                if (activeevent.icon != null)
                {
                    textcanvas.transform.Find("Frame").Find("Icon").gameObject.SetActive(true);
                    textcanvas.transform.Find("Frame").Find("Text").GetComponent<RectTransform>().localPosition = new Vector2(180,0);
                }
                else
                {
                    textcanvas.transform.Find("Frame").Find("Icon").gameObject.SetActive(false);
                    textcanvas.transform.Find("Frame").Find("Text").GetComponent<RectTransform>().localPosition = new Vector2(0,0);
                }
            }
            //空のテキストならテキストウィンドウを無効化
            if (activeevent.text == "")
            {
                textcanvas.SetActive(false);
            }
        }

        if (activeevent.type == Handler.EVENTTYPE.TRANSITION)
        {
            //画面全体に対するトランジション
            if (activeevent.target == Handler.TARGET.SCREEN)
                _Transition.mTransition(activeevent);
            //PICTURE1~5に対するトランジション
            else
                _Transition.mTransition(activeevent, picture[(int)activeevent.target - 1]);
    }
        if (activeevent.type == Handler.EVENTTYPE.ENCOUNT)
        {
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
            Map.SetPlayer((int)Map.movehere.x, (int)Map.movehere.y, Map.direction);
            gamemaster.GetComponent<DungeonMaster>().StopMove();
        }
        if (activeevent.type == Handler.EVENTTYPE.PICTURE)
        {
            imagecanvas.SetActive(true);

            picture[(int)activeevent.target - 1].SetActive(true);
            picture[(int)activeevent.target - 1].GetComponent<Image>().sprite = activeevent.picture;
            picture[(int)activeevent.target - 1].transform.localPosition = new Vector2(activeevent.picX, activeevent.picY);
            picture[(int)activeevent.target - 1].GetComponent<RectTransform>().sizeDelta = new Vector2(activeevent.sizeX, activeevent.sizeY);
        }
        
        yield return 0;
    }

    //イベント開始処理
    public void StartEvent(Event receivedevent)
    {
        //実行中イベント=受け取ったイベント
        runningevent = receivedevent;
        //イベントリスト=受け取ったイベントの中身
        eventlist = runningevent.eventlist;
        //イベント名(GameObject)名取得
        //一度きりイベントの無効化用
        eventname = runningevent.gameObject.name;
        //イベント実行中は他のボタンの無効化
        buttoncanvas.gameObject.SetActive(false);
        //ボタン押した判定のリセット
        scroll = false;
        //イベント実行
        Debug.Log("イベント開始");
        StartCoroutine(mUpdate());
    }

    //イベント終了処理
    public void EndEvent()
    {
        //他のボタンの有効化
        buttoncanvas.gameObject.SetActive(true);
        //イベント用ボタン、テキスト等の無効化
        textcanvas.SetActive(false);
        //一度きりイベントなら呼ばれないようにする
        if(runningevent.onlyonce == true)
        {
            this.gameObject.GetComponent<SaveEvent>().DisableEvent(eventname);
        }
        Debug.Log("イベント終了");
    }

    private IEnumerator mUpdate()
    {
        //iの初期化
        int i = 0;
        while (true)
        {
            //一番最初のイベントは即実行
            if (i == 0)
            {
                yield return NextEvent(0);
                i++;
            }
            //2番目以降
            if (i < eventlist.Count)
            {
                //上と同時に実行するイベント
                if (eventlist[i].simultaneous == true)
                {
                    yield return NextEvent(i);
                    i++;
                }
                //それ以外のイベントはボタンが押されるまで待機
                if (scroll == true)
                {
                    yield return NextEvent(i);
                    i++;
                    scroll = false;
                }
            }
            //全イベント完了
            if (i >= eventlist.Count)
            {
                //終了条件　変えるべき？
                if (scroll == true)
                {
                    EndEvent();
                    yield break;
                }
            }
            yield return 0;
        }
    }

    //次のイベント読み込み
    private IEnumerator NextEvent(int i)
    {
        yield return Execute(eventlist[i]);
    }

    //ボタンが押された判定(テキスト読み進めたり)
    public void OnClick()
    {
        scroll = true;
    }
}

