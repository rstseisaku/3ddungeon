using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

using Variables;
using TRANSITION;

public class mCustomWindow
{
    public class mEditorWindow : EditorWindow
    {

        // メニューのWindowにEditorExという項目を追加。
        [MenuItem("Window/イベント")]
        static void Open()
        {
            // メニューのWindow/EditorExを選択するとOpen()が呼ばれる。
            // 表示させたいウィンドウは基本的にGetWindow()で表示＆取得する。
            EditorWindow.GetWindow<mEditorWindow>("イベントリスト"); // タイトル名を"EditorEx"に指定（後からでも変えられるけど）

        }

        // Windowのクライアント領域のGUI処理を記述
        void OnGUI()
        {
            // 試しにラベルを表示
            EditorGUILayout.LabelField("ようこそ！　Unityエディタ拡張の沼へ！");

            //全イベントを格納しているキャンバスの取得
            GameObject eventcanvas = GameObject.Find("Event");
            int eventnum = eventcanvas.transform.childCount;
            //イベントキャンバスが保持している全イベントを取得
            List<GameObject> eventlist = new List<GameObject>();
            foreach (Transform child in eventcanvas.transform)
            {
                GameObject temp = child.gameObject;
                eventlist.Add(temp);
            }

            EditorGUILayout.BeginVertical();
            for (int i = 0; i < eventnum; i++)
            {
                EditorGUILayout.BeginHorizontal();
                eventlist[i].isStatic = EditorGUILayout.Toggle("ロック", eventlist[i].isStatic);
                if (eventlist[i].isStatic == true)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }
                EditorGUILayout.ObjectField(eventlist[i], typeof(GameObject), true);
                if (GUILayout.Button("変更"))
                {
                    EditorWindow.GetWindow<EventContents>("イベント内容").eventconfig = eventlist[i].GetComponent<Event>();
                }
                if (GUILayout.Button("削除"))
                {
                    RemoveEvent(eventlist, i);
                    eventnum--;
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            if (GUILayout.Button("追加"))
            {
                AddNewEvent(eventlist, eventcanvas);
            }
        }

        //新規イベント(GameObject)の追加
        void AddNewEvent(List<GameObject> eventlist, GameObject eventcanvas)
        {
            //新しいオブジェクトの作成(イベントスクリプトをアタッチ)
            GameObject temp = new GameObject();
            temp.AddComponent<Event>();
            //イベントキャンバスの子オブジェクトとして設定
            temp.transform.SetParent(eventcanvas.transform);
            //イベント名を変更(デフォルト:(マップ名)event(n))
            string mapname = SceneManager.GetActiveScene().name;
            int eventnum = eventcanvas.transform.childCount;
            temp.name = mapname + "event" + eventnum.ToString();
            eventlist.Add(temp);
        }

        void RemoveEvent(List<GameObject> eventlist, int i)
        {
            DestroyImmediate(eventlist[i].gameObject);
            eventlist.Remove(eventlist[i]);
        }

    }

    public class EventContents : EditorWindow
    {
        public Event eventconfig;
        Vector2 scrollPosition;
        public bool folding;

        Handler handler = new Handler();

        void OnGUI()
        {
            eventconfig.activateonwhat = (Event.ActivateTYPE)EditorGUILayout.EnumPopup("起動条件", eventconfig.activateonwhat);
            eventconfig.onlyonce = EditorGUILayout.Toggle("一度きり", eventconfig.onlyonce);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(320));
            int count = eventconfig.eventlist.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    eventconfig.eventlist[i].type = (Handler.EVENTTYPE)EditorGUILayout.EnumPopup("種類", eventconfig.eventlist[i].type, GUILayout.Width(256));
                    if (GUILayout.Button("削除", GUILayout.Width(64)))
                    {
                        RemoveEvent(i);
                        count--;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (folding = EditorGUILayout.Foldout(folding, "隠す"))
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.WORD)
                        {
                            eventconfig.eventlist[i].text = EditorGUILayout.TextField("文章", eventconfig.eventlist[i].text, GUILayout.Width(384));
                        }
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.TRANSITION)
                        {
                            eventconfig.eventlist[i].rule = EditorGUILayout.ObjectField("ルール画像", eventconfig.eventlist[i].rule, typeof(Texture2D), true) as Texture2D;
                            EditorGUILayout.BeginVertical();
                            eventconfig.eventlist[i].mode = (Transition.TRANSITION_MODE)EditorGUILayout.EnumPopup("種類", eventconfig.eventlist[i].mode);
                            eventconfig.eventlist[i].time = EditorGUILayout.FloatField("時間(s)", eventconfig.eventlist[i].time);
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._MASK)
                            {
                                eventconfig.eventlist[i].mask = EditorGUILayout.ObjectField("マスク画像", eventconfig.eventlist[i].mask, typeof(Texture2D), true) as Texture2D;
                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._WHITEOUT)
                            {
                                eventconfig.eventlist[i].whiteout = EditorGUILayout.Slider("whiteout", eventconfig.eventlist[i].whiteout, 0, 1);
                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._BLACKOUT)
                            {
                                eventconfig.eventlist[i].blackout = EditorGUILayout.Slider("blackout", eventconfig.eventlist[i].blackout, 0, 1);
                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._COLOR_INVERSION)
                            {

                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._FADEIN)
                            {

                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._FADEOUT)
                            {

                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._GRAYSCALE)
                            {

                            }
                            EditorGUILayout.EndVertical();
                        }
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.ENCOUNT)
                        {
                            eventconfig.eventlist[i].enemygroupID = EditorGUILayout.IntField("敵グループID", eventconfig.eventlist[i].enemygroupID, GUILayout.Width(256));
                        }
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.MOVESCENE)
                        {
                            eventconfig.eventlist[i].movetothisscene = EditorGUILayout.TextField("シーン移動", eventconfig.eventlist[i].movetothisscene, GUILayout.Width(384));
                        }
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.MOVEPOS)
                        {
                            EditorGUILayout.BeginVertical();
                            eventconfig.eventlist[i].direction = (Handler.DIRECTION)EditorGUILayout.EnumPopup("方向", eventconfig.eventlist[i].direction, GUILayout.Width(256));
                            eventconfig.eventlist[i].angle = (int)eventconfig.eventlist[i].direction * 90;
                            eventconfig.eventlist[i].moveX = EditorGUILayout.IntField("移動先X", eventconfig.eventlist[i].moveX, GUILayout.Width(256));
                            eventconfig.eventlist[i].moveY = EditorGUILayout.IntField("移動先Y", eventconfig.eventlist[i].moveY, GUILayout.Width(256));
                            EditorGUILayout.EndVertical();
                        }
                        eventconfig.eventlist[i].waituntilclick = EditorGUILayout.Toggle("クリックまで待つ", eventconfig.eventlist[i].waituntilclick);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }                   
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("追加", GUILayout.Width(64)))
            {
                eventconfig.eventlist.Add(handler);
                handler = new Handler();
            }
        }

        void RemoveEvent(int i)
        {
            eventconfig.eventlist.Remove(eventconfig.eventlist[i]);
        }
    }
}