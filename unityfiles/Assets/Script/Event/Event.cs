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

    //イベントが持つべき値
    //List<Handler>：イベントの本体
    //activateonwhat：起動条件-接触、自動など
    //onlyonce：一度きりかどうか


    //Handlerが持つべき値
    //イベントの種類(enum)
    //各イベントに使用する値

    public enum ActivateTYPE
    {
        自動 = 0,
        接触 = 1,
    }

    public List<Handler> eventlist = new List<Handler>();

    public ActivateTYPE activateonwhat;

    public bool onlyonce;

    private EventManagement eventmanager;

    //自動実行
    public void Start()
    {
        eventmanager = GameObject.Find("Event").GetComponent<EventManagement>();
        if (activateonwhat == ActivateTYPE.自動)
        {
            ActivateEvent();
        }
    }

    //接触イベント起動
    public void OnTriggerEnter(Collider collision)
    {
        if (activateonwhat == ActivateTYPE.接触)
        {
            if (collision.transform.tag == "MainCamera")
            {    
                ActivateEvent();
            }
        }
    }

    private void ActivateEvent()
    {
        eventmanager.StartEvent(this);
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
        custom.activateonwhat = (Event.ActivateTYPE)EditorGUILayout.EnumPopup("この時実行", custom.activateonwhat);
        if(custom.activateonwhat == Event.ActivateTYPE.接触)
        {
            //接触イベントならRigidBodyを追加
            if (custom.gameObject.GetComponent<BoxCollider>() == null)
            {
                custom.gameObject.AddComponent<BoxCollider>();
                custom.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            }
            if (custom.gameObject.GetComponent<Rigidbody>() == null)
            {
                custom.gameObject.AddComponent<Rigidbody>();
                custom.gameObject.GetComponent<Rigidbody>().useGravity = false;
                custom.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        //一度きりイベント
        custom.onlyonce = EditorGUILayout.Toggle("一度きり", custom.onlyonce);
        
        //クリックするまで待つか
        handler.waituntilclick = EditorGUILayout.Toggle("クリックまで待つ", handler.waituntilclick);

        //新しく追加するイベントの種類に応じて設定
        handler.type = (Handler.EVENTTYPE)EditorGUILayout.EnumPopup("イベントの種類", handler.type);

        if (handler.type == Handler.EVENTTYPE.WORD)
        {
            handler.text = EditorGUILayout.TextField("文章", handler.text);
        }
        if (handler.type == Handler.EVENTTYPE.TRANSITION)
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
        if (handler.type == Handler.EVENTTYPE.ENCOUNT)
        {
            handler.enemygroupID = EditorGUILayout.IntField("敵グループID", handler.enemygroupID);
        }
        if (handler.type == Handler.EVENTTYPE.MOVESCENE)
        {
            handler.movetothisscene = EditorGUILayout.TextField("移動シーン", handler.movetothisscene);
        }
        if (handler.type == Handler.EVENTTYPE.MOVEPOS)
        {
            EditorGUILayout.BeginHorizontal();
            handler.direction = (Handler.DIRECTION)EditorGUILayout.EnumPopup("方向", handler.direction);
            handler.moveX = EditorGUILayout.IntField("移動先X", handler.moveX);
            handler.moveY = EditorGUILayout.IntField("移動先Y", handler.moveY);
            EditorGUILayout.EndHorizontal();
            
            handler.direction = (Handler.DIRECTION)EditorGUILayout.EnumPopup("方向", handler.direction);
            handler.angle = (int)handler.direction * 90;
        }
        

        if (GUILayout.Button("追加"))
        {
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
            EditorGUILayout.EnumPopup("イベントの種類", custom.eventlist[i].type);
            if (custom.eventlist[i].type == Handler.EVENTTYPE.WORD)
            {
                EditorGUILayout.Toggle("クリックまで待つ", custom.eventlist[i].waituntilclick);
                EditorGUILayout.TextField("文章", custom.eventlist[i].text);
            }
            if (custom.eventlist[i].type == Handler.EVENTTYPE.TRANSITION)
            {
                EditorGUILayout.Toggle("クリックまで待つ", custom.eventlist[i].waituntilclick);
                EditorGUILayout.EnumPopup("", custom.eventlist[i].mode);
            }
            if(custom.eventlist[i].type == Handler.EVENTTYPE.ENCOUNT)
            {
                EditorGUILayout.Toggle("クリックまで待つ", custom.eventlist[i].waituntilclick);
                EditorGUILayout.IntField("敵グループ", custom.eventlist[i].enemygroupID);
            }
            if (custom.eventlist[i].type == Handler.EVENTTYPE.MOVESCENE)
            {
                EditorGUILayout.Toggle("クリックまで待つ", custom.eventlist[i].waituntilclick);
                EditorGUILayout.TextField("移動シーン", custom.eventlist[i].movetothisscene);
            }
            if (custom.eventlist[i].type == Handler.EVENTTYPE.MOVEPOS)
            {
                EditorGUILayout.Toggle("クリックまで待つ", custom.eventlist[i].waituntilclick);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.IntField("移動先X", custom.eventlist[i].moveX);
                EditorGUILayout.IntField("移動先Y", custom.eventlist[i].moveY);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUI.EndDisabledGroup();
        

        EditorUtility.SetDirty(target);
    }
}
#endif