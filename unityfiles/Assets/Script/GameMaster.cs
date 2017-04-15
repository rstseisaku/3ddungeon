using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    public int rotateTime = 10; // 回転にかかるフレームの設定
    public int moveTime = 10; // 移動フレームの設定
    public int rotateWaitTime = 10; // 回転にかかるフレームの設定
    public int moveWaitTime = 10; // 移動フレームの設定


    private GameObject lookTarget; // カメラ。スクリプト内で取得。


    // Resources/Prefabs から直接読み込む

    // Use this for initialization
    void Start () {
        // カメラオブジェクトの取得
        lookTarget = GameObject.FindWithTag("MainCamera");

        // 疑似 3D フィールド生成
        for (int j = 0; j < 30; ++j)
        {
            for (int i = 0; i < 30; ++i)
            {
                Instantiate(Resources.Load("Prefabs/FloorArea1"),
                    new Vector3(i , 0, j ), // Plane を 0.1 倍にすると 1x1 になる
                    Quaternion.identity);
                if (i == j )
                {
                    Instantiate(Resources.Load("Prefabs/WallArea1"),
                        new Vector3(i, 0.5f, j),
                        Quaternion.identity);
                }
            }
        }
        // カメラの移動
        lookTarget.transform.position = new Vector3( 0.0f, 0.5f, 0.0f );

        // コルーチンの起動
        StartCoroutine("MyUpdate");
    }

    // Update is called once per frame
    void Update() { }


    IEnumerator MyUpdate() 
    {
        // 『Time.DeltaTime の使い方』
        while (true) // ゲームメインループ
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                // 回転させる(右方向)
                yield return MyRotate(new Vector3(0, 90, 0));
            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                // 回転させる(左方向)
                yield return MyRotate(new Vector3(0, -90, 0));
            }

            if (Input.GetAxis("Vertical") > 0)
            {
                /*
                lookTarget.transform.position += lookTarget.transform.forward; // カメラを前方向に 1 だけ移動
                */

                // 前方に移動させる
                yield return MyMove();   
            }



            // 処理の終了
            yield return 0;
        }
    }

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
        Vector3 movePos = new Vector3(0, 0, 0);
        for (int i = 0; i < moveTime; i++)
        {
            movePos = lookTarget.transform.forward; // カメラを前方向に 1 だけ移動
            // Debug.Log( movePos );
            movePos /= moveTime;

            lookTarget.transform.position += movePos;
            yield return 0;
        }
        for (int i = 0; i < moveWaitTime; i++)
        {
            yield return 0;
        }
    }
}
