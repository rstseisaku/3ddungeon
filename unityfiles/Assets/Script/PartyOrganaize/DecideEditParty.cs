using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideEditParty : MonoBehaviour {
    static GameObject decEditPartyObj;
    static mDecPartyObj decPartyObjScript;

    public static int editPartyId;
    public static EDIT_PARTY_STATUS editPartyStatus;

    /* 編集パーティを選ぶ */
    public static IEnumerator Loop(SaveParty saveParty)
    {
        yield return Loop(saveParty,"【編成終了】");
    }

    public static IEnumerator Loop(SaveParty saveParty, string returnString)
    {
        /* パーティーを表示 */
        DrawParty(saveParty);
        SetReturnString(returnString);        

        /* 制御部分 */
        while (true)
        {
            if (decPartyObjScript.isDecided)
            {
                editPartyId = decPartyObjScript.touchedId;
                break;
            }
            yield return 0;
        }

        /* 次状態へ遷移 */
        if (editPartyId >= 0) editPartyStatus = EDIT_PARTY_STATUS._CHANGE_CHARACTER;
        if (editPartyId == -2) editPartyStatus = EDIT_PARTY_STATUS._END;

        /* 削除 */
        Destroy(decEditPartyObj);
        decPartyObjScript = null;
    }


    private static void DrawParty(SaveParty saveParty)
    {
        // インスタンス化
        GameObject canvas = GameObject.Find("PartyCanvas");
        if (canvas == null)
        {
            canvas = Utility._Object.GenerateCanvas(5);
            canvas.name = "PartyCanvas";
        }

        string FilePath = "Prefabs\\Party\\DecEditParty";
        decEditPartyObj = Utility._Object.MyInstantiate(FilePath, canvas);
        decPartyObjScript = decEditPartyObj.GetComponent<mDecPartyObj>();
        decPartyObjScript.GenerateObject(saveParty);
    }

    public static void SetReturnString( string str )
    {
        decPartyObjScript.SetReturnString(str);
    }
}
