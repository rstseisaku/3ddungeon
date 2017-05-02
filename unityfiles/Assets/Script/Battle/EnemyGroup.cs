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
        //** デバッグ
        enemyCharacterId = new int[7];
        for( int i=0; i<enemyCharacterId.Length; i++)
        {
            enemyCharacterId[i] = i+1;
        }
        enemyNum = enemyCharacterId.Length;
    }
}
