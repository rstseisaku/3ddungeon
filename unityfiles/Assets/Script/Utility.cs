using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Utility : MonoBehaviour
{
    public static IEnumerator Wait( int frame )
    {
        for (int i = 0; i < frame; i++)
            yield return 0;
    }

    // 止まらない？ ⇒ yield return 忘れてない？
    public static IEnumerator WaitKey()
    {
        while ( mInput.existNewTouch == -1 ) // 新押があるまで待機
        {
            yield return 0;
        }
    }


    // ファイル名からテクスチャを返す
    public static Texture2D MyGetTexture(string FilePath)
    {
        return Resources.Load<Texture2D>(FilePath);
    }

    // ファイル名からオブジェクトを返す
    public static GameObject MyInstantiate(string FilePath)
    {
        return MyInstantiate(FilePath, null);
    }

    // ファイル名からオブジェクトを返す(Canvasセット付)
    public static GameObject MyInstantiate(string FilePath, GameObject c)
    {
        GameObject obj;
        obj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        if (c != null) { obj.transform.SetParent(c.transform, false); }
        return obj;
    }

    // ファイル名からオブジェクトを返す
    // (Image ならスプライトを貼り付ける)
    public static GameObject MyInstantiate(string FilePath, GameObject c, string FilePathTexture)
    {
        GameObject obj = MyInstantiate(FilePath, c);
        Texture2D t = MyGetTexture(FilePathTexture);
        Image image = obj.GetComponent<Image>();
        if( image != null)
        {
            image.sprite = Sprite.Create(t,
                new Rect(0, 0, t.width, t.height),
                Vector2.zero);
        }
        return obj;
    }

    // ファイル名からオブジェクトを返す
    // (Image ならスプライトを貼り付け、サイズを設定)
    public static GameObject MyInstantiate(string FilePath, GameObject c, string FilePathTexture, Vector2 vec)
    {
        GameObject obj = MyInstantiate(FilePath, c, FilePathTexture);
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect != null) rect.sizeDelta = vec;
        return obj;
    }

    /*
     * ここら辺の処理は、書き直す
     */
    public static IEnumerator MoveScene(
        string sceneName,
        string FadeObjPath,
        float frame)
    {
        // トラジション演出

        // 描画先キャンバスを生成
        GameObject canvas = new GameObject("Canvas");
        canvas.name = "FadeCanvas";

        canvas.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);

       // Canvas c = canvas.AddComponent<Canvas>();
        //c.renderMode = RenderMode.ScreenSpaceOverlay;
        
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject obj = MyInstantiate( FadeObjPath , canvas);
        obj.GetComponent<FadeoutScript>().addNum = (1.0f / frame);
        obj.name = "Fade";
        yield return Wait( (int)frame + 10 );

        // 移動
        SceneManager.LoadScene(sceneName);


        // トラジション終了
        yield return 0;
    }

    // ここら辺の処理は、書き直す
    public static IEnumerator MoveScene( string sceneName )
    {
        yield return MoveScene(sceneName, "Prefabs\\Fade\\Fadeout2", 60);
    }

}
 