using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Variables;
using TRANSITION;

#if UNITY_EDITOR

using UnityEditor;


/*マップイベントの編集スクリプト*/
public class mCustomWindow
{
    /*マップイベントのリスト表示*/
    public class mEditorWindow : EditorWindow
    {
        /*仕様*/
        /*
         　ロック   イベント名   コマンド
            ☑       eventname   編集　削除
            .           .           .   .
            .           .           .   . 


           追加
        */

        /*役割*/
        /*
          ロック      編集不可の設定(完成したイベントを削除しないように)
          イベント名  イベントのGameObject名、なるべくわかりやすいものにするべし
          コマンド  　選択したイベントに対して行う動作、編集－イベント内容の設定、変更
                                                    削除－選択したマップイベントの削除
          追加        新しいマップイベントの追加
        */
        
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
            /*ラベルを表示*/
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ロック", GUILayout.Width(100));
            EditorGUILayout.LabelField("イベント名", GUILayout.Width(200));
            EditorGUILayout.LabelField("コマンド", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            
            /*全イベントを格納しているキャンバスの取得*/
            GameObject eventcanvas = GameObject.Find("MapEvent");
            int eventnum = eventcanvas.transform.childCount;
            /**/

            /*イベントキャンバスが保持している全イベントを取得*/
            List<GameObject> eventlist = new List<GameObject>();
            foreach (Transform child in eventcanvas.transform)
            {
                GameObject temp = child.gameObject;
                eventlist.Add(temp);
            }
            /**/

            /*既存イベントの表示*/
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < eventnum; i++)
            {
                EditorGUILayout.BeginHorizontal();
                eventlist[i].isStatic = EditorGUILayout.Toggle("", eventlist[i].isStatic, GUILayout.Width(100));

                /*ロック時は編集不可*/
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
                /**/

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            /*新規イベントの追加*/
            if (GUILayout.Button("追加", GUILayout.Width(100)))
            {
                AddNewEvent(eventlist, eventcanvas);
            }
            /**/
        }
        /**/

        /*新規イベント(GameObject)の追加*/
        void AddNewEvent(List<GameObject> eventlist, GameObject eventcanvas)
        {
            /*新しいオブジェクトの作成(イベントスクリプトをアタッチ)*/
            GameObject temp = new GameObject();
            temp.AddComponent<Event>();
            /**/

            /*イベントキャンバスの子オブジェクトとして設定*/
            temp.transform.SetParent(eventcanvas.transform);
            /**/

            /*イベント名を変更(デフォルト:(マップ名)event(n))*/
            string mapname = SceneManager.GetActiveScene().name;
            int eventnum = eventcanvas.transform.childCount;
            temp.name = mapname + "event" + eventnum.ToString();
            eventlist.Add(temp);
            /**/
        }
        /**/

        /*イベントの削除*/
        void RemoveEvent(List<GameObject> eventlist, int i)
        {
            DestroyImmediate(eventlist[i].gameObject);
            eventlist.Remove(eventlist[i]);
        }
        /**/
    }
    /**/



    /*マップイベントの編集ウィンドウ*/
    public class EventContents : EditorWindow
    {
        /*仕様*/
        /*
          共通部分                                      内容設定部分(後述)       
          ______________________________________        _____________________________________________________
          起動条件   一度きり   
          接触        ☑
          
          種類       コマンド         上と同時実行
          word       編集　挿入　削除       
          transition 編集　挿入　削除       ☑
            .           .                   .
            .           .                   .

           追加
        */

        /*役割*/
        /*
          起動条件  イベントの起動条件    接触―プレイヤー接触時に実行
                                        自動―シーンに入った時に実行
          一度きり  一回のみ実行するイベントかどうか
          種類      Word(文章表示) Transition(トランジション) Encount(エンカウント) MoveScene(シーン移動) MovePos(プレイヤーの位置移動) Picture(ピクチャ表示)
          コマンド  選択したイベントに対して行う動作    編集－イベント内容の設定、変更
                                                    挿入－選択したマップイベントの前に新規イベントを挿入
                                                    削除－選択したマップイベントの削除
          上と同時実行    上に設定したイベントと同時に実行する
          追加            新しいイベントを最後に追加
        */

