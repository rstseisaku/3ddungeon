using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class DecideChangeCharacter : MonoBehaviour
{
    // TODO: もっといいやり方あるはず

    // 戻り値として扱う変数
    public static int changeCharaPartyId;
    public static EDIT_PARTY_STATUS editPartyStatus;

    // 内部で利用する変数
    private static GameObject partyObj;
    private static mPartyObject mPartyScript;

    /* 交換キャラクターを選択 */
    public static IEnumerator Loop(Party party)
    {
        /* パーティーを表示 */
        DrawParty(party);

        /* 制御部分 */
        while (true)
        {
            if (mPartyScript.touchId != -1)
            {
                changeCharaPartyId = mPartyScript.touchId;
                break;
            }
            yield return 0;
        }

        /* 次状態算出 */
        if (changeCharaPartyId == -2) editPartyStatus = EDIT_PARTY_STATUS._PARTY_SELECT;
        if (changeCharaPartyId >= 0) editPartyStatus = EDIT_PARTY_STATUS._JOIN_CHARACTER;

        /* 削除 */
        Destroy(partyObj);
        mPartyScript = null;
    }

    /* パーティーを表示 */
    public static void DrawParty(Party party)
    {
        // インスタンス化
        GameObject canvas = GameObject.Find("PartyCanvas");
        if ( canvas == null)
        {
            canvas = Utility._Object.GenerateCanvas(5);
            canvas.name = "PartyCanvas";
        }

        string FilePath = "Prefabs\\Party\\Party";
        partyObj = Utility._Object.MyInstantiate(FilePath, canvas);
        mPartyScript = partyObj.GetComponent<mPartyObject>();
        mPartyScript.FindObjectAddress();
        mPartyScript.SetPartyName(party.partyName);

        // キャラクタの情報をセットしていく
        for (int i = 0; i < party.partyCharacter.Length; i++)
        {
            PartyCharacter pChara = party.partyCharacter[i];
            if (pChara.characterId == -1)
            {
                mPartyScript.SetStandImage(i, "Images/Stand/c999");
                mPartyScript.FrameSetActive(i, false);
                continue;
            }

            mPartyScript.SetHpText(i, pChara.cs.maxHp);
            mPartyScript.SetStandImage(i, pChara.cs.standGraphicPath);
            mPartyScript.FrameSetActive(i, true);
        }
    }
}