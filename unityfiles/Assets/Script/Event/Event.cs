using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    public enum TYPE
    {
        WORD = 0,
        TRANSITION = 1,
        ENCOUNT = 2,
        MOVESCENE = 3,
        MOVEPOS = 4,

    }
    public enum DIRECTION
    {
        UP = 0,
        RIGHT = 1,
        DOWN = 2, 
        LEFT = 3

    }
    public EVENTTYPE activateonwhat;

    public TYPE mode;

    public DIRECTION direction;

    private EventManagement eventmanager;

    //自動実行
    public void Start()
    {
        eventmanager = GameObject.Find("GameMaster").GetComponent<EventManagement>();
        if (activateonwhat == EVENTTYPE.自動)
        {
            ActivateEvent();
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
            }
        }
    }

    private void ActivateEvent()
    {
        eventmanager.StartEvent(eventlist);
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
        //クリックするまで待つか
        handler.waituntilclick = EditorGUILayout.Toggle("クリックまで待つ", handler.waituntilclick);
        //一度きりイベント
        handler.onlyonce = EditorGUILayout.Toggle("一度きり", handler.onlyonce);


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
            handler.enemygroupID = EditorGUILayout.IntField("敵グループID", handler.enemygroupID);
        }
        if (custom.mode == Event.TYPE.MOVESCENE)
        {
            handler.movetothisscene = EditorGUILayout.TextField("移動シーン", handler.movetothisscene);
        }
        if (custom.mode == Event.TYPE.MOVEPOS)
        {
            EditorGUILayout.BeginHorizontal();
            handler.moveX = EditorGUILayout.IntField("移動先X", handler.moveX);
            handler.moveY = EditorGUILayout.IntField("移動先Y", handler.moveY);
            EditorGUILayout.EndHorizontal();
            
            custom.direction = (Event.DIRECTION)EditorGUILayout.EnumPopup("方向", custom.direction);
            handler.direction = (int)custom.direction * 90;
        }
        

        if (GUILayout.Button("追加"))
        { 
            handler.type = (int)custom.mode;
            
            //list.Add(handler);
            //handler = new Handler();
            if(handler.onlyonce == true)
            {
                handler.obj = custom.gameObject;
            }

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
            if (custom.eventlist[i].type == 0)
            {
                custom.eventlist[i].text = EditorGUILayout.TextField("文章", custom.eventlist[i].text);
            }
            if (custom.eventlist[i].type == 1)
            {
                custom.eventlist[i].mode = (Transition.TRANSITION_MODE)EditorGUILayout.EnumPopup("", custom.eventlist[i].mode);
            }
            if (custom.eventlist[i].type == 3)
            {
                custom.eventlist[i].movetothisscene = EditorGUILayout.TextField("移動シーン", custom.eventlist[i].movetothisscene);
            }
            if (custom.eventlist[i].type == 4)
            {
                EditorGUILayout.BeginHorizontal();
                custom.eventlist[i].moveX = EditorGUILayout.IntField("移動先X", custom.eventlist[i].moveX);
                custom.eventlist[i].moveY = EditorGUILayout.IntField("移動先Y", custom.eventlist[i].moveY);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUI.EndDisabledGroup();
        

        EditorUtility.SetDirty(target);
    }
}
#endif