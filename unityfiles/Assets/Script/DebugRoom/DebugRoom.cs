using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ---- ここから拡張コード ---- */
#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになる
using UnityEditor.SceneManagement;
#endif



enum DebugType
{
    Encount,
    Other
}

/* デバッグルームのクラス */
class DebugRoom : MonoBehaviour
{
    public int _encountGroupId;
    public int[] _partyCharacterId;
    public DebugType debugMode;
    public mSaveData _saveData;

    private void Update()
    {
        // ゲームがはじまると起爆する。
        // Debug.Log(debugMode);
    }

    public void onClick()
    {
        if( debugMode == DebugType.Encount)
        {
            StartCoroutine("Encount");
        }
    }

    IEnumerator Encount()
    {
        // エンカウント準備
        GameObject obj = new GameObject();
        obj.AddComponent<EnemyGroup>();
        GameObject obj2 = new GameObject();
        _saveData = obj2.AddComponent<mSaveData>();

        // 1フレウェイトでもセットは終わりそうだけど。
        yield return Utility._Wait.WaitFrame(10);
        yield return obj2.GetComponent<mSaveData>().WaitLoad();

        _saveData.GetSaveParty().mainParty = 0;
        for (int i = 0; i < _partyCharacterId.Length; i++)
        {
            _saveData.GetSaveParty().partyCharacterId[0, i] = _partyCharacterId[i];
        }

        _saveData.StartAdventure();
        yield return Utility._Encount.Encount(_encountGroupId);
    }
}




// メモ: 欲しい機能
//  ・味方の情報をcsvから読み込み表示(ゲーム画面上？)
//　・味方のデータをインスペクタ上で編集してcsvに反映
//　・敵グループデータをインスペクタ上で編集してcsvに反映
//　・敵データをインスペクタ上で編集してcsvに反映
#if UNITY_EDITOR
[CustomEditor(typeof(DebugRoom))]
public class CustomDebugRoom : Editor
{
    DebugRoom dr;

    DebugType dt;
    int groupId;
    int[] partyId;

    public void OnEnable()
    {
        // 設定されているプロパティを取得する
        // debugMode =  serializedObject.FindProperty("debugMode");

        // 処理コードのインスタンス
        dr = (DebugRoom)target;
        partyId = dr._partyCharacterId;
        if (partyId == null) // これ二度と呼ばれなくね…
        {
            partyId = new int[Variables.Party.CharaNumPerParty];
            for (int i = 0; i < partyId.Length; i++) partyId[i] = 0;
            dr._partyCharacterId = partyId;
        }

        // 選ばれたときに呼ばれる？
        Debug.Log("OnEnable");
    }

    public override void OnInspectorGUI()
    {
        dt = dr.debugMode;
        groupId = dr._encountGroupId;
        partyId = dr._partyCharacterId;


        EditorGUI.BeginChangeCheck();

        /*
        // serializedObject をアクセス前に最新情報に更新
        // serializedObject.Update();
        //
        // プロパティ表示＆反映
        // EditorGUILayout.PropertyField("debugMode);
        // serializedObject.ApplyModifiedPropertiesWithoutUndo();
        */

        // デバッグモードのプロパティを表示
        dt = (DebugType)EditorGUILayout.EnumPopup("デバッグモード", (DebugType)dr.debugMode);



        if (dt == DebugType.Encount)
        {
            groupId = (int)EditorGUILayout.IntField("敵グループのID", (int)dr._encountGroupId); // グループのID
            for (int i = 0; i < Variables.Party.CharaNumPerParty; i++)
            {
                partyId[i] = (int)EditorGUILayout.IntField("味方ID", partyId[i]);
                if (partyId[i] == 0) partyId[i] = 1;
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            // Undoの登録
            Undo.RecordObject(dr, "ChangeUp");

            // 登録
            dr.debugMode = dt;
            dr._encountGroupId = groupId;
            dr._partyCharacterId = partyId;

            Debug.Log("Changed");
        }

        if (GUILayout.RepeatButton("Save"))
        {
            // 現在シーンだけ保存
            // 値を変更⇒Undoを実装する必要があるらしい。
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("Saved");
        }
    }
}

#endif