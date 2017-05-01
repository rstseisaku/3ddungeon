using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * パーティ１つが保持する情報を管理するオブジェクト
 */
public class Party : MonoBehaviour
{
    /* パーティー編成名・パーティーID */
    int id; // 編成ID
    public string partyName; // 編成名
    public int partyNum; // パーティー人数

    /* 所属しているキャラクター */
    public PartyCharacter[] partyCharacter; // パーティーにいるキャラクタ
    public int[] partyId;

    public void Init(string _name)
    {
        // 名前受け取り
        partyName = _name;

        // デバッグ
        partyCharacter = new PartyCharacter[5];
        partyId = new int[5];
        for (int i = 0; i < partyCharacter.Length; i++)
        {
            partyCharacter[i] = gameObject.AddComponent<PartyCharacter>();
            if (i <= 2)
            {
                partyCharacter[i].Init(i+1);
                partyId[i] = 1;
            }
            else
            {
                partyCharacter[i].Init(-1);
                partyId[i] = -1;
            }
        }
        partyNum = 5;
    }
}

