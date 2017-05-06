using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variables
{
    //何度か使う可能性がある変数はこっちに記入
    public class __Debug
    {
        public static bool isDrawFPS = true;
        public static bool isPlayerCheat = true;
    }

    public class Map
    {

        //マップ生成関連
        public static int mapX = 0;
        public static int mapY = 0;
        public const string filepath = "Assets\\Resources\\MapData\\";

        //フィールドマップの親オブジェクト
        public static GameObject map;
        public static MapMake mapobject;
        //ミニマップの親オブジェクト
        public static GameObject minimapcanvas;
        public static miniMap minimap;

        public static void GetGameObject()
        {
            map = GameObject.Find("Map");
            mapobject = map.GetComponent<MapMake>();
            minimapcanvas = GameObject.Find("MiniMap");
            minimap = minimapcanvas.GetComponent<miniMap>();
            playerobject = GameObject.FindWithTag("MainCamera");
        }

        //プレイヤーオブジェクト
        public static GameObject playerobject;
        public static Vector2 playerpos;
        public static float direction;

        public static void GetPlayerPos()
        {
            playerpos = new Vector2(playerobject.transform.localPosition.x, playerobject.transform.localPosition.z);
            direction = playerobject.transform.localEulerAngles.y;
        }    
        
        // プレイヤの配置
        public static void SetPlayer(int StartX, int StartY, float localdirection)
        {
            //カメラの移動
            playerobject.transform.localPosition = new Vector3(StartX, 0.5f, StartY);
            //カメラの向きを最後に向いていた方向へ向ける
            playerobject.transform.localEulerAngles = new Vector3(0, localdirection, 0);

        }

        //ミニマップのオフセット
        public const int OFFSET_X = -350;
        public const int OFFSET_Y = 0;
        public static Vector3 OFFSET = new Vector3(OFFSET_X, OFFSET_Y, 0);

        public static int range = 2;

        //プレイヤー移動関連


        //その他
        public static Vector2 movehere;
    }

    public class Player
    {
        public static int ROTATETIME = 10; // 回転にかかるフレームの設定
        public static int MOVETIME = 10; // 移動フレームの設定
        public static int ROTATEWAITTIME = 10; // 回転にかかるフレームの設定
        public static int MOVEWAITTIME = 10; // 移動フレームの設定
    }

    public class Unit
    {
        // キャラクターの情報
        public static int Num = 15; // キャラクター種類
        public static string PlayerDataFilePath = "Assets\\Resources\\CharacterData\\data.csv";
    }
    

    public class Party
    {
        public static string SingletonObjectName = "SingletonPartyObject";

        public static int Num = 6; // パーティ数
        public static int CharaNumPerParty = 5; // パーティの人数(固定)
        public static int[] DecEditPartyPosX = { -550, -550, -550, 50, 50, 50 };
        public static int[] DecEditPartyPosY = { 100, -50, -200, 100, -50, -200 };
    }

    public class Save
    {
        public static string Name = "SingletonSaveObject"; // 1 つしか存在しないオブジェクト名
    }

    public class Gacha
    {
        public static string BackgroundPath = "Images\\Background\\Background1";
    }

    public class Enemy
    {
        public static string EnemyGroupObjectName = "SingltonEnemyGroup"; // 1 つしか存在しないオブジェクト名
        public static string EnemyDataFilePath = "Assets\\Resources\\CharacterData\\enemyData.csv";
        public static string EnemyGroupFilePath = "Assets\\Resources\\CharacterData\\enemyGroupData.csv";
    }

    public class SE
    {
        public static string soundObjName = "SingletonSoundObj";
        public enum SeName {
        dokan = 0,
        dokan2 = 1,
        kin = 2,
        battle_unison_chari = 3,
        system_cursor = 4,
        system_dec = 5
        };
        public static string[] SeFolderPath ={
            "SE\\hit64",
            "SE\\hit54",
            "SE\\freeze10",
            "SE\\chari14_a",
            "SE\\cursor18",
            "SE\\cursor33"};
    }
    public class BGM
    {
        public static string soundObjName = SE.soundObjName;
        public enum BgmName
        {
            title = 0,
            dungeon = 1,
            battle = 2,
        };
        public static string[] BgmFolderPath ={
            "BGM\\wind",
            "BGM\\seirei",
            "BGM\\kamo99" };
    }

    [System.Serializable]
    public class Handler
    {
        //共通
        public int type;
        public bool waituntilclick = false;

        //文章表示で使用
        public string text;

        //トランジションで使用
        public Texture2D rule;
        public float time;
        public TRANSITION.Transition.TRANSITION_MODE mode;
        public Texture2D mask;
        public float blackout;
        public float whiteout;
        public bool thisobject;
        public GameObject transobject;

        //エンカウントで使用
        public int enemygroupID;

        //シーン移動で使用
        public string movetothisscene;

        //同じエリア間で移動
        public int moveX;
        public int moveY;
        public float direction;
    }


}