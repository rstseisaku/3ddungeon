using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static IEnumerator Wait( int frame )
    {
        for (int i = 0; i < frame; i++)
            yield return 0;
    }

    // ファイル名からテクスチャを返す
    public static Texture2D MyGetTexture(string FilePath)
    {
        return Resources.Load<Texture2D>(FilePath);
    }
}