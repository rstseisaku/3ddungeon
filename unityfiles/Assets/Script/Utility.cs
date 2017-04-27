using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utility : MonoBehaviour
{
    public static IEnumerator Wait( int frame )
    {
        for (int i = 0; i < frame; i++)
            yield return 0;
    }

    public static IEnumerator WaitKey()
    {
        while ( mInput.existNewTouch == -1 )
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

}