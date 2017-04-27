using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Variables
{
    //何度か使う可能性がある変数はこっちに記入
    
    public class Map 
    {

        //マップ生成関連
        public static int mapX = 0;
        public static int mapY = 0;
        public const string filepath = "Assets\\Resources\\MapData\\";

        //フィールドマップの親オブジェクト
        public static GameObject map;
        public static Map1 map1;
        //ミニマップの親オブジェクト
        public static GameObject minimapcanvas;
        public static miniMap minimap;

        public static void GetGameObject()
        {
            map = GameObject.Find("Map");
            map1 = map.GetComponent<Map1>();
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

        //ミニマップのオフセット
        public const int OFFSET_X = -100;
        public const int OFFSET_Y = 0;
        public static Vector3 OFFSET = new Vector3(OFFSET_X, OFFSET_Y, 0);

        public static int range = 2;

        //プレイヤー移動関連


        //その他

    }

    public class Player
    {
        public static int ROTATETIME = 10; // 回転にかかるフレームの設定
        public static int MOVETIME = 10; // 移動フレームの設定
        public static int ROTATEWAITTIME = 10; // 回転にかかるフレームの設定
        public static int MOVEWAITTIME = 10; // 移動フレームの設定
    }

}