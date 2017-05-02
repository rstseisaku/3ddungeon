using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif

using fuck;

public class Event : MonoBehaviour {
    
    [SerializeField]
    public List<Handler> handler = new List<Handler>();
    public enum TYPE
    {
        WORD = 0,
        TRANSITION = 1,
        ENCOUNT = 2

    }
    public TYPE mode;

    int i = 0;
    public Transition test;

    void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.tag == "MainCamera")
        {
            Debug.Log("collision");
            //ActivateEvent();
        }
    }

    void ActivateEvent(int i)
    {
        Debug.Log(i);
         GameObject.Find("GameMaster").GetComponent<EventManagement>().Execute(handler[i]);
 
    }

    

    public void Update() {
        
        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (i < handler.Count)
                ActivateEvent(i);
            i++;
        }
    }
}

[System.Serializable]
public class Handler
{
    public int type;
    public string text;
    public Transition transition = new Transition();

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

        serializedObject.Update();

        //新しく追加するイベントの種類に応じて設定
        //custom.mode = (Event.TYPE)EditorGUILayout.EnumPopup("種類", custom.mode);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mode"), new GUIContent("Event Type"));

        if (custom.mode == Event.TYPE.WORD)
        {
            handler.text = EditorGUILayout.TextField("文章", handler.text);
        }
        if (custom.mode == Event.TYPE.TRANSITION)
        {
            handler.transition.rule = EditorGUILayout.ObjectField("ルール画像", handler.transition.rule, typeof(Texture2D), true) as Texture2D;
            handler.transition.time = EditorGUILayout.FloatField("時間(s)", handler.transition.time);
            handler.transition.mode = (Transition.TRANSITION_MODE)EditorGUILayout.EnumPopup("種類", handler.transition.mode);

        }
        if (custom.mode == Event.TYPE.ENCOUNT)
        {

        }



        if (GUILayout.Button("追加"))
        { 
            handler.type = (int)custom.mode;
            
            custom.handler.Add(handler);
            handler = new Handler();
        }

        if (GUILayout.Button("削除"))
        {
            custom.handler.Clear();
        }
        //表示はするけど編集不可
        EditorGUI.BeginDisabledGroup(true);
        for (int i = 0; i < custom.handler.Count; i++)
        {
            custom.handler[i].type = EditorGUILayout.IntField("タイプ", custom.handler[i].type);
            if(custom.handler[i].type == 0)
            custom.handler[i].text = EditorGUILayout.TextField("文章", custom.handler[i].text);
            //if (custom.handler[i].type == 1)
            //custom.handler[i].transition.mode = (Transition.TRANSITION_MODE)EditorGUILayout.EnumPopup("",custom.handler[i].transition.mode);
        }
        EditorGUI.EndDisabledGroup();


        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(target);
    }
}
#endif