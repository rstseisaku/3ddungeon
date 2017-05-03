using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * シーンをまたいで敵のグループIDを保存するためのID
 */
public class EnemyGroup : MonoBehaviour
{
    public int enemyGroupId;
    public int[] enemyCharacterId;
    public int enemyNum;

    private void Start()
    {
        /* 重複チェック */
        if (GameObject.Find(Variables.Enemy.EnemyGroupObjectName) != null)
        {
            Destroy(this.gameObject);
        }
        gameObject.name = Variables.Enemy.EnemyGroupObjectName;
        DontDestroyOnLoad(this);
    }

    public void LoadCharacterIdFromGroupId()
    {
        // .csv ファイルなどから読み込む
        if (enemyGroupId == 0)
        {
            Debug.LogError("敵グループの値に0が設定されました。(1として扱います(´･ω･｀))");
            enemyGroupId = 1;
        }

        // 設定ファイルを読込
        string[] buffer;
        buffer = System.IO.File.ReadAllLines(Variables.Enemy.EnemyGroupFilePath);

        // linebuffer にキャラクターの情報( characterId 番目の )を格納
        string[] linebuffer;
        linebuffer = buffer[enemyGroupId].Split(',');
        string[] str = linebuffer;

        int[] getIntDataFromStr = new int[str.Length];
        enemyNum = 0;
        for ( int i=0; i < getIntDataFromStr.Length; i++)
        {
            if (str[i] != "")
            {
                getIntDataFromStr[i] = int.Parse(str[i]);
                enemyNum++;
            }
            else
            {
                getIntDataFromStr[i] = -1;
            }
        }
        enemyCharacterId = new int[enemyNum];
        for( int i = 0; i < enemyNum; i++)
        {
            enemyCharacterId[i] = getIntDataFromStr[i];
        }
    }
}
