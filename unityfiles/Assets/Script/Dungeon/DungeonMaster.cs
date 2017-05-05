using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Variables;
using Utility;

//ダンジョン全体の挙動を管理
public class DungeonMaster : MonoBehaviour {

    //マップごとにインスペクタで設定するべき値:
    //mapname:マップ名(例:マップ1)
    //randomencount:エンカウント率(%)
    //firstX:初期X座標
    //firstY:初期Y座標


    //現在のマップ
    public  string mapname;

    //ミニマップのモード　　0：非表示　1：拡大　2：縮小
    private int mode = 0;

    //エンカウント管理
    private int encount;
    public int randomencount = 0;
    
    //初期位置のセットに必要 2回目以降は最後に居た位置を使用
    private static bool firsttime = true;
    public int firstX;
    public int firstY;
    
    //
    private MapMake map;
    private miniMap minimap;

    // Use this for initialization
    void Start () {

        //マップチップとミニマップを紐づけるオブジェクトを取得
        //必ず最初にこれを行わないとエラー吐く
        Map.GetGameObject();


        //マップの作成
        //マップチップの配置
        MakeMap( mapname );
        //ミニマップの作成
        MakeminiMap( mapname );

        //初回のみ必要な変数なのでここでfalseにする
        firsttime = false;

        // コルーチンの起動(メインループ)
        StartCoroutine("MyUpdate");

    }

    //本ループ
    IEnumerator MyUpdate() 
    {
        // メモ 『Time.DeltaTime の使い方』
        while (true) // ゲームメインループ
        {
            //情報更新
            //プレイヤー位置の更新
            yield return UpdatePlayerPos();
            //ミニマップの更新
            yield return UpdateminiMap();

            //↑キーが押されている間
            if ( Input.GetAxis("Vertical") > 0 )
            {
                //前方が移動可能か判定
                Vector2 nextpos = NextPosition();
                //前方が移動可能なら移動させる
                if (map.isMoveable( (int)nextpos.x, (int)nextpos.y) )
                {
                    //移動処理
                    yield return MyMove();

                    //エンカウント判定
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

            //現在はボタンから起動出来るようにした
            //モード変更用のボタンでも作る?
            //spaceキーが離された時にミニマップのモードを変更
            /*
            if ( Input.GetKeyUp(KeyCode.Space) )
            {
                ChangeMode();
            }*/

            // 処理の終了
            yield return 0;
        }
    } // ---MyUpdate()

    // 一定フレームをかけて回転させる
    IEnumerator MyRotate( Vector3 vec3 )
    {
        //回転角/フレームで徐々に回転を演出
        for ( int i = 0; i < Player.ROTATETIME; i++ )
        {
            //プレイヤーの回転
            Map.playerobject.transform.Rotate( vec3 / Player.ROTATETIME );
            //ミニマップ上でのプレイヤー位置の回転
            minimap.playerpos.GetComponent<Transform>().Rotate(new Vector3(0, 0, -vec3.y) / Player.ROTATETIME);

            yield return 0;
        }

        //一定フレームのウェイト
        yield return _Wait.WaitFrame(Player.ROTATETIME);
    }

    // 一定フレームをかけて移動させる
    IEnumerator MyMove()
    {
        //向いてる方向を取得
        Vector3 movePos = new Vector3( Mathf.Sin( Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360 ),
                                       0,
                                       Mathf.Cos( Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360 ) );
        //フレームあたりの移動距離
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
        yield return _Wait.WaitFrame(Player.MOVEWAITTIME);
    }

    //プレイヤーの位置更新
    IEnumerator UpdatePlayerPos()
    {
        //プレイヤー位置取得用関数
        Map.GetPlayerPos();

        yield return 0;
    }

    //ミニマップの更新
    IEnumerator UpdateminiMap()
    {
        //ミニマップ更新用関数
        minimap.updateminimap();

        yield return 0;
    }

    //移動場所が移動可能か判定
    private Vector2 NextPosition()
    {
        //移動先の位置取得
        //現在位置＋向いている方角へ1マス移動した座標
        int nextX = (int)Map.playerpos.x +
                    (int)Mathf.Sin( Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360 );
        int nextY = (int)Map.playerpos.y + 
                    (int)Mathf.Cos( Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360 );

        return new Vector2( nextX, nextY );
    } 

    //ミニマップのモード変更
    public void ChangeMode()
    {
        //ミニマップのモード変更
        //非表示、拡大、縮小のローテーション
        minimap.displaymode( mode );
        minimap.updateminimap();
        mode++;
        mode %= 3;

        return;
    }

    //マップの作成
    private void MakeMap( string MapName )
    {
        //マップオブジェクトの取得
        map = Map.mapobject;

        //csvファイルに従ってマップを生成
        map.MakeMap( MapName );

        // プレイヤーの位置設定
        //初回
        if ( firsttime )
        {
            SetPlayer( firstX, firstY );
        }
        //２回目以降
        //staticで残っている値を使用
        else
        {
            SetPlayer( (int)Map.playerpos.x, (int)Map.playerpos.y);
        }

        //
        if (Map.usethisvalue == true)
        {
            SetPlayer((int)Map.movehere.x, (int)Map.movehere.y);
            Map.usethisvalue = false;
        }

        return;
    } 

    //ミニマップの作成
    private void MakeminiMap( string MapName )
    {
        //マップ関連オブジェクトの取得
        minimap = Map.minimap;

        //ミニマップの作成
        minimap.SetminiMap(MapName);
        //ミニマップ上にプレイヤーを表示
        minimap.SetPlayer();
    }

    // プレイヤの配置
    private void SetPlayer( int StartX, int StartY )
    {
        //カメラの移動
        Map.playerobject.transform.position = new Vector3( StartX, 0.5f, StartY );
        //カメラの向きを最後に向いていた方向へ向ける
        Map.playerobject.transform.localEulerAngles = new Vector3(0,Map.direction,0);
    }

    //エンカウント判定
    private IEnumerator Encounter()
    {
        //基礎エンカウント率を元に乱数でエンカウントをスタックする
        encount += (int)( Random.Range( 0f, 1.0f ) * randomencount );

        //エンカウントが100を超えたら敵と遭遇
        //その後エンカウントを0に戻す
        if ( encount > 100 )
        {
           //戦闘シーンへ移行
           //出来ればここで出現する敵を決定したい
            yield return Utility._Scene.MoveScene("BattleScene");
            encount = 0;
        }
    }
}
