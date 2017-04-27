using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class mInput : MonoBehaviour
{
    // マウス操作を用いるか、タッチ操作を用いるか
    public static bool isBrowserGame = true;

    public static int touchNum; // タッチされた個数
    public static Vector2[] touchPos; // タッチされた座標
    public static TouchPhase[] touchPhase; // タッチされている状況(新押し判定など)
    public static int existNewTouch = -1; // 新タッチへの ID を返す
    public static int existEndTouch = -1; // 話されたタッチへの ID を返す(座標とかは残るからデータを使える)

    // Update is called once per frame
    void Start()
    {
        touchPos = new Vector2[3];
        touchPhase = new TouchPhase[3];
    }

    /*
     * 基本的に入力は 1 つとして扱う方が楽
     */
    void Update()
    {
        if( !isBrowserGame )
        {
            // スマホゲームの入力処理
            Smartphone();
        }
        else
        {
            // ブラウザゲーム用の入力処理( マウス )
            // ( 別名・デバッグモード )
            BrowserGame();
        }
    }


    private void BrowserGame()
    {
        touchNum = 0;
        existNewTouch = -1;
        existEndTouch = -1;

        int num = 0; // 左クリック
        if( Input.GetMouseButtonDown(num))
        {
            existNewTouch = 0;
            touchPhase[0] = TouchPhase.Began; // enum
        }
        if (Input.GetMouseButton(num))
        {
            touchPos[0].x = Input.mousePosition.x;
            touchPos[0].y = Input.mousePosition.y;
            touchPos[0].x = touchPos[0].x / Screen.width * 1280;
            touchPos[0].y = touchPos[0].y / Screen.height * 720;
            touchPos[0].x -= 640;
            touchPos[0].y -= 360;
            touchPhase[0] = TouchPhase.Stationary; // enum
        }
        if (Input.GetMouseButtonUp(num))
        {
            existEndTouch = 0;
            touchPhase[0] = TouchPhase.Ended; // enum
        }
    }

    private void Smartphone()
    {
        // タッチされている個数を格納
        touchNum = Input.touchCount;
        existNewTouch = -1;
        existEndTouch = -1;

        // タッチがあった場合
        int i = 0;
        foreach (Touch t in Input.touches)
        {
            if (t.phase != TouchPhase.Ended &&
                t.phase != TouchPhase.Canceled)
            {
                // タッチ状況を格納しておく
                touchPos[i].x = t.position.x;
                touchPos[i].y = t.position.y;
                touchPos[i].x = touchPos[i].x / Screen.width * 1280;
                touchPos[i].y = touchPos[i].y / Screen.height * 720;
                touchPos[i].x -= 640;
                touchPos[i].y -= 360;
                touchPhase[i] = t.phase;
                if (t.phase == TouchPhase.Began) existNewTouch = i;
                i++;
            }
            if (t.phase == TouchPhase.Ended) existEndTouch = i;
        }
    }

}
