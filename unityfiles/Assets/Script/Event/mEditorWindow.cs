using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Variables;
using TRANSITION;

#if UNITY_EDITOR

using UnityEditor;

//マップイベントのリスト表示
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

        //イベントの削除
        void RemoveEvent(List<GameObject> eventlist, int i)
        {
            DestroyImmediate(eventlist[i].gameObject);
            eventlist.Remove(eventlist[i]);
        }

    }

    //マップイベントの編集ウィンドウ
    public class EventContents : EditorWindow
    {
        public Event eventconfig;
        Vector2 scrollPosition;
        public bool folding;
        private int x = 0;

        Handler handler = new Handler();

        void OnGUI()
        {
            //ウィンドウサイズ固定
            maxSize = new Vector2(800, 400);
            minSize = new Vector2(800, 380);

            //イベントの種類(全イベント共通)
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("起動条件", GUILayout.Width(100));
            EditorGUILayout.LabelField("一度きり", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            eventconfig.activateonwhat = (Event.ActivateTYPE)EditorGUILayout.EnumPopup("", eventconfig.activateonwhat, GUILayout.Width(100));
            eventconfig.onlyonce = EditorGUILayout.Toggle("", eventconfig.onlyonce, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            //イベントの中身
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(320));
            int count = eventconfig.eventlist.Count;
            if (count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("種類", GUILayout.Width(100));
                EditorGUILayout.LabelField("コマンド", GUILayout.Width(96));
                EditorGUILayout.LabelField("上と同時実行", GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    eventconfig.eventlist[i].type = (Handler.EVENTTYPE)EditorGUILayout.EnumPopup("", eventconfig.eventlist[i].type, GUILayout.Width(100));
                    if (GUILayout.Button("編集", GUILayout.Width(32)))
                    {
                        x = i;
                    }
                    if (GUILayout.Button("挿入", GUILayout.Width(32)))
                    {
                        AddEvent(i);
                    }
                    if (GUILayout.Button("削除", GUILayout.Width(32)))
                    {
                        RemoveEvent(i);
                        count--;
                    }
                    eventconfig.eventlist[i].simultaneous = EditorGUILayout.Toggle("", eventconfig.eventlist[i].simultaneous, GUILayout.Width(64));

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndScrollView();
            
            if (GUILayout.Button("追加", GUILayout.Width(64)))
            {
                AddEvent();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            if (x >= 0 && x < eventconfig.eventlist.Count)
            {
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.WORD)
                {
                    EditorGUILayout.LabelField("文章", GUILayout.Width(72));
                    eventconfig.eventlist[x].text = EditorGUILayout.TextArea(eventconfig.eventlist[x].text, GUILayout.Width(374), GUILayout.Height(256));
                }
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.TRANSITION)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("ルール画像", GUILayout.Width(72));
                    EditorGUILayout.LabelField("時間(s)", GUILayout.Width(72));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    eventconfig.eventlist[x].rule = EditorGUILayout.ObjectField("", eventconfig.eventlist[x].rule, typeof(Texture2D), true, GUILayout.Width(64)) as Texture2D;
                    eventconfig.eventlist[x].time = EditorGUILayout.FloatField("", eventconfig.eventlist[x].time, GUILayout.Width(64));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("種類", GUILayout.Width(64));
                    if (eventconfig.eventlist[x].mode == Transition.TRANSITION_MODE._MASK)
                    {
                        EditorGUILayout.LabelField("マスク画像", GUILayout.Width(64));
                    }
                    if (eventconfig.eventlist[x].mode == Transition.TRANSITION_MODE._WHITEOUT)
                    {
                        EditorGUILayout.LabelField("whiteout", GUILayout.Width(64));
                    }
                    if (eventconfig.eventlist[x].mode == Transition.TRANSITION_MODE._BLACKOUT)
                    {
                        EditorGUILayout.LabelField("blackout", GUILayout.Width(64));
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    eventconfig.eventlist[x].mode = (Transition.TRANSITION_MODE)EditorGUILayout.EnumPopup("", eventconfig.eventlist[x].mode, GUILayout.Width(128));
                    if (eventconfig.eventlist[x].mode == Transition.TRANSITION_MODE._MASK)
                    {
                        eventconfig.eventlist[x].mask = EditorGUILayout.ObjectField("", eventconfig.eventlist[x].mask, typeof(Texture2D), true, GUILayout.Width(64)) as Texture2D;
                    }
                    if (eventconfig.eventlist[x].mode == Transition.TRANSITION_MODE._WHITEOUT)
                    {
                        eventconfig.eventlist[x].whiteout = EditorGUILayout.Slider("", eventconfig.eventlist[x].whiteout, 0, 1, GUILayout.Width(128));
                    }
                    if (eventconfig.eventlist[x].mode == Transition.TRANSITION_MODE._BLACKOUT)
                    {
                        eventconfig.eventlist[x].blackout = EditorGUILayout.Slider("", eventconfig.eventlist[x].blackout, 0, 1, GUILayout.Width(128));
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.LabelField("対象", GUILayout.Width(32));
                    eventconfig.eventlist[x].target = (Handler.TARGET)EditorGUILayout.EnumPopup("", eventconfig.eventlist[x].target, GUILayout.Width(72));
                    EditorGUILayout.LabelField("", GUILayout.Width(4));
                }
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.ENCOUNT)
                {
                    EditorGUILayout.LabelField("グループID", GUILayout.Width(72));
                    eventconfig.eventlist[x].enemygroupID = EditorGUILayout.IntField("", eventconfig.eventlist[x].enemygroupID, GUILayout.Width(64));
                    EditorGUILayout.LabelField("", GUILayout.Width(316));
                }
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.MOVESCENE)
                {
                    EditorGUILayout.LabelField("移動先シーン", GUILayout.Width(72));
                    eventconfig.eventlist[x].movetothisscene = EditorGUILayout.TextField("", eventconfig.eventlist[x].movetothisscene, GUILayout.Width(384));
                }
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.MOVEPOS)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("方向", GUILayout.Width(72));
                    EditorGUILayout.LabelField("移動先X", GUILayout.Width(72));
                    EditorGUILayout.LabelField("移動先Y", GUILayout.Width(72));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    eventconfig.eventlist[x].direction = (Handler.DIRECTION)EditorGUILayout.EnumPopup("", eventconfig.eventlist[x].direction, GUILayout.Width(64));
                    eventconfig.eventlist[x].angle = (int)eventconfig.eventlist[x].direction * 90;
                    eventconfig.eventlist[x].moveX = EditorGUILayout.IntField("", eventconfig.eventlist[x].moveX, GUILayout.Width(64));
                    eventconfig.eventlist[x].moveY = EditorGUILayout.IntField("", eventconfig.eventlist[x].moveY, GUILayout.Width(64));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.LabelField("", GUILayout.Width(312));
                }
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.PICTURE)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("対象", GUILayout.Width(72));
                    EditorGUILayout.LabelField("ピクチャ表示", GUILayout.Width(72));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    eventconfig.eventlist[x].target = (Handler.TARGET)EditorGUILayout.EnumPopup("", eventconfig.eventlist[x].target, GUILayout.Width(72));
                    eventconfig.eventlist[x].picture = EditorGUILayout.ObjectField("", eventconfig.eventlist[x].picture, typeof(Sprite), true, GUILayout.Width(64)) as Sprite;
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("X", GUILayout.Width(64));
                    EditorGUILayout.LabelField("Y", GUILayout.Width(64));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    eventconfig.eventlist[x].picX = EditorGUILayout.IntField("", eventconfig.eventlist[x].picX, GUILayout.Width(64));
                    eventconfig.eventlist[x].picY = EditorGUILayout.IntField("", eventconfig.eventlist[x].picY, GUILayout.Width(64));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("SizeX", GUILayout.Width(64));
                    EditorGUILayout.LabelField("SizeY", GUILayout.Width(64));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    eventconfig.eventlist[x].sizeX = EditorGUILayout.IntField("", eventconfig.eventlist[x].sizeX, GUILayout.Width(64));
                    eventconfig.eventlist[x].sizeY = EditorGUILayout.IntField("", eventconfig.eventlist[x].sizeY, GUILayout.Width(64));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.LabelField("", GUILayout.Width(12));
                }
             }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();
        }

        //イベントの追加
        void AddEvent()
        {
            eventconfig.eventlist.Add(handler);
            handler = new Handler();
        }
        //途中に挿入
        void AddEvent(int i)
        {
            eventconfig.eventlist.Insert(i, handler);
            handler = new Handler();
        }

        //イベントの削除
        void RemoveEvent(int i)
        {
            eventconfig.eventlist.Remove(eventconfig.eventlist[i]);
        }
    }
}

#endif