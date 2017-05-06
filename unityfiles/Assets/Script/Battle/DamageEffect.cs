using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    // 直前に表示したものを格納
    public static GameObject atkObj; // 攻撃者のイラスト
    public static GameObject targetObj; // ターゲットのイラスト
    public static GameObject damageObj; // ダメージテキスト
    public static GameObject cmbObj; // コンボテキスト
    public static GameObject effObj; // エフェクトオブジェクト

    // ダメージ算出
    public static int CalDamage(BaseCharacter actionChara ,ComboManager cm)
    {
        int damage = actionChara.cs.atk;
        if (actionChara.isMagic) damage *= 2;
        damage = (damage * cm.magnificationDamage) / 100;
        return damage;
    }

    // ダメージの表示
    public static GameObject DrawDamage(int damage)
    {
        // カーソルオブジェクトの表示
        GameObject battleCanvas = GameObject.Find("Canvas");
        string FilePath = "Prefabs\\Battle\\AttackText";
        damageObj = Utility._Object.MyInstantiate(
            FilePath,
            battleCanvas);
        damageObj.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        damageObj.GetComponent<Text>().text = "" + damage;
        damageObj.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        return damageObj;
    }

    // コンボの表示
    public static GameObject DrawCombo(ComboManager cm)
    {
        // カーソルオブジェクトの表示
        GameObject battleCanvas = GameObject.Find("Canvas");
        string FilePath = "Prefabs\\Battle\\AttackText";
        cmbObj = Utility._Object.MyInstantiate(
            FilePath,
            battleCanvas);
        cmbObj.GetComponent<RectTransform>().localPosition += new Vector3(40, 260, 0);
        cmbObj.GetComponent<Text>().fontSize = 68;
        cmbObj.GetComponent<Text>().text = cm.comboString;
        return cmbObj;
    }
    // 攻撃者のグラフィックを表示
    public static GameObject DrawAttackChara(BaseCharacter actionChara)
    {
        // Image オブジェクト生成
        GameObject battleCanvas = GameObject.Find("Canvas");
        string FilePath = "Prefabs\\Battle\\ImageBase";
        // 顔グラオブジェクトの生成
        atkObj =
            Utility._Object.MyInstantiate(FilePath, battleCanvas, actionChara.cs.faceGraphicPath);
        // サイズ設定
        atkObj.transform.localScale =
            new Vector2(ConstantValue.BATTLE_ATTACKFACE_SIZE, ConstantValue.BATTLE_ATTACKFACE_SIZE);
        return atkObj;
    }
    // ターゲットの顔グラを表示
    public static GameObject TargetGraphicDraw(BaseCharacter bc)
    {
        // Image オブジェクト生成
        GameObject battleCanvas = GameObject.Find("Canvas");
        string FilePath = "Prefabs\\Battle\\ImageBase";
        // 顔グラオブジェクトの生成
        targetObj = (GameObject)Instantiate(Resources.Load(FilePath),
                            new Vector3(0, 0, 0),
                            Quaternion.identity);
        // 作成した Image オブジェクトにテクスチャを貼り付ける
        targetObj.GetComponent<Image>().sprite =
            bc.ctbFaceObj.faceObj.GetComponent<Image>().sprite;
        // Canvas を親オブジェクトに設定
        targetObj.transform.SetParent(battleCanvas.transform, false);
        // サイズ設定
        targetObj.GetComponent<Image>().transform.localScale =
            new Vector2(ConstantValue.BATTLE_ATTACKFACE_SIZE, ConstantValue.BATTLE_ATTACKFACE_SIZE);
        return targetObj;
    }
    // エッフェクト
    public static GameObject AttackEffect( int effectId )
    {
        // カーソルオブジェクトの表示
        string FilePath = "Prefabs\\Effect\\Effect" + effectId;
        effObj = (GameObject)Instantiate(Resources.Load(FilePath));
        effObj.GetComponent<ParticleSystem>().Play();
        return effObj;
    }


    public static void DestroyDamageObjects()
    {
        Destroy(damageObj);
        Destroy(cmbObj);
    }

    public static void DestroyAllObject()
    {
        Destroy(damageObj);
        Destroy(cmbObj);
        Destroy(effObj);
        Destroy(atkObj);
        Destroy(targetObj);
    }
}
