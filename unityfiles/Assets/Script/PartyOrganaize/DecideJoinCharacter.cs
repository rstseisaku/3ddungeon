using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class DecideJoinCharacter : MonoBehaviour
{
    // TODO: もっといいやり方あるはず

    // 戻り値として扱う変数
    public static int joinCharaId;
    public static EDIT_PARTY_STATUS editPartyStatus;

    // 内部で利用する変数
    private static GameObject partyObj;
    private static HaveChara haveCharaScript;

    /* 交換キャラクターを選択 */
    public static IEnumerator Loop(ObtainChara obtainChara)
    {
        DrawParty(obtainChara);

        /*  */
        while (true)
        {
            if( haveCharaScript.touchId != -1)
            {
                joinCharaId = haveCharaScript.touchId;
                break;
            }
            yield return 0;
        }

        /* 次状態算出 */
        if (joinCharaId == -2) editPartyStatus = EDIT_PARTY_STATUS._CHANGE_CHARACTER;
        if (joinCharaId >= 0) editPartyStatus = EDIT_PARTY_STATUS._DECIDED_JOIN_CHARACTER;

        /* 削除 */
        Destroy(partyObj);
        haveCharaScript = null;
    }

    /* パーティーを表示 */
    public static void DrawParty(ObtainChara obtainChara)
    {
        // インスタンス化
        GameObject canvas = GameObject.Find("PartyCanvas");

        string FilePath = "Prefabs\\Party\\HaveChara";
        partyObj = Utility.MyInstantiate(FilePath, canvas);
        haveCharaScript = partyObj.GetComponent<HaveChara>();
        haveCharaScript.GenerateObject( obtainChara.isObtainChara );
    }
}