using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/*
 * パーティについてのセーブデータを管理するクラス
 */
[System.Serializable()]
public class SaveParty
{
    /* 所持する値 */
    public string[] partyName; // パーティーの名前
    [SerializeField]
    public int[,] partyCharacterId; // キャラクターのID
    public int mainParty; // メインパーティ

    /* 変数の領域確保 */
    public void NewVariables()
    {
        partyName = new string[Variables.Party.Num];
        partyCharacterId = new int[Variables.Party.Num,Variables.Party.CharaNumPerParty];
    }
    public void InitPartyData()
    {
        // パーティのデータを初期化
        mainParty = 0;
        for (int i = 0; i < Variables.Party.Num; i++) partyName[i] = "パーティ " + (i+1);
        for (int i = 0; i< Variables.Party.Num; i++)
        {
            for (int j = 0; j < Variables.Party.CharaNumPerParty; j++)
            {
                partyCharacterId[i,j] = -1;
            }
        }
        // 初期パーティを適当に設定
        partyCharacterId[0, 0] = 1;
    }
}