        /*内容設定部分*/
        /*イベントの種類によって変化
          Word                              Transition
          ______________________________    ______________________________
          Encount                           MoveScene
          ______________________________    ______________________________
          MovePos                           Picture
          ______________________________    ______________________________
        */

        /*イベントスクリプト*/
        public Event eventconfig;
        /**/

        /*スクロール*/
        Vector2 scrollPosition;
        /**/

        /*選択したイベントの番号保持*/
        private int x = 0;
        /**/

        /*新しいイベントハンドラー*/
        Handler handler = new Handler();
        /**/

        /*デフォルトサイズ*/
        private int DefaultSizeX;
        private int DefaultSizeY;
        /**/

        /*移動シーン*/
        SceneAsset temp = null;
        /**/

        void OnGUI()
        {
            /*ウィンドウサイズ固定*/
            maxSize = new Vector2(800, 400);
            minSize = new Vector2(800, 380);
            /**/

            /*イベントの種類(全イベント共通)*/
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

            /*イベントの種類*/
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
            /**/
            
            /*イベントの追加*/
            if (GUILayout.Button("追加", GUILayout.Width(64)))
            {
                AddEvent();
            }
            /**/

            EditorGUILayout.EndVertical();

            /*イベントの内容表示*/
            if (x >= 0 && x < eventconfig.eventlist.Count)
            {
                /*文章表示*/
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.WORD)
                {
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("テキスト", GUILayout.Width(72));
                    eventconfig.eventlist[x].text = EditorGUILayout.TextArea(eventconfig.eventlist[x].text, GUILayout.Width(374), GUILayout.Height(64));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("アイコン使用", GUILayout.Width(80));
                    EditorGUI.BeginDisabledGroup(!(eventconfig.eventlist[x].useicon = EditorGUILayout.Toggle(eventconfig.eventlist[x].useicon)));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("アイコン画像", GUILayout.Width(80));
                    eventconfig.eventlist[x].icon = EditorGUILayout.ObjectField("", eventconfig.eventlist[x].icon, typeof(Sprite), true, GUILayout.Width(64)) as Sprite;
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndVertical();
                }
                /**/

                /*トランジション*/
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
                        EditorGUILayout.LabelField("albedo", GUILayout.Width(64));
                    }
                    if (eventconfig.eventlist[x].mode == Transition.TRANSITION_MODE._BLACKOUT)
                    {
                        EditorGUILayout.LabelField("blackout", GUILayout.Width(64));
                        EditorGUILayout.LabelField("albedo", GUILayout.Width(64));
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
                        eventconfig.eventlist[x].albedo = EditorGUILayout.Slider("", eventconfig.eventlist[x].albedo, 0, 1, GUILayout.Width(128));
                    }
                    if (eventconfig.eventlist[x].mode == Transition.TRANSITION_MODE._BLACKOUT)
                    {
                        eventconfig.eventlist[x].blackout = EditorGUILayout.Slider("", eventconfig.eventlist[x].blackout, 0, 1, GUILayout.Width(128));
                        eventconfig.eventlist[x].albedo = EditorGUILayout.Slider("", eventconfig.eventlist[x].albedo, 0, 1, GUILayout.Width(128));
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.LabelField("レイヤー", GUILayout.Width(32));
                    eventconfig.eventlist[x].target = (Handler.TARGET)EditorGUILayout.EnumPopup("", eventconfig.eventlist[x].target, GUILayout.Width(72));
                    EditorGUILayout.LabelField("", GUILayout.Width(4));
                }
                /**/

                /*エンカウント*/
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.ENCOUNT)
                {
                    EditorGUILayout.LabelField("グループID", GUILayout.Width(72));
                    eventconfig.eventlist[x].enemygroupID = EditorGUILayout.IntField("", eventconfig.eventlist[x].enemygroupID, GUILayout.Width(64));
                    EditorGUILayout.LabelField("", GUILayout.Width(316));
                }
                /**/

