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
            //ラベルを表示
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ロック", GUILayout.Width(100));
            EditorGUILayout.LabelField("イベント名", GUILayout.Width(200));
            EditorGUILayout.LabelField("コマンド", GUILayout.Width(100));

            EditorGUILayout.EndHorizontal();
            
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
                eventlist[i].isStatic = EditorGUILayout.Toggle("", eventlist[i].isStatic, GUILayout.Width(100));
                if (eventlist[i].isStatic == true)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }
                EditorGUILayout.ObjectField(eventlist[i], typeof(GameObject), true, GUILayout.Width(200));
                if (GUILayout.Button("編集"))
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
            
            if (GUILayout.Button("追加", GUILayout.Width(100)))
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
            //ウィンドウサイズ固定
            maxSize = new Vector2(800, 400);
            minSize = new Vector2(800, 380);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("起動条件", GUILayout.Width(100));
            EditorGUILayout.LabelField("一度きり", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            eventconfig.activateonwhat = (Event.ActivateTYPE)EditorGUILayout.EnumPopup("", eventconfig.activateonwhat, GUILayout.Width(100));
            eventconfig.onlyonce = EditorGUILayout.Toggle("", eventconfig.onlyonce, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(320));
            int count = eventconfig.eventlist.Count;
            if (count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("種類", GUILayout.Width(100));
                EditorGUILayout.LabelField("コマンド", GUILayout.Width(64));
                EditorGUILayout.LabelField("詳細", GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    eventconfig.eventlist[i].type = (Handler.EVENTTYPE)EditorGUILayout.EnumPopup("", eventconfig.eventlist[i].type, GUILayout.Width(100));
                    if (GUILayout.Button("挿入", GUILayout.Width(32)))
                    {
                        //リストの途中に挿入
                    }
                    if (GUILayout.Button("削除", GUILayout.Width(32)))
                    {
                        RemoveEvent(i);
                        count--;
                    }
                    if (folding = EditorGUILayout.Foldout(folding, ""))
                    {
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.WORD)
                        {
                            EditorGUILayout.LabelField("文章", GUILayout.Width(72));
                            eventconfig.eventlist[i].text = EditorGUILayout.TextField("", eventconfig.eventlist[i].text, GUILayout.Width(384));
                        }
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.TRANSITION)
                        {
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("ルール画像", GUILayout.Width(72));
                            EditorGUILayout.LabelField("時間(s)", GUILayout.Width(72));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            eventconfig.eventlist[i].rule = EditorGUILayout.ObjectField("", eventconfig.eventlist[i].rule, typeof(Texture2D), true, GUILayout.Width(64)) as Texture2D;
                            eventconfig.eventlist[i].time = EditorGUILayout.FloatField("", eventconfig.eventlist[i].time, GUILayout.Width(64));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("種類", GUILayout.Width(64));
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._MASK)
                            {
                                EditorGUILayout.LabelField("マスク画像", GUILayout.Width(64));
                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._WHITEOUT)
                            {
                                EditorGUILayout.LabelField("whiteout", GUILayout.Width(64));
                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._BLACKOUT)
                            {
                                EditorGUILayout.LabelField("blackout", GUILayout.Width(64));
                            }
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            eventconfig.eventlist[i].mode = (Transition.TRANSITION_MODE)EditorGUILayout.EnumPopup("", eventconfig.eventlist[i].mode, GUILayout.Width(128));
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._MASK)
                            {
                                eventconfig.eventlist[i].mask = EditorGUILayout.ObjectField("", eventconfig.eventlist[i].mask, typeof(Texture2D), true, GUILayout.Width(64)) as Texture2D;
                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._WHITEOUT)
                            {
                                eventconfig.eventlist[i].whiteout = EditorGUILayout.Slider("", eventconfig.eventlist[i].whiteout, 0, 1, GUILayout.Width(128));
                            }
                            if (eventconfig.eventlist[i].mode == Transition.TRANSITION_MODE._BLACKOUT)
                            {
                                eventconfig.eventlist[i].blackout = EditorGUILayout.Slider("", eventconfig.eventlist[i].blackout, 0, 1, GUILayout.Width(128));
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.LabelField("対象", GUILayout.Width(32));
                            eventconfig.eventlist[i].target = (Handler.TARGET)EditorGUILayout.EnumPopup("", eventconfig.eventlist[i].target, GUILayout.Width(72));
                            EditorGUILayout.LabelField("", GUILayout.Width(4));
                        }
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.ENCOUNT)
                        {
                            EditorGUILayout.LabelField("グループID", GUILayout.Width(72));
                            eventconfig.eventlist[i].enemygroupID = EditorGUILayout.IntField("", eventconfig.eventlist[i].enemygroupID, GUILayout.Width(64));
                            EditorGUILayout.LabelField("", GUILayout.Width(316));
                        }
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.MOVESCENE)
                        {
                            EditorGUILayout.LabelField("移動先シーン", GUILayout.Width(72));
                            eventconfig.eventlist[i].movetothisscene = EditorGUILayout.TextField("", eventconfig.eventlist[i].movetothisscene, GUILayout.Width(384));
                        }
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.MOVEPOS)
                        {
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("方向", GUILayout.Width(72));
                            EditorGUILayout.LabelField("移動先X", GUILayout.Width(72));
                            EditorGUILayout.LabelField("移動先Y", GUILayout.Width(72));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            eventconfig.eventlist[i].direction = (Handler.DIRECTION)EditorGUILayout.EnumPopup("", eventconfig.eventlist[i].direction, GUILayout.Width(64));
                            eventconfig.eventlist[i].angle = (int)eventconfig.eventlist[i].direction * 90;
                            eventconfig.eventlist[i].moveX = EditorGUILayout.IntField("", eventconfig.eventlist[i].moveX, GUILayout.Width(64));
                            eventconfig.eventlist[i].moveY = EditorGUILayout.IntField("", eventconfig.eventlist[i].moveY, GUILayout.Width(64));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.LabelField("", GUILayout.Width(312));
                        }
                        if (eventconfig.eventlist[i].type == Handler.EVENTTYPE.PICTURE)
                        {
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("対象", GUILayout.Width(72));
                            EditorGUILayout.LabelField("ピクチャ表示", GUILayout.Width(72));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            eventconfig.eventlist[i].target = (Handler.TARGET)EditorGUILayout.EnumPopup("", eventconfig.eventlist[i].target, GUILayout.Width(72));
                            eventconfig.eventlist[i].picture = EditorGUILayout.ObjectField("", eventconfig.eventlist[i].picture, typeof(Sprite), true, GUILayout.Width(64)) as Sprite;
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("X", GUILayout.Width(64));
                            EditorGUILayout.LabelField("Y", GUILayout.Width(64));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            eventconfig.eventlist[i].picX = EditorGUILayout.IntField("", eventconfig.eventlist[i].picX, GUILayout.Width(64));
                            eventconfig.eventlist[i].picY = EditorGUILayout.IntField("", eventconfig.eventlist[i].picY, GUILayout.Width(64));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.LabelField("SizeX", GUILayout.Width(64));
                            EditorGUILayout.LabelField("SizeY", GUILayout.Width(64));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            eventconfig.eventlist[i].sizeX = EditorGUILayout.IntField("", eventconfig.eventlist[i].sizeX, GUILayout.Width(64));
                            eventconfig.eventlist[i].sizeY = EditorGUILayout.IntField("", eventconfig.eventlist[i].sizeY, GUILayout.Width(64));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.LabelField("", GUILayout.Width(12));
                        }
                        EditorGUILayout.LabelField("上と同時", GUILayout.Width(64));
                        eventconfig.eventlist[i].simultaneous = EditorGUILayout.Toggle("", eventconfig.eventlist[i].simultaneous, GUILayout.Width(64));
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndScrollView();
            
            //EditorGUILayout.Space(30)
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