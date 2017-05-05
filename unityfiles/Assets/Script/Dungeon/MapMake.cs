using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using  Variables;
using Utility;

//マップメイク関連
public class MapMake : MonoBehaviour {
    
    //マップチップの名前
    private string[] mapchip;
    //オブジェクトの通行可否、床オブジェクトか壁オブジェクトか
    private int[] ObjectType;
    //座標毎のオブジェクトの種類
    private int[,] mapdata;

    //マップを作成
    //引数で作りたいマップのcsvを参照
    public void MakeMap( string FileName )
    {
        //一時使用変数
        //ファイルから文字列を格納する変数
        string[] buffer;
        string[] linebuffer;

        //bufferにファイルから読み込んだものを格納
        buffer = System.IO.File.ReadAllLines( Map.filepath + FileName );

        //マップのサイズを格納(csv1行目)
        linebuffer = buffer[0].Split(',');
        Map.mapX = int.Parse( linebuffer[0] );
        Map.mapY = int.Parse( linebuffer[1] );
        //マップデータを管理する配列をリサイズ
        mapdata = new int[Map.mapX, Map.mapY];
    
        //マップ生成に使うプレハブの取得(csv2行目)
        mapchip = buffer[1].Split(',');
        ObjectType = new int[mapchip.Length];

        //オブジェクトの種類(通行可否、床壁オブジェクト)の取得(csv3行目)
        linebuffer = buffer[2].Split(',');
        //マップチップの種類を取得
        for ( int i = 0; i < ObjectType.Length; i++ ) //
        {
            //何もない所は-1として置く
            if( linebuffer[i] == "" )
            {
                ObjectType[i] = -1;
            }
            //0:通行可能床 1:通行不可床 2:通行可壁(扉など?) 3:通行不可壁
            else
            {
                ObjectType[i] = int.Parse( linebuffer[i] );
            }
        }

        //マップデータを元にマップを作成
        for ( int i = 0; i < Map.mapY; i++ )
        {
            //csvの下の段から作成(この方がマップ作成と現実のマップ配置で違和感が少なくなる)
            linebuffer = buffer[2 + Map.mapY - i].Split(',');

            for ( int j = 0; j < Map.mapX; j++ )
            {
                //配置するマップチップを取得
                mapdata[j, i] = int.Parse( linebuffer[j] );

                //床、壁オブジェクトは配置するy座標が違うのでここで定義
                float posY = 0.0f;
                //床オブジェクト
                if (ObjectType[mapdata[j, i]] == 0 ||
                     ObjectType[mapdata[j, i]] == 1)
                {
                    posY = 0.0f;
                }
                //壁オブジェクト
                else if (ObjectType[mapdata[j, i]] == 2 ||
                         ObjectType[mapdata[j, i]] == 3)
                {
                    posY = 0.5f;
                }

                //オブジェクトの作成
                _Object.MyInstantiate(mapchip[mapdata[j, i]], new Vector3(j, posY, i), Map.map);
            }
        }
    } //---MakeMap


    //移動先が移動可能か判定
    // input: 移動先の座標
    public bool isMoveable( int x,int y )
    {
        int tmp = ObjectType[mapdata[x, y]];
        //0:通行可能床 1:通行不可床 2:通行可壁(扉など?) 3:通行不可壁   
        if ( tmp == 0 || tmp == 2 ) return true;
        else return false;
    }
}
