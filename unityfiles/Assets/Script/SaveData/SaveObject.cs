using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


[System.Serializable()]
public class SaveObject
{
    /* 所持する値 */
    public SaveParty saveParty;
    public ObtainChara obtainChara;


    /* 変数定義レベルの初期化 */
    public void NewVariables()
    {
        saveParty = new SaveParty();
        obtainChara = new ObtainChara();
        saveParty.NewVariables();
        obtainChara.NewVariables();
    }


    /* セーブデータの初期化 */
    public void InitSavedata()
    {
        saveParty.InitPartyData();
        obtainChara.InitUserData();
    }
}

