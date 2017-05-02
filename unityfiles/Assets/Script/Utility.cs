﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Utility
{
    public class _Wait : MonoBehaviour
    {
        // 止まらない？ ⇒ yield return 忘れてない？
        public static IEnumerator WaitFrame(int frame)
        {
            for (int i = 0; i < frame; i++)
                yield return 0;
        }

        //  止まらない？ ⇒ mInput 扱うオブジェクト生きてる？
        public static IEnumerator WaitKey()
        {
            while (mInput.existNewTouch == -1) // 新押があるまで待機
            {
                yield return 0;
            }
        }
    }

    public class _Image : MonoBehaviour {
        // ファイル名からテクスチャを返す
        public static Texture2D MyGetTexture(string FilePath)
        {
            return Resources.Load<Texture2D>(FilePath);
        }
        // ファイル名からスプライトを返す
        public static Sprite MyGetSprite(string FilePath)
        {
            Texture2D t = MyGetTexture(FilePath);
            if (t == null) { Debug.LogError("ファイル " + FilePath + "は見つかりません"); return null; }
            Sprite sp = Sprite.Create(t,
                    new Rect(0, 0, t.width, t.height),
                    Vector2.zero);
            return sp;
        }
    }

    public class _Object : MonoBehaviour
    {
        // ファイル名からオブジェクトを返す
        public static GameObject MyInstantiate(string FilePath)
        {
            return MyInstantiate(FilePath, null);
        }

        // オブジェクト名からオブジェクトを返す
        public static GameObject MyFind( string ObjectName)
        {
            GameObject obj = GameObject.Find(ObjectName);            
            if (obj == null)
            {
                _Error.ObjectNotFound(ObjectName);
            }
            return obj;
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
            Texture2D t = _Image.MyGetTexture(FilePathTexture);
            Image image = obj.GetComponent<Image>();
            if (image != null)
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

        // キャンパスの生成
        public static GameObject GenerateCanvas()
        {
            GameObject canvas = new GameObject("Canvas");
            canvas.name = "FadeCanvas";

            canvas.AddComponent<GraphicRaycaster>();

            CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);

            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            return canvas;
        }
        // キャンパスの生成
        public static GameObject GenerateCanvas( int orderInLayer)
        {
            GameObject canvas = GenerateCanvas();
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            canvas.transform.SetParent(GameObject.Find("Main Camera").transform, false);
            canvas.GetComponent<Canvas>().sortingOrder = orderInLayer;
            return canvas;
        }
        // オブジェクト生成
        public static GameObject MyGenerateImage(
            string Path,
            GameObject parent,
            Vector2 vec)
        {
            GameObject obj = new GameObject("OBJECT");
            Image img = obj.AddComponent<Image>();
            RectTransform rt = obj.GetComponent<RectTransform>();

            /* オブジェクトの加工 */
            obj.transform.SetParent(parent.transform, false);
            img.sprite = _Image.MyGetSprite(Path);
            rt.sizeDelta = vec;

            return obj;
        }
    }

    public class _Scene : MonoBehaviour
    {
        public static IEnumerator MoveScene(
            string sceneName,
            string FadeImagePath,
            float frame)
        {
            /*
             * トラジション演出
             */
            // 描画先キャンバスを生成
            GameObject canvas = _Object.GenerateCanvas();

            // トランジションオブジェクト
            GameObject obj = _Object.MyInstantiate(
                "Prefabs\\Fade\\Trasition",
                canvas,
                FadeImagePath,
                new Vector2(1280, 960));
            obj.GetComponent<fuck.Transition>().rule = _Image.MyGetTexture("Images\\Transition\\transition_1");
            obj.GetComponent<fuck.Transition>().mode = fuck.Transition.TRANSITION_MODE._FADEIN;
            obj.GetComponent<fuck.Transition>().time = (frame / 60);
            obj.name = "Fade";
            yield return _Wait.WaitFrame((int)frame + 10);

            // 移動
            SceneManager.LoadScene(sceneName);

            // トラジション終了
            yield return 0;
        }

        // ここら辺の処理は、書き直す
        public static IEnumerator MoveScene(string sceneName)
        {
            yield return MoveScene(sceneName, "Images\\Background\\Background1", 60);
        }
    }

    public class _Encount : MonoBehaviour
    {
        public static IEnumerator Encount( int enemyGroupId )
        {
            /* エンカウントオブジェクトの取得 */
            GameObject obj = _Object.MyFind(Variables.Enemy.EnemyGroupObjectName);
            EnemyGroup eg = obj.GetComponent<EnemyGroup>();
            eg.enemyGroupId = enemyGroupId;
            eg.LoadCharacterIdFromGroupId();

            yield return _Scene.MoveScene("battleScene");
        }
    }

    public class _Error : MonoBehaviour
    {
        public static void ObjectNotFound(string str)
        {
            Debug.LogError("オブジェクト名: " + str +" は見つかりませんでした。");
        }
    }

}
