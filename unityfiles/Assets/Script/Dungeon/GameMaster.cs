using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Variables;

public class GameMaster : MonoBehaviour {
    public int rotateTime = 10; // 回転にかかるフレームの設定
    public int moveTime = 10; // 移動フレームの設定
    public int rotateWaitTime = 10; // 回転にかかるフレームの設定
    public int moveWaitTime = 10; // 移動フレームの設定

    //ダンジョン内の現在位置
    private int PlayerPositionX;
    private int PlayerPositionY;

    //ミニマップのモード　　0：非表示　1：拡大　2：縮小
    private int mode = 0;

    private GameObject playerobject; // カメラ。スクリプト内で取得。


    //unityに文句言われるからそのうち直す
    Map1 map;
    miniMap minimap;


    // Resources/Prefabs から直接読み込む

    // Use this for initialization
    void Start () {

        //マップ関連オブジェクトの取得
        map = Map.map1;
        minimap = Map.minimap;
        

        // カメラオブジェクトの取得
        playerobject = Map.playerobject;

        // csvファイルに従ってマップを生成
        map.MakeMap("Map1.csv");
        minimap.SetminiMap("Map1.csv");

        // プレイヤーの位置設定
        SetPlayer(1,2);        

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
            // カメラ位置から 2Dマップでの(X,Y)を求める
            PlayerPositionX = (int)Mathf.Round(playerobject.transform.position.x);
            PlayerPositionY = (int)Mathf.Round(playerobject.transform.position.z);

            // 角度と現在位置から、真っすぐ進んだ際の2D(X,Y) を求めておく
            int nextX = PlayerPositionX + (int)Mathf.Sin(playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360);
            int nextY = PlayerPositionY + (int)Mathf.Cos(playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360);

            //↑キーが押されている間
            if (Input.GetAxis("Vertical") > 0)
            {
                // 前方に移動させる
                if (map.isMoveable(nextX, nextY))
                {
                    Debug.Log(Map.playerpos);
                    yield return MyMove();
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
            //spaceキーが離された時にミニマップのモードを変更
            if (Input.GetKeyUp(KeyCode.Space))
            {
                minimap.displaymode(mode);
                mode++;
                mode %= 3;
            }
            // 処理の終了
            yield return 0;
        }
    } // ---MyUpdate()

    // 一定フレームをかけて回転させる
    IEnumerator MyRotate( Vector3 vec3 )
    {
        for (int i = 0; i < rotateTime; i++)
        {
            playerobject.transform.Rotate( vec3 / rotateTime );
            yield return 0;
        }
        for (int i = 0; i < rotateWaitTime; i++)
        {
            yield return 0;
        }
    }

    // 一定フレームをかけて移動させる
    IEnumerator MyMove()
    {
        // 向いてる方向に 1 マス移動
        Vector3 movePos = new Vector3(Mathf.Sin(playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360), 0, Mathf.Cos(playerobject.transform.localEulerAngles.y * 2 * Mathf.PI / 360));
        //movePos = playerobject.transform.forward; // カメラを前方向に 1 だけ移動
        movePos /= moveTime;
        //一定フレームかけて移動する
        for (int i = 0; i < moveTime; i++)
        {
            playerobject.transform.position += movePos;
            yield return 0;
        }
        // 誤差修正
        playerobject.transform.position =
            new Vector3(Mathf.Round(playerobject.transform.position.x),
                        playerobject.transform.position.y,
                        Mathf.Round(playerobject.transform.position.z));
        // 移動後のウェイト
        for (int i = 0; i < moveWaitTime; i++)
        {
            yield return 0;
        }
        //プレイヤーの位置情報の更新
        Map.GetPlayerPos();
        GameObject.Find("playerpos").GetComponent<Transform>().localPosition = new Vector3(Map.playerpos.x * 10, Map.playerpos.y * 10, 0) -Map.OFFSET;
    }

    // プレイヤの配置
    private void SetPlayer(int StartX, int StartY)
    {
        // カメラの移動
        playerobject.transform.position = new Vector3(StartX, 0.5f, StartY);
    }
}
