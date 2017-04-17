using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mInput : MonoBehaviour
{
    public static int touchNum; // タッチされた個数
    public static Vector2[] touchPos; // タッチされた座標
    public static TouchPhase[] touchPhase; // タッチされている状況(新押し判定など)
    public static int existNewTouch; // 新タッチへの ID を返す
    public static int existEndTouch; // 話されたタッチへの ID を返す(座標とかは残るからデータを使える)

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
        // Debug.Log(touchPos[0]);
    }
}
