using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    public int rotateTime = 10; // 回転にかかるフレームの設定
    public int moveTime = 10; // 移動フレームの設定
    public int rotateWaitTime = 10; // 回転にかかるフレームの設定
    public int moveWaitTime = 10; // 移動フレームの設定

    //ダンジョン内の現在位置
    private int PlayerPositionX;
    private int PlayerPositionY;



    private GameObject lookTarget; // カメラ。スクリプト内で取得。

    Map1 map = new Map1();


    // Resources/Prefabs から直接読み込む

    // Use this for initialization
    void Start () {
        // カメラオブジェクトの取得
        lookTarget = GameObject.FindWithTag("MainCamera");

        // csvファイルに従ってマップを生成
        map.MakeMap("Map1.csv");

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
            PlayerPositionX = (int)Mathf.Round(lookTarget.transform.position.x);
            PlayerPositionY = (int)Mathf.Round(lookTarget.transform.position.z);

            // 角度と現在位置から、真っすぐ進んだ際の2D(X,Y) を求めておく
            int nextX = PlayerPositionX + (int)Mathf.Sin(lookTarget.transform.localEulerAngles.y * 2 * Mathf.PI / 360);
            int nextY = PlayerPositionY + (int)Mathf.Cos(lookTarget.transform.localEulerAngles.y * 2 * Mathf.PI / 360);

            if (Input.GetAxis("Vertical") > 0)
            {
                // 前方に移動させる
                if (map.isMoveable(nextX, nextY))
                {
                    yield return MyMove();
                    continue;
                }
            }

            if (Input.GetAxis("Horizontal") > 0)
            {
                // 回転させる(右方向)
                yield return MyRotate(new Vector3(0, 90, 0));
                continue;
            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                // 回転させる(左方向)
                yield return MyRotate(new Vector3(0, -90, 0));
                continue;
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
            lookTarget.transform.Rotate( vec3 / rotateTime );
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
        Vector3 movePos = new Vector3(0, 0, 0);
        movePos = lookTarget.transform.forward; // カメラを前方向に 1 だけ移動
        movePos /= moveTime;
        
        //一定フレームかけて移動する
        for (int i = 0; i < moveTime; i++)
        {
            lookTarget.transform.position += movePos;
            yield return 0;
        }
        // 誤差修正
        lookTarget.transform.position =
            new Vector3(Mathf.Round(lookTarget.transform.position.x),
                        lookTarget.transform.position.y,
                        Mathf.Round(lookTarget.transform.position.z));
        // 移動後のウェイト
        for (int i = 0; i < moveWaitTime; i++)
        {
            yield return 0;
        }
    }

    // プレイヤの配置
    private void SetPlayer(int StartX, int StartY)
    {
        // カメラの移動
        lookTarget.transform.position = new Vector3(StartX, 0.5f, StartY);
    }
}
