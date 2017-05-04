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
    public string partyName; // 編成名

    /* 所属しているキャラクター */
    public PartyCharacter[] partyCharacter; // パーティーにいるキャラクタ
    public int[] partyCharacterId;


    /* 領域確保だけ行う */
    public void NewVariables()
    {
        partyCharacter = new PartyCharacter[Variables.Party.CharaNumPerParty];
        partyCharacterId = new int[Variables.Party.CharaNumPerParty];
    }

    /* partyCharacterId を元にロードする処理 */
    public void LoadFromPartyCharacterId()
    {
        for (int i = 0; i < Variables.Party.CharaNumPerParty; i++)
        {
            partyCharacter[i] = gameObject.AddComponent<PartyCharacter>();
            partyCharacter[i].Init(partyCharacterId[i]);
        }
    }

    public void Init(string _name)
    {
        // 旧

        /*
        // 名前受け取り
        partyName = _name;

        // デバッグ
        partyCharacter = new PartyCharacter[Variables.Party.CharaNumPerParty];
        partyCharacterId = new int[Variables.Party.CharaNumPerParty];
        for (int i = 0; i < partyCharacter.Length; i++)
        {
            partyCharacter[i] = gameObject.AddComponent<PartyCharacter>();
            if (i <= 2)
            {
                partyCharacter[i].Init(i+1);
                partyCharacterId[i] = 1;
            }
            else
            {
                partyCharacter[i].Init(-1);
                partyCharacterId[i] = -1;
            }
        }
        */
    }
}

