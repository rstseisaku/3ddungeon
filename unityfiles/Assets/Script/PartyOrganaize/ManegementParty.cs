using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EDIT_PARTY_STATUS
{
    _PARTY_SELECT,
    _CHANGE_CHARACTER,
    _JOIN_CHARACTER,
    _DECIDED_JOIN_CHARACTER,
    _END
}

/*
 * パーティー情報を管理するクラス
 * ( 全てのシーンで利用 )
 */
class ManegementParty : MonoBehaviour
{
    public int mainPartyId;
    public Party editParty;

    /* 状態管理 */
    EDIT_PARTY_STATUS editPartyStatus;
    private int editPartyId = -1;
    private int changeCharaPartyId = -1;
    private int joinCharaId = -1;

    /* パーティのセーブデータ */
    private mSaveData saveData;

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
        yield return Init(); // セーブデータの読み込みにウェイトを挟む

        /* メインループ */
        editPartyStatus = EDIT_PARTY_STATUS._CHANGE_CHARACTER;
        editPartyStatus = EDIT_PARTY_STATUS._PARTY_SELECT;
        changeCharaPartyId = -1;
        while (true)
        {
            switch (editPartyStatus)
            {
                // 編集するパーティの選択
                case EDIT_PARTY_STATUS._PARTY_SELECT:
                    yield return DecideEditParty.Loop(saveData.GetSaveParty());
                    editPartyId = DecideEditParty.editPartyId;
                    editPartyStatus = DecideEditParty.editPartyStatus;
                    break;
                // 編成対象となるキャラクターの選択            
                case EDIT_PARTY_STATUS._CHANGE_CHARACTER:
                    SetEditParty(); // editPartyId に応じて、パーティ情報を格納
                    yield return DecideChangeCharacter.Loop(editParty);
                    changeCharaPartyId = DecideChangeCharacter.changeCharaPartyId;
                    editPartyStatus = DecideChangeCharacter.editPartyStatus;
                    break;
                // 誰を加入させるか選択
                case EDIT_PARTY_STATUS._JOIN_CHARACTER:
                    yield return DecideJoinCharacter.Loop(saveData.GetObtainChara());
                    joinCharaId = DecideJoinCharacter.joinCharaId;
                    editPartyStatus = DecideJoinCharacter.editPartyStatus;
                    break;
                // 実際の入れ替え処理
                case EDIT_PARTY_STATUS._DECIDED_JOIN_CHARACTER:
                    ChangeMember();
                    SaveParty();
                    editPartyStatus = EDIT_PARTY_STATUS._CHANGE_CHARACTER;
                    break;
                case EDIT_PARTY_STATUS._END:
                    yield return Utility._Scene.MoveScene("Base", "Images\\Background\\Black",90);
                    break;
                default:
                    break;
            }
            // ふりーずぼーしいい！！！！！！
            yield return 0;
        }


    }

    /* 初期化 */
    IEnumerator Init()
    {
        /* セーブデータが存在すれば読み込む */
        saveData = GameObject.Find(Variables.Save.Name).GetComponent<mSaveData>();
        yield return saveData.WaitLoad();
    }

    /* セーブの反映 */
    public void SaveParty()
    {
        saveData.EditSaveParty(editParty, editPartyId);
        saveData.MakeSaveData();
    }

    /* パーティのロード */
    public void SetEditParty()
    {
        // 例外処理
        if (editPartyId < 0)
        {
            Debug.LogError("editPartyId が初期化されてないです(´･ω･｀)");
            return;
        }

        editParty = gameObject.AddComponent<Party>();
        editParty.NewVariables();

        editParty.partyName = saveData.GetSaveParty().partyName[editPartyId];
        for( int i=0; i<Variables.Party.CharaNumPerParty; i++)
        {
            editParty.partyCharacterId[i] = saveData.GetSaveParty().partyCharacterId[editPartyId,i];
        }
        editParty.LoadFromPartyCharacterId();
    }

    /* 全パーティーを表示 */
    public void DrawAllParty()
    {
    }

    /* メンバー入替 */
    // 編成ID・交換キャラパーティID・参加キャラIDの3パラメタ利用
    public void ChangeMember()
    {
        // パーティから外す場合の処理
        if (joinCharaId < 0 ) joinCharaId = -1;

        // キャラデータのロード
        editParty.partyCharacterId[changeCharaPartyId] = joinCharaId;
        editParty.partyCharacter[changeCharaPartyId].LoadCharacterData(joinCharaId,saveData.GetObtainChara());
        if (joinCharaId < 0) return;

        // ステータス割振値を取得
        editParty.partyCharacter[changeCharaPartyId].atkAdd = saveData.GetObtainChara().atkAdd[joinCharaId];
        editParty.partyCharacter[changeCharaPartyId].maxHpAdd = saveData.GetObtainChara().maxHpAdd[joinCharaId];
    }
}