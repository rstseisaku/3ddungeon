﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


class mSaveData : MonoBehaviour
{
    public SaveObject saveObject;
    private bool isEnable;


    void Start()
    {
        /* 消さない */
        Debug.Log(GameObject.Find(Variables.Save.Name));
        if ( GameObject.Find(Variables.Save.Name) != null)
        {
            Destroy(this.gameObject);
        }

        gameObject.name = Variables.Save.Name;
        DontDestroyOnLoad(this);

        /* セーブデータをロード */
        saveObject = (SaveObject)LoadSaveData();
        if ( saveObject == null )
        {
            Debug.Log("セーブデータがないです。");

            /* データを初期化 */
            saveObject = new SaveObject();
            saveObject.NewVariables();
            saveObject.InitSavedata();

            /* セーブデータを出力 */
            MakeSaveData();
        }
        else
        {
            Debug.Log("セーブデータはあった。");
            DebugView();
        }
        isEnable = true;
    }

    /* ロードが終わるまで待機(なうろーでぃんぐ) */
    public IEnumerator WaitLoad()
    {
        while (!isEnable) yield return 0;
    }

    public void DebugView()
    {
        /*
        // セーブデータの中身見る
        for (int j = 0; j < Variables.Party.Num; j++)
        {
            Debug.Log("PT ID=" + j);
            for (int i = 0; i < Variables.Party.CharaNumPerParty; i++)
            {
                Debug.Log("" + i + "," + j + "] " + saveObject.saveParty.partyCharacterId[j, i]);
            }
        }
        */
    }

    /* セーブデータを取得する関数 */
    public object LoadSaveData()
    {
        /* セーブデータの生成 */
        string curDir = Directory.GetCurrentDirectory();
        FileStream fs = null;
        object obj = null;

        try
        {
            fs = new FileStream(
              curDir + "\\Save.dat",
              FileMode.Open,
              FileAccess.Read);
            BinaryFormatter f = new BinaryFormatter();
            //読み込んで逆シリアル化する
            obj = f.Deserialize(fs);
        }
        catch { }
        finally
        {
            if( fs != null)
                fs.Close();
        }

        return obj;
    }

    /* セーブデータを生成する関数 */
    public bool MakeSaveData()
    {
        DebugView();

        /* セーブデータの生成 */
        string curDir = Directory.GetCurrentDirectory();

        FileStream fs = new FileStream(
            curDir + "\\Save.dat",
            FileMode.Create,
            FileAccess.Write);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, saveObject); //シリアル化して書き込む
        fs.Close();

        Debug.Log("何か吐き出した");

        return true; // セーブ成功
    }



    /* セーブデータオブジェクトの編集 */
    public void EditSaveParty( Party pt , int editId)
    {
        GetSaveParty().partyName[editId] = pt.partyName;
        for( int i=0; i<Variables.Party.CharaNumPerParty; i++)
        {
            GetSaveParty().partyCharacterId[editId, i] = pt.partyCharacterId[i];
        }
    }
      
    /* セーブデータオブジェクトのアクセス */
    public SaveParty GetSaveParty() { return saveObject.saveParty; }
    public ObtainChara GetObtainChara() { return saveObject.obtainChara; }
}