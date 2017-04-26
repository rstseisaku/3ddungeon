using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using  Variables;

public class Map1 : MonoBehaviour {
    
    //マップチップの名前
    string[] mapchip;
    //オブジェクトの通行可否、床オブジェクトか壁オブジェクトか
    public int[] ObjectType;
    //座標毎のオブジェクトの種類
    int[,] mapdata;

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
		
	}

    //マップを作成する関数
    public void MakeMap( string FileName )
    {
        //一時的に文字列を格納するファイル
        string[] buffer;
        string[] linebuffer;

        GameObject temp;

        //bufferにファイルから読み込んだものを格納
        buffer = System.IO.File.ReadAllLines( Map.filepath + FileName );
        //カンマで区切られている情報ごとに分割
        linebuffer = buffer[0].Split(',');
        //マップのサイズを格納
        Map.mapX = int.Parse( linebuffer[0] );
        Map.mapY = int.Parse( linebuffer[1] );
        mapdata = new int[Map.mapX, Map.mapY];
        

    
        //マップ生成に使うプレハブの取得
        mapchip = buffer[1].Split(',');
        ObjectType = new int[mapchip.Length];

        //オブジェクトの種類の取得
        linebuffer = buffer[2].Split(',');
        for ( int i = 0; i < ObjectType.Length; i++ ) // マップチップの要素数(種類)
        {
            //何もない所は-1として置く
            if( linebuffer[i] == "" )
            {
                ObjectType[i] = -1;
            }
            else
            {
                ObjectType[i] = int.Parse( linebuffer[i] );
            }
        }

        //マップデータを起こす
        for ( int i = 0; i < Map.mapY; i++ )
        {
            linebuffer = buffer[2 + Map.mapY - i].Split(',');

            for ( int j = 0; j < Map.mapX; j++ )
            {
                /*
                 MapData[x, y]に直した
                 */
                mapdata[j, i] = int.Parse( linebuffer[j] );
            }
        }

        //マップデータ通りにプレハブを配置
        for ( int i = 0; i < Map.mapY; i++ )
        {

            for ( int j = 0; j < Map.mapX; j++ )
            {
                float posY = 0.0f;
                //床オブジェクト
                if ( ObjectType[mapdata[j, i]] == 0 ||
                     ObjectType[mapdata[j, i]] == 1 )
                {
                    posY = 0.0f;
                }
                //壁オブジェクト
                else if( ObjectType[mapdata[j, i]] == 2 ||
                         ObjectType[mapdata[j, i]] == 3 )
                {
                    posY = 0.5f;
                }
                temp = Instantiate( Resources.Load( mapchip[mapdata[j, i]] ),
                                    new Vector3( j, posY, i ),
                                    Quaternion.identity ) as GameObject;

                //MapObjectの子オブジェクトにする
                temp.transform.SetParent( Map.map.transform );

            }
        }
    } //---MakeMap


    // input: 移動先の座標
    public bool isMoveable( int x,int y )
    {
        int tmp = ObjectType[mapdata[x, y]];
        if ( tmp == 0 || tmp == 2 ) return true;
        else return false;
    }
}
