using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * パーティー情報を管理するクラス
 * ( 全てのシーンで利用 )
 */
class ManegementParty
{
    /* パーティー編成名・パーティーID */
    int id; // 編成ID
    string name; // 編成名

    /* 所属しているキャラクター */
    PartyCharacter[] partyCharacter; // パーティーにいるキャラクター
}

