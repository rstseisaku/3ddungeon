using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Variables;
using Utility;

//ダンジョン全体の挙動を管理
public class DungeonMaster : MonoBehaviour
{

    //マップごとにインスペクタで設定するべき値:
    //mapname:マップ名(例:マップ1)
    //randomencount:エンカウント率(%)
    //firstX:初期X座標
    //firstY:初期Y座標


    //現在のマップ
    public string mapname;

    //ミニマップのモード　　0：非表示　1：拡大　2：縮小
    private int mode = 0;

    //エンカウント管理
    private int encount;
    public int randomencount = 0;

    //
    private MapMake map;
    private miniMap minimap;

    private bool flag;

    private Coroutine coroutine;

    // Use this for initialization
    void Start()
    {
        SoundManager.SceneChangePlaySound(BGM.BgmName.dungeon);
        flag = false;

        //マップチップとミニマップを紐づけるオブジェクトを取得
        //必ず最初にこれを行わないとエラー吐く
        Map.GetGameObject();
        if (Map.playerpos == new Vector2(0,0))
        {
            Map.GetPlayerPos();
        }
        // プレイヤーの位置設定

        //２回目以降
        //staticで残っている値を使用

        Map.SetPlayer((int)Map.playerpos.x, (int)Map.playerpos.y, Map.direction);
    

        //マップの作成
        //マップチップの配置
        MakeMap(mapname);
        //ミニマップの作成
        MakeminiMap(mapname);

        //本ループ
        StartCoroutine(MyUpdate());
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


            //デバッグ中はキー操作の方が楽
            if (__Debug.isInputKeyEnabled == true)
            {
                //↑キーが押されている間
                if (Input.GetAxis("Vertical") > 0)
                {
                    //前方が移動可能か判定
                    Vector2 nextpos = NextPosition();
                    //前方が移動可能なら移動させる
                    if (map.isMoveable((int)nextpos.x, (int)nextpos.y))
                    {
                        //移動処理
                        yield return MyMove();

                        //エンカウント判定
                        yield return Encounter();

                        continue;
                    }
                }
                //→キーが押されている間
                if (Input.GetAxis("Horizontal") > 0)
                {
                    // 回転させる(右方向)
                    yield return MyRotate(new Vector3(0, 90, 0));
                    continue;
                }
                //←キーが押されている間
                if (Input.GetAxis("Horizontal") < 0)
                {
                    // 回転させる(左方向)
                    yield return MyRotate(new Vector3(0, -90, 0));
                    continue;
                }


                //現在はボタンから起動出来るようにした
                //モード変更用のボタンでも作る?
                //spaceキーが離された時にミニマップのモードを変更

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    ChangeMode();
                }
            }
        }
    } // ---MyUpdate()

    // 一定フレームをかけて回転させる
    public IEnumerator MyRotate(Vector3 vec3)
    {
        //回転角/フレームで徐々に回転を演出
        for (int i = 0; i < Player.ROTATETIME; i++)
        {
            //プレイヤーの回転
            Map.playerobject.transform.Rotate(vec3 / Player.ROTATETIME);
            //ミニマップ上でのプレイヤー位置の回転
            minimap.playerpos.GetComponent<Transform>().Rotate(new Vector3(0, 0, -vec3.y) / Player.ROTATETIME);

            yield return 0;
        }

        //一定フレームのウェイト
        yield return _Wait.WaitFrame(Player.ROTATETIME);

        flag = false;

        yield break;
    }

    // 一定フレームをかけて移動させる
    IEnumerator MyMove()
    {
        //向いてる方向を取得
        Vector3 movePos = new Vector3(Mathf.Sin(Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360),
                                       0,
                                       Mathf.Cos(Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360));
        //フレームあたりの移動距離
        movePos /= Player.MOVETIME;
        //一定フレームかけて移動する
        for (int i = 0; i < Player.MOVETIME; i++)
        {
            Map.playerobject.transform.position += movePos;
            yield return 0;
        }
        // 誤差修正
        Map.playerobject.transform.position = new Vector3(Mathf.Round(Map.playerobject.transform.position.x),
                                                           Map.playerobject.transform.position.y,
                                                           Mathf.Round(Map.playerobject.transform.position.z));

        yield return UpdatePlayerPos();

        // 移動後のウェイト
        yield return _Wait.WaitFrame(Player.MOVEWAITTIME);

        flag = false;

        yield break;
    }

    public void Turn_LEFT()
    {
        if (flag == false)
        {
            Vector3 vec3 = new Vector3(0, -90, 0);
            StartCoroutine(MyRotate(vec3));
            flag = true;
        }
    }
    public void Turn_RIGHT()
    {
        if (flag == false)
        {
            Vector3 vec3 = new Vector3(0, 90, 0);
            StartCoroutine(MyRotate(vec3));
            flag = true;
        }
    }
    public void Go_Straight()
    {
        if (flag == false)
        {
            //前方が移動可能か判定
            Vector2 nextpos = NextPosition();
            //前方が移動可能なら移動させる
            if (map.isMoveable((int)nextpos.x, (int)nextpos.y))
            {
                //移動処理
                coroutine = StartCoroutine(MyMove());

                //エンカウント判定
                StartCoroutine(Encounter());

                flag = true;
            }
        }
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
                    (int)Mathf.Sin(Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360);
        int nextY = (int)Map.playerpos.y +
                    (int)Mathf.Cos(Map.playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360);

        return new Vector2(nextX, nextY);
    }

    //ミニマップのモード変更
    public void ChangeMode()
    {
        //ミニマップのモード変更
        //非表示、拡大、縮小のローテーション
        minimap.displaymode(mode);
        minimap.updateminimap();
        mode++;
        mode %= 3;

        return;
    }

    //マップの作成
    private void MakeMap(string MapName)
    {
        //マップオブジェクトの取得
        map = Map.mapobject;

        //csvファイルに従ってマップを生成
        map.MakeMap(MapName);

        return;
    }

    //ミニマップの作成
    private void MakeminiMap(string MapName)
    {
        //マップ関連オブジェクトの取得
        minimap = Map.minimap;

        //ミニマップの作成
        minimap.SetminiMap(MapName);
        //ミニマップ上にプレイヤーを表示
        minimap.SetPlayer();
    }

    //エンカウント判定
    private IEnumerator Encounter()
    {
        //基礎エンカウント率を元に乱数でエンカウントをスタックする
        encount += (int)(Random.Range(0f, 1.0f) * randomencount);

        //エンカウントが100を超えたら敵と遭遇
        //その後エンカウントを0に戻す
        if (encount > 100)
        {
            //戦闘シーンへ移行
            //出来ればここで出現する敵を決定したい
            yield return _Encount.Encount((int)(Random.Range(1f, 4.0f)));
            encount = 0;
        }
        yield break;
    }

    public void Stop()
    {
        StopCoroutine(coroutine);
    }
}
