using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Variables;
using Utility;

//ミニマップ関連
public class miniMap : MonoBehaviour {

    //ミニマップを構成する画像を格納する配列
    GameObject[,] minimap;

    //現在のモード
    static int currentmode = 2;

    //プレイヤーの位置を表示するオブジェクト
    public GameObject playerpos;

    //プレイヤーの配置
    public void SetPlayer()
    {
        //プレイヤーの位置を表示
        playerpos = _Object.MyInstantiate("Prefabs/Map/playerpos", GameObject.Find("MiniMap"));
        
        //最初は縮小モード
        playerpos.transform.localScale = new Vector3( 0.5f, 0.5f, 0.5f );

        //オフセット調整
        playerpos.transform.localPosition = -Map.OFFSET;

        //最後に向いている方向
        playerpos.transform.localEulerAngles = new Vector3(0,0,-Map.direction);
    }

    //ミニマップの作成
    public void SetminiMap(string Filename) {

        //一時使用変数
        //一時的にファイルから取得した文字列を格納
        string[] buffer;
        string[] linebuffer;

        //ファイルから読み込み
        buffer = System.IO.File.ReadAllLines( Map.filepath + Filename );
        
        //マップの大きさに応じて配列をリサイズ
        //ミニマップの画像を配置する配列
        minimap = new GameObject[Map.mapX, Map.mapY];
        int[,] mapdata = new int[Map.mapX, Map.mapY];


        //マップチップの種類(csv一行目)
        linebuffer = buffer[0].Split(',');
        string[] mapchip = buffer[1].Split(',');
        //ミニマップ用のプレハブはマップ用のプレハブ+miniというファイルで作る
        for ( int i = 0; i < mapchip.Length; i++ )
        {
            mapchip[i] += "mini";
        }
        
        //ミニマップ作成
        for ( int i = 0; i < Map.mapY; i++ )
        {
            //csvの下の段から作成(この方がマップ作成と現実のマップ配置で違和感が少なくなる)
            linebuffer = buffer[2 + Map.mapY - i].Split(',');

            for ( int j = 0; j < Map.mapX; j++ )
            {
                //どのマップチップを配置するか格納
                mapdata[j, i] = int.Parse( linebuffer[j] );

                //ミニマップのマップチップを配置する座標
                Vector3 vec = new Vector3(j * 10, i * 10, 0) -                                          //基本位置
                                          Map.OFFSET -                                                  //オフセット調整
                                          new Vector3(Map.playerpos.x * 10, Map.playerpos.y * 10, 0);   //プレイヤー位置からの相対位置
                                                                                                        //(ミニマップはプレイヤー位置固定で動作しているため)

                //オブジェクトの作成
                minimap[j, i] = _Object.MyInstantiate(mapchip[mapdata[j, i]], vec, Map.minimapcanvas);

                //スケールの調整(縮小モード)
                minimap[j, i].transform.localScale = new Vector3(0.5f, 0.5f, 1);

                //向きの調整(必要ない?)
                minimap[j, i].transform.localEulerAngles = new Vector3(0, 0, 0);

                //自分の周囲だけ表示
                if ( ( i <= Map.playerpos.y + ( Map.range * currentmode ) &&        //プレイヤーを起点として
                       i >= Map.playerpos.y - ( Map.range * currentmode ) ) &&      //-rangeから+rangeまでのオブジェクトのみ表示
                     ( j <= Map.playerpos.x + ( Map.range * currentmode ) &&        //
                       j >= Map.playerpos.x - ( Map.range * currentmode ) ) )       //
                {
                    minimap[j, i].transform.gameObject.SetActive( true );
                }
                else
                {
                    minimap[j, i].transform.gameObject.SetActive( false );
                }
            }
        }   
    }

    //表示モード、通常、拡大、非表示
    public void displaymode( int mode )
    {
        currentmode = mode;
        //非表示
        if ( currentmode == 0 )
        {
            int count = 0;
            foreach ( Transform child in Map.minimap.transform )
            {
                child.gameObject.SetActive( false );
                count++;
            }
        }
        //拡大モード
        else if ( currentmode == 1 )
        {
            int count = 0;
            foreach ( Transform child in Map.minimap.transform )
            {
                child.transform.localScale = new Vector3( 1, 1, 1 );
                count++;
            }
        }
        //縮小モード
        else if ( currentmode == 2 )
        {
            int count = 0;
            foreach ( Transform child in Map.minimap.transform )
            {
                child.transform.localScale = new Vector3( 0.5f, 0.5f, 1 );
                count++;
            }
        }
        //念のためにそれ以外になった時
        else
        {
            Debug.Log("ここが出ちゃ駄目");
        }
    }

    //位置などの更新
    public void updateminimap()
    {
        //非表示なら何もしない
        if ( currentmode == 0 )
        {
        }
        //縮小モード、拡大モードなら自分の周囲だけ表示
        else if ( currentmode == 1 || currentmode == 2 )
        {
            playerpos.GetComponent<Transform>().gameObject.SetActive( true );
            for ( int i = 0;i< Map.mapY; i++ )
            {
                for ( int j = 0; j < Map.mapX; j++ )
                {
                    //自分の周囲だけ表示
                    if ( ( i <= Map.playerpos.y + ( Map.range * currentmode ) &&
                           i >= Map.playerpos.y - ( Map.range * currentmode ) ) &&
                         ( j <= Map.playerpos.x + ( Map.range * currentmode ) &&
                           j >= Map.playerpos.x - ( Map.range * currentmode ) ) )
                    {
                        minimap[j,i].transform.gameObject.SetActive( true );
                    }
                    else
                    {
                        minimap[j, i].transform.gameObject.SetActive( false );
                    }

                    //縮小モードと拡大モードではマップチップの大きさが違うため調整用
                    //minimap[j, i].transform.localscaleから取得した値を使ってもいい
                    int scale = 3 - currentmode;
                    minimap[j, i].transform.localPosition = new Vector3( j * 10, i * 10, 0 ) * scale
                                                                        - Map.OFFSET
                                                                        - new Vector3( Map.playerpos.x * 10, Map.playerpos.y * 10, 0 ) * scale;
                }
            }
        }
    }
}
