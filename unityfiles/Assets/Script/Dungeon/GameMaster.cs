using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Variables;

public class GameMaster : MonoBehaviour {

    //現在のマップ
    public  string mapname;

    //ミニマップのモード　　0：非表示　1：拡大　2：縮小
    private int mode = 0;

    //エンカウント管理
    private int encount;
    int randomencount = 999999;
    
    private static bool firsttime = true;

    //unityに文句言われるからそのうち直す
    Map1 map;
    miniMap minimap;


    // Use this for initialization
    void Start () {

        Map.GetGameObject();

        MakeMap( mapname );
        MakeminiMap( mapname );

        firsttime = false;

        // コルーチンの起動(メインループ)
        StartCoroutine("MyUpdate");

    }

    // Update is called once per frame
    void Update() {
        // 利用しない
    }

    IEnumerator MyUpdate() 
    {
        // メモ 『Time.DeltaTime の使い方』
        while (true) // ゲームメインループ
        {
            yield return UpdatePlayerPos();

            yield return UpdateminiMap();

            //↑キーが押されている間
            if ( Input.GetAxis("Vertical") > 0 )
            {
                Vector2 nextpos = NextPosition();
                // 前方に移動させる
                if (map.isMoveable( (int)nextpos.x, (int)nextpos.y) )
                {
                    yield return MyMove();
                    yield return Encounter();
                    continue;
                }
            }

            //→キーが押されている間
            if ( Input.GetAxis("Horizontal") > 0 )
            {
                // 回転させる(右方向)
                yield return MyRotate( new Vector3( 0, 90, 0 ) );
                continue;
            }
            //←キーが押されている間
            if ( Input.GetAxis("Horizontal") < 0 )
            {
                // 回転させる(左方向)
                yield return MyRotate( new Vector3( 0, -90, 0 ) );
                continue;
            }
            //spaceキーが離された時にミニマップのモードを変更
            if ( Input.GetKeyUp(KeyCode.Space) )
            {
                ChangeMode();
            }
            // 処理の終了
            yield return 0;
        }
    } // ---MyUpdate()

    // 一定フレームをかけて回転させる
    IEnumerator MyRotate( Vector3 vec3 )
    {
        //どっちがいいっすかね
  //      minimap.playerpos.GetComponent<Transform>().Rotate(new Vector3(0, 0, vec3.y));
        for ( int i = 0; i < Player.ROTATETIME; i++ )
        {
            minimap.playerpos.GetComponent<Transform>().Rotate( new Vector3( 0, 0, -vec3.y ) / Player.ROTATETIME );

            Map.playerobject.transform.Rotate( vec3 / Player.ROTATETIME );
            yield return 0;
        }
        for ( int i = 0; i < Player.ROTATEWAITTIME; i++ )
        {
            yield return 0;
        }
    }

    // 一定フレームをかけて移動させる
    IEnumerator MyMove()
    {
        // 向いてる方向に 1 マス移動
        Vector3 movePos = new Vector3( Mathf.Sin( Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360 ),
                                       0,
                                       Mathf.Cos( Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360 ) );
        //movePos = playerobject.transform.forward; // カメラを前方向に 1 だけ移動
        movePos /= Player.MOVETIME;
        //一定フレームかけて移動する
        for ( int i = 0; i < Player.MOVETIME; i++ )
        {
            Map.playerobject.transform.position += movePos;
            yield return 0;
        }
        // 誤差修正
        Map.playerobject.transform.position = new Vector3( Mathf.Round( Map.playerobject.transform.position.x ),
                                                           Map.playerobject.transform.position.y,
                                                           Mathf.Round( Map.playerobject.transform.position.z ) );
        

        // 移動後のウェイト
        for ( int i = 0; i < Player.MOVEWAITTIME; i++ )
        {
            yield return 0;
        }
    }

    //プレイヤーの位置更新
    IEnumerator UpdatePlayerPos()
    {
        Map.GetPlayerPos();

        yield return 0;
    }

    //ミニマップの更新
    IEnumerator UpdateminiMap()
    {
        minimap.updateminimap();

        yield return 0;
    }

    //移動場所が移動可能か判定
    private Vector2 NextPosition()
    {
        int nextX = (int)Map.playerpos.x + (int)Mathf.Sin( Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360 );
        int nextY = (int)Map.playerpos.y + (int)Mathf.Cos( Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360 );

        return new Vector2( nextX, nextY );
    } 

    //ミニマップのモード変更
    private void ChangeMode()
    {
        minimap.displaymode( mode );
        minimap.updateminimap();
        mode++;
        mode %= 3;
    }

    //マップの作成
    private void MakeMap( string MapName )
    {
        
        //マップオブジェクトの取得
        map = Map.map1;

        // csvファイルに従ってマップを生成
        map.MakeMap( MapName );

        // プレイヤーの位置設定
        if ( firsttime )
        {
            SetPlayer( 1, 2 );
        }
        else
        {
            SetPlayer( (int)Map.playerpos.x, (int)Map.playerpos.y );
        }
        Map.GetPlayerPos();
    } 

    //ミニマップの作成
    private void MakeminiMap( string MapName )
    {
        //マップ関連オブジェクトの取得
        minimap = Map.minimap;

        //ミニマップの作成
        minimap.SetminiMap("Map1.csv");
        minimap.SetPlayer();
    }

    // プレイヤの配置
    private void SetPlayer( int StartX, int StartY )
    {
        // カメラの移動
        Map.playerobject.transform.position = new Vector3( StartX, 0.5f, StartY );
        Map.playerobject.transform.localEulerAngles = new Vector3(0,Map.direction,0);
    }

    private IEnumerator Encounter()
    {
        encount += (int)( Random.Range( 0f, 1.0f ) * randomencount );
        if ( encount > 100 )
        {
            yield return Utility._Scene.MoveScene("BattleScene");
            encount = 0;
        }
    }
}
