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

        map.MakeMap("Map1.csv");
        SetPlayer(1,2);
        

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
            PlayerPositionX = (int)Mathf.Round(lookTarget.transform.position.x);
            PlayerPositionY = (int)Mathf.Round(lookTarget.transform.position.z);

            int nextX = PlayerPositionX + (int)Mathf.Sin(lookTarget.transform.localEulerAngles.y * 2 * Mathf.PI / 360);
            int nextY = PlayerPositionY + (int)Mathf.Cos(lookTarget.transform.localEulerAngles.y * 2 * Mathf.PI / 360);
            // Debug.Log();

            if (Input.GetAxis("Vertical") > 0)
            {
                if (map.isMoveable(nextX, nextY))
                {
                    // 前方に移動させる
                    yield return MyMove();
                    continue;
                }
                else
                {
                   // for( int i=0; i<60; i++) yield return 0;
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
        movePos = lookTarget.transform.forward; // カメラを前方向に 1 だけ移動
                                                // Debug.Log( movePos );
        movePos /= moveTime;
        
        for (int i = 0; i < moveTime; i++)
        {
            lookTarget.transform.position += movePos;
            yield return 0;
        }
        lookTarget.transform.position = new Vector3(Mathf.Round(lookTarget.transform.position.x),
                                                    lookTarget.transform.position.y,
                                                    Mathf.Round(lookTarget.transform.position.z));
        for (int i = 0; i < moveWaitTime; i++)
        {
            yield return 0;
        }
    }

    //playerの配置
    private void SetPlayer(int StartX, int StartY)
    {
        // カメラの移動
        lookTarget.transform.position = new Vector3(StartX, 0.5f, StartY);

    }
}