                /*シーン移動*/
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.MOVESCENE)
                {
                    EditorGUILayout.LabelField("移動先シーン", GUILayout.Width(72));
                    if (eventconfig.eventlist[x].movetothisscene == null)
                    {
                        temp = null;
                    }
                    else
                    {
                         temp = eventconfig.eventlist[x].scenetemp;
                    }
                    temp = EditorGUILayout.ObjectField("", temp, typeof(SceneAsset), true, GUILayout.Width(128)) as SceneAsset;
                    EditorGUILayout.LabelField("", GUILayout.Width(200));
                    if (temp != null)
                    {
                        eventconfig.eventlist[x].scenetemp = temp;
                        eventconfig.eventlist[x].movetothisscene = temp.name;
                    }
                }
                /**/

                /*場所移動*/
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
                /**/

                /*ピクチャ表示*/
                if (eventconfig.eventlist[x].type == Handler.EVENTTYPE.PICTURE)
                {
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("ピクチャ表示", GUILayout.Width(72));
                    if (eventconfig.eventlist[x].picture = EditorGUILayout.ObjectField("", eventconfig.eventlist[x].picture, typeof(Sprite), true, GUILayout.Width(64)) as Sprite)
                    {
                        DefaultSizeX = eventconfig.eventlist[x].picture.texture.width;
                        DefaultSizeY = eventconfig.eventlist[x].picture.texture.height;
                    }
                    EditorGUILayout.LabelField("レイヤー", GUILayout.Width(72));
                    eventconfig.eventlist[x].target = (Handler.TARGET)EditorGUILayout.EnumPopup("", eventconfig.eventlist[x].target, GUILayout.Width(72));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("PosX", GUILayout.Width(64));
                    EditorGUILayout.LabelField("PosY", GUILayout.Width(64));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    eventconfig.eventlist[x].picX = EditorGUILayout.IntField("", eventconfig.eventlist[x].picX, GUILayout.Width(64));
                    eventconfig.eventlist[x].picY = EditorGUILayout.IntField("", eventconfig.eventlist[x].picY, GUILayout.Width(64));
                    if (GUILayout.Button("左", GUILayout.Width(32)))
                    {
                        eventconfig.eventlist[x].picX = -400;
                    }
                    if (GUILayout.Button("中央", GUILayout.Width(32)))
                    {
                        eventconfig.eventlist[x].picX = 0;
                    }
                    if (GUILayout.Button("右", GUILayout.Width(32)))
                    {
                        eventconfig.eventlist[x].picX = 400;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("SizeX", GUILayout.Width(64));
                    EditorGUILayout.LabelField("SizeY", GUILayout.Width(64));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    eventconfig.eventlist[x].sizeX = EditorGUILayout.IntField("", eventconfig.eventlist[x].sizeX, GUILayout.Width(64));
                    eventconfig.eventlist[x].sizeY = EditorGUILayout.IntField("", eventconfig.eventlist[x].sizeY, GUILayout.Width(64));
                    if (GUILayout.Button("defaultsize", GUILayout.Width(96)))
                    {
                        eventconfig.eventlist[x].sizeX = DefaultSizeX;
                        eventconfig.eventlist[x].sizeY = DefaultSizeY;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("", GUILayout.Width(12));

                    EditorGUILayout.EndVertical();
                }
                /**/
            }
            /**/

            EditorGUILayout.EndHorizontal();
            
        }

        /*イベントの追加*/
        void AddEvent()
        {
            eventconfig.eventlist.Add(handler);
            handler = new Handler();
        }
        /**/

        /*途中に挿入*/
        void AddEvent(int i)
        {
            eventconfig.eventlist.Insert(i, handler);
            handler = new Handler();
        }
        /**/

        /*イベントの削除*/
        void RemoveEvent(int i)
        {
            eventconfig.eventlist.Remove(eventconfig.eventlist[i]);
        }
        /**/
    }
    /**/
}

#endif