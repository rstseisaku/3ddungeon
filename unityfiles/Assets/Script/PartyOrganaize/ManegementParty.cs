using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


enum EDIT_PARTY_STATUS
{
    _PARTY_SELECT,
    _CHANGE_CHARACTER,
    _JOIN_CHARACTER,
    _DECIDED_JOIN_CHARACTER
}

/*
 * パーティー情報を管理するクラス
 * ( 全てのシーンで利用 )
 */
 public class ManegementParty : MonoBehaviour
{
    public int mainPartyId;
    public Party[] partyList;
    public ObtainChara obtainChara;

    EDIT_PARTY_STATUS editPartyStatus;
    public int editPartyId;
    public int changeCharaPartyId;
    public int joinCharaId;

    GameObject canvas;

    void Start()
    {
        StartCoroutine("MainLoop");
    }



    /*
     * ================================================
     * ゲームメインループ
     * ================================================
     */
    IEnumerator MainLoop()
    {
        /* 初期化処理 */
        Init();

        /* メインループ */
        editPartyStatus = EDIT_PARTY_STATUS._CHANGE_CHARACTER;
        editPartyId = mainPartyId;
        changeCharaPartyId = -1;
        while (true)
        {
            switch (editPartyStatus)
            {
                // 編成対象となるキャラクターの選択            
                case EDIT_PARTY_STATUS._CHANGE_CHARACTER:
                    yield return DecideChangeCharacter.Loop(partyList[editPartyId]);
                    changeCharaPartyId = DecideChangeCharacter.changeCharaPartyId;
                    editPartyStatus = DecideChangeCharacter.editPartyStatus;
                    break;
                // 誰を加入させるか選択
                case EDIT_PARTY_STATUS._JOIN_CHARACTER:
                    yield return DecideJoinCharacter.Loop(obtainChara);
                    joinCharaId = DecideJoinCharacter.joinCharaId;
                    editPartyStatus = DecideJoinCharacter.editPartyStatus;
                    break;
                // 誰を加入させるか選択
                case EDIT_PARTY_STATUS._DECIDED_JOIN_CHARACTER:
                    ChangeMember();
                    editPartyStatus = EDIT_PARTY_STATUS._CHANGE_CHARACTER;
                    break;
                    // 【未定義】
                default:
                    break;
            }
            // ふりーずぼーしいい！！！！！！
            yield return 0;
        }


    }

    /* 初期化 */
    public void Init()
    {
        /* オブジェクト読み込み */
        canvas = GameObject.Find("PartyCanvas");


        /* セーブデータが存在すれば読み込む */


        /* 初期化 */
        InitAllParty();
        obtainChara = gameObject.AddComponent<ObtainChara>();
        obtainChara.Init(); // 初期化
    }

    /* 全パーティー初期化 */
    public void InitAllParty()
    {
        mainPartyId = 0;
        partyList = new Party[6];
        for (int i = 0; i < partyList.Length; i++)
        {
            // パーティーを1つ生成
            partyList[i] = gameObject.AddComponent<Party>();
            // パーティーの中身を初期化(1キャラ+ 4つの空白)
            partyList[i].Init( "パーティー " + i);
        }
    }

    /* 全パーティーを表示 */
    public void DrawAllParty()
    {
    }

    /* メンバー入替 */
    // 編成ID・交換キャラパーティID・参加キャラIDの3パラメタ利用
    public void ChangeMember()
    {
        // キャラデータのロード
        partyList[editPartyId].partyId[changeCharaPartyId] = joinCharaId;
        partyList[editPartyId].partyCharacter[changeCharaPartyId].Init(joinCharaId);
        // ステータス割振値を取得
        partyList[editPartyId].partyCharacter[changeCharaPartyId].atkAdd = ObtainChara.atkAdd[joinCharaId];
        partyList[editPartyId].partyCharacter[changeCharaPartyId].maxHpAdd = ObtainChara.maxHpAdd[joinCharaId];
    }
}