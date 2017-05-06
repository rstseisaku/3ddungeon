using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ユニゾン処理 */
public class Unison : MonoBehaviour
{
    public static IEnumerator DoUnison(BaseCharacter[] actionCharas,
        BaseCharacter TargetChara, 
        ComboManager cm)
    {
        // 演出を表示する(攻撃者の演出)
        GameObject canvas = GameObject.Find("Canvas");
        List<GameObject> uniObj = new List<GameObject>();
        int unisonCount = 0;
        int unisonSupport = 0;
        for ( int i=0; i< actionCharas.Length; i++)
        {
            if( actionCharas[i].ctbNum == 0 ){
                string str = actionCharas[i].cs.standGraphicPath;
                unisonSupport += actionCharas[i].cs.unisonSupport;
                GameObject obj = Utility._Object.MyGenerateImage(str, canvas, new Vector2(270, 540));
                obj.GetComponent<RectTransform>().localPosition = id2Pos(unisonCount);
                uniObj.Add( obj );

                unisonCount++;
                SoundManager.PlaySe(Variables.SE.SeName.battle_unison_chari);
                yield return Utility._Wait.WaitFrame(10);
            }
        }

        // ウェイト
        yield return Utility._Wait.WaitFrame(30);

        // ユニゾンのエフェクトを消す
        foreach (GameObject g in uniObj) Destroy(g);

        // 対象表示
        DamageEffect.TargetGraphicDraw(TargetChara);
        yield return Utility._Wait.WaitFrame(10);
        // 戦闘アニメーション
        SoundManager.PlaySe(Variables.SE.SeName.battle_unison_effect);
        DamageEffect.AttackEffect(1);
        yield return Utility._Wait.WaitFrame(45);

        // ダメージの算出
        SoundManager.PlaySe(Variables.SE.SeName.battle_unison_damage);
        int damage = OpeCharaList.GetAverageAtk(actionCharas);
        damage *= ( BCV.UNISON_DAMAGE_COEFFICIENT[unisonCount]);
        damage /= 100;
        damage *= cm.magnificationDamage;
        damage /= 100;
        damage *= (100 + unisonSupport); // ( 要調整 )
        damage /= 100;
        TargetChara.hp -= damage;
        if (TargetChara.hp < 0) TargetChara.hp = 0;
        
        // ダメージの演出
        DamageEffect.DrawDamage(damage);
        DamageEffect.DrawCombo(cm);
        yield return Utility._Wait.WaitFrame(45);

        DamageEffect.DestroyAllObject();


        yield return 0;
    }

    static private Vector3 id2Pos( int id )
    {
        if (id == 0) return new Vector3(0, 0, 0);
        int x = -1;
        if (id % 2 == 0) x = 1;
        x *= ( (id + 1) / 2);
        x *= 200;
        return new Vector3(x, 0, 0);
    }
}
