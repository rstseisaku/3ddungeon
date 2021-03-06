﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System;

public class mSaveData : MonoBehaviour
{
    public SaveObject saveObject;
    private bool isEnable;

    void Start()
    {
        /*
         * シーンをまたいでも消さないオブジェクトに登録
         * (パーティ編成などに利用)
         */
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

    internal SaveParty GetSaveParty(string v)
    {
        throw new NotImplementedException();
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
        BinaryFormatter bf = new BinaryFormatter();
        string serializedData = PlayerPrefs.GetString("SaveData");

        MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serializedData));
        object deserializedObject = (object)bf.Deserialize(dataStream);

        return deserializedObject;

        /*
        // セーブデータの生成
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
        */
    }

    /* セーブデータを生成する関数 */
    public bool MakeSaveData()
    {
        //DebugView();

        /* セーブデータの生成 */
        string curDir = Directory.GetCurrentDirectory();
        MemoryStream fs= new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, saveObject); //シリアル化して書き込む

        string tmp = System.Convert.ToBase64String(fs.ToArray());
        try
        {
            PlayerPrefs.SetString("SaveData", tmp);
        }
        catch (PlayerPrefsException)
        {
            return false;
        }

        fs.Close();
        return true;

#if UNITY_EDITOR
        /*
        FileStream fs = new FileStream(
            curDir + "\\Save.dat",
            FileMode.Create,
            FileAccess.Write);
            */

#elif UNITY_ANDROID
              WWW www = new WWW(
            curDir + "\\Save.dat");
       
        www.Dispose();
  
#endif

        return true; // セーブ成功
    }

    public void DeleteSaveData()
    {
        /* データを初期化 */
        saveObject = new SaveObject();
        saveObject.NewVariables();
        saveObject.InitSavedata();

        MakeSaveData();
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

    /* ダンジョン・バトルで利用するパーティオブジェクトを生成 */
    private void MakeDungenBattleParty()
    {
        GameObject obj;
        if ( (obj = GameObject.Find(Variables.Party.SingletonObjectName)) != null )
        {
            Destroy(obj);
        }

        obj = new GameObject(Variables.Party.SingletonObjectName);
        DontDestroyOnLoad(obj);

        Party p = obj.AddComponent<Party>();
        p.NewVariables();
        p.partyName = GetSaveParty().partyName[GetSaveParty().mainParty];
        for( int i=0; i<Variables.Party.CharaNumPerParty; i++)            
            p.partyCharacterId[i] = GetSaveParty().partyCharacterId[GetSaveParty().mainParty,i];
        p.LoadFromPartyCharacterId( GetObtainChara() );
    }

    /* 探索開始時に呼ぶ処理 */
    public void StartAdventure()
    {
        MakeDungenBattleParty();
        Destroy(this.gameObject);
    }

    /* セーブデータオブジェクトのアクセス */
    public SaveParty GetSaveParty() { return saveObject.saveParty; }
    public ObtainChara GetObtainChara() { return saveObject.obtainChara; }
}
