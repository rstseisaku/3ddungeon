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
        public static GameObject map = GameObject.Find("Map");
        public static Map1 map1 = map.GetComponent<Map1>();
        //ミニマップの親オブジェクト
        public static GameObject minimapcanvas = GameObject.Find("MiniMap");
        public static miniMap minimap = minimapcanvas.GetComponent<miniMap>();

        //プレイヤーオブジェクト
        public static GameObject playerobject = GameObject.FindWithTag("MainCamera");
        public static Vector2 playerpos = new Vector2(playerobject.transform.localPosition.x, playerobject.transform.localPosition.z);

        public static void GetPlayerPos()
        {
            playerpos = new Vector2(playerobject.transform.localPosition.x, playerobject.transform.localPosition.z);
        }

        //ミニマップのオフセット
        public const int OFFSET_X = -100;
        public const int OFFSET_Y = 0;
        public static Vector3 OFFSET = new Vector3(OFFSET_X, OFFSET_Y, 0);

        public static int range = 2;

        //プレイヤー移動関連


        //その他

    }
}