using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Variables;
using TRANSITION;

#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif


public class Event : MonoBehaviour {

    [SerializeField]
    public List<Handler> eventlist = new List<Handler>();
    public enum EVENTTYPE
    {
        自動 = 0,
        接触 = 1,
        調べる = 2
    }

    public enum TYPE
    {
        WORD = 0,
        TRANSITION = 1,
        ENCOUNT = 2,
        MOVESCENE = 3

    }
    public EVENTTYPE activateonwhat;

    public TYPE mode;

    private bool eventisactive = false;
    private int i = 0;

    //自動実行
    public void Start()
    {
        if(activateonwhat == EVENTTYPE.自動)
        {
            eventisactive = true;
            NextEvent(i);
            i++;
        }
    }

    //接触イベント起動
    public void OnTriggerEnter(Collider collision)
    {
        if (activateonwhat == EVENTTYPE.接触)
        {
            if (collision.transform.tag == "MainCamera")
            {
                ActivateEvent();
                NextEvent(i);
                i++;
            }
        }
    }

    private void ActivateEvent()
    {
        eventisactive = true;
 
    }

    //イベント送り
    private void NextEvent(int i)
    {
        GameObject.Find("GameMaster").GetComponent<EventManagement>().Execute(eventlist[i]);
    }
    
    public void Update()
    {
        if (eventisactive == true)
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                if (i < eventlist.Count)
                    NextEvent(i);
                i++;
            }
        }
    }
}



/* ---- ここから拡張コード ---- */
#if UNITY_EDITOR
/**
 * Inspector拡張クラス
 */
 
[CustomEditor(typeof(Event))]
public class CustomEvent : Editor
{
    Handler handler = new Handler();
    

    public override void OnInspectorGUI()
    {
        Event custom = target as Event;

        //イベントの起動条件
        custom.activateonwhat = (Event.EVENTTYPE)EditorGUILayout.EnumPopup("この時実行", custom.activateonwhat);

        //新しく追加するイベントの種類に応じて設定
        custom.mode = (Event.TYPE)EditorGUILayout.EnumPopup("種類", custom.mode);

        if (custom.mode == Event.TYPE.WORD)
        {
            handler.text = EditorGUILayout.TextField("文章", handler.text);
        }
        if (custom.mode == Event.TYPE.TRANSITION)
        {
            handler.rule = EditorGUILayout.ObjectField("ルール画像", handler.rule, typeof(Texture2D), true) as Texture2D;
            handler.time = EditorGUILayout.FloatField("時間(s)", handler.time);
            handler.mode = (Transition.TRANSITION_MODE)EditorGUILayout.EnumPopup("種類", handler.mode);

            if (handler.mode == Transition.TRANSITION_MODE._MASK)
            {
                handler.mask = EditorGUILayout.ObjectField("マスク画像", handler.mask, typeof(Texture2D), true) as Texture2D;
            }
            if (handler.mode == Transition.TRANSITION_MODE._WHITEOUT)
            {
                handler.whiteout = EditorGUILayout.Slider("whiteout", handler.whiteout, 0, 1);
            }
            if (handler.mode == Transition.TRANSITION_MODE._BLACKOUT)
            {
                handler.blackout = EditorGUILayout.Slider("blackout", handler.blackout, 0, 1);
            }
            if (handler.mode == Transition.TRANSITION_MODE._COLOR_INVERSION)
            {

            }
            if (handler.mode == Transition.TRANSITION_MODE._FADEIN)
            {

            }
            if (handler.mode == Transition.TRANSITION_MODE._FADEOUT)
            {

            }
            if (handler.mode == Transition.TRANSITION_MODE._GRAYSCALE)
            {

            }

            handler.thisobject = EditorGUILayout.Toggle("this object", handler.thisobject);
            if (handler.thisobject == true)
            {
                handler.transobject = custom.gameObject;
            }

        }
        if (custom.mode == Event.TYPE.ENCOUNT)
        {

        }
        if (custom.mode == Event.TYPE.MOVESCENE)
        {
            handler.moveto = EditorGUILayout.TextField("移動シーン", handler.moveto);
        }


        if (GUILayout.Button("追加"))
        { 
            handler.type = (int)custom.mode;
            
            //list.Add(handler);
            //handler = new Handler();

            custom.eventlist.Add(handler);
            handler = new Handler();
        }

        if (GUILayout.Button("削除"))
        {
            custom.eventlist.Clear();
        }
        //表示はするけど編集不可
        EditorGUI.BeginDisabledGroup(true);
        for (int i = 0; i < custom.eventlist.Count; i++)
        {
            custom.eventlist[i].type = EditorGUILayout.IntField("タイプ", custom.eventlist[i].type);
            if(custom.eventlist[i].type == 0)
                custom.eventlist[i].text = EditorGUILayout.TextField("文章", custom.eventlist[i].text);
            if (custom.eventlist[i].type == 1)
                custom.eventlist[i].mode = (Transition.TRANSITION_MODE)EditorGUILayout.EnumPopup("", custom.eventlist[i].mode);

            if (custom.eventlist[i].type == 3)
                custom.eventlist[i].moveto = EditorGUILayout.TextField("文章", custom.eventlist[i].moveto);
        }
        EditorGUI.EndDisabledGroup();
        

        EditorUtility.SetDirty(target);
    }
}
#endif