using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * パーティー情報を管理するクラス
 * ( 全てのシーンで利用 )
 */
 public class ManegementParty : MonoBehaviour
{
    public int mainPartyId;
    public Party[] partyList;

    void Start()
    {
        // パーティーを表示
        mainPartyId = 3;
        partyList = new Party[6];
        for (int i = 0; i < partyList.Length; i++)
        {
            partyList[i] = gameObject.AddComponent<Party>();
            partyList[i].Init( "パーティー " + i );
        }
    }
}



/*
 * パーティ１つで保持する情報を管理するオブジェクト
 */
public class Party : MonoBehaviour
{
    /* パーティー編成名・パーティーID */
    int id; // 編成ID
    string name; // 編成名
    public int partyNum; // パーティー人数

    /* 所属しているキャラクター */
    public PartyCharacter[] partyCharacter; // パーティーにいるキャラクタ


    public void Init(string _name)
    {
        // 名前受け取り
        name = _name;

        // デバッグ
        partyCharacter = new PartyCharacter[5];
        for (int i = 0; i < partyCharacter.Length; i++)
        {
            partyCharacter[i] = gameObject.AddComponent<PartyCharacter>();
            partyCharacter[i].Init();
            partyCharacter[i].atkAdd += 600;
        }
        partyNum = 5;
    }
}

