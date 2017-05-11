using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * パーティ１つが保持する情報を管理するオブジェクト
 * ( パーティ編成画面・ダンジョン画面・戦闘(読み込み時)に利用 )
 */
public class Party : MonoBehaviour
{
    /* パーティー編成名・パーティーID */
    public string partyName; // 編成名
    public string nowScene;

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
            partyCharacter[i].LoadCharacterData(partyCharacterId[i]);
        }
    }
    public void LoadFromPartyCharacterId(ObtainChara oc)
    {
        for (int i = 0; i < Variables.Party.CharaNumPerParty; i++)
        {
            partyCharacter[i] = gameObject.AddComponent<PartyCharacter>();
            partyCharacter[i].LoadCharacterData(partyCharacterId[i],oc);
        }
    }

    /* 外部提供用インタフェース */
    // パーティーキャラクターのオブジェクトをそのままを返す
    public PartyCharacter GetPartyCharacter(int partyId)
    {
        return partyCharacter[partyId];
    }
    // 振分値を反映した値を返す
    public CharacterStatus GetPartyCharacterBattleStatus(int partyId)
    {
        // 編成されていなければ null を返す
        if( partyCharacterId[partyId] < 0) { return null; }
        return partyCharacter[partyId].bcs;
    }
    public int GetNowHp(int partyId)
    {
        // 編成されていなければ -1 を返す
        if (partyCharacterId[partyId] < 0) { return -1; }
        return partyCharacter[partyId].hp;
    }
}

