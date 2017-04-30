using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * =============================================
 * やっていること
 * =============================================
 *  キャラクターリストの操作を行う
 *  
 *  [ Call, Get, Set, Count ]
 */
public class OpeCharaList : MonoBehaviour
{
    // 対象: CTB = 0 のキャラ全て( プレイヤー or 敵 )
    // 処理内容: Mag 値の合計を求める
    public static int GetSumMoveableMag(BaseCharacter[] cb)
    {
        int sum = 0;
        foreach (BaseCharacter chara in cb)
        {
            if (chara.ctbNum <= 0) sum += chara.cs.mag;
        }
        return sum;
    }

    // 対象: CTB = 0 のキャラ( プレイヤー or 敵 )
    // 処理内容: スタン処理を起動する
    public static void CallStun(BaseCharacter[] cb, int stunCtbNum)
    {
        foreach (BaseCharacter chara in cb)
        {
            if (chara.ctbNum <= 0) chara.StartStun(stunCtbNum);
        }
    }

    // 行動可能人数をカウント
    //  ┗ 第一引数: ユニゾンで待機しているキャラを無視
    public static Vector2 CountActionableCharacter(
         PlayerCharacter[] cd,
         EnemyCharacterData[] ecd)
    {
        // 行動可能人数を算出する
        int PlayerCount = 0;
        int EnemyCount = 0;
        for (int i = 0; i < cd.Length; i++)
        {
            if (cd[i].isWaitUnison) continue;
            if (cd[i].ctbNum <= 0) PlayerCount++;
        }
        for (int i = 0; i < ecd.Length; i++)
        {
            if (ecd[i].isWaitUnison) continue;
            if (ecd[i].ctbNum <= 0) EnemyCount++;
        }
        return new Vector2(PlayerCount, EnemyCount);
    } // --- CountActionCharacter()

    // 行動可能キャラクターのノックバック値の和を算出
    public static int GetSumKnockback(BaseCharacter[] cd)
    {
        int sumKnockback = 0;
        for (int i = 0; i < cd.Length; i++)
        {
            if (cd[i].ctbNum <= 0) sumKnockback += cd[i].cs.knockback;
        }
        return sumKnockback;
    } // --- GetSumKnockback()

    // 行動可能キャラクターの予測オブジェクトを表示
    public static void SetPredictActionableCharacter(BaseCharacter[] cd)
    {
        for (int i = 0; i < cd.Length; i++)
        {
            if (cd[i].ctbNum <= 0)
                cd[i].SetPredictFromWaitAction();
        }
    } // --- SetPredictActionableCharacter()
    public static void SetMagPredictActionableCharacter(BaseCharacter[] cd)
    {
        int aveMag = GetAverageMagWait(cd); 
        for (int i = 0; i < cd.Length; i++)
        {
            if (cd[i].ctbNum <= 0)
                cd[i].SetPredictFromCtbNum(aveMag);
        }
    } // ---  SetMagPredictActionableCharacter()

    // 行動可能キャラ( = CTB が0)で、詠唱しているキャラをカウント
    public static int CountActionableMagic(
         PlayerCharacter[] cd,
         EnemyCharacterData[] enemyCd)
    {
        // どちらサイドのキャラかを気にする必要はない
        // 【 CTB = 0 かつ 詠唱中】 の条件で
        //  ユニゾン待機の敵キャラは候補から外せる
        int count = 0;
        for (int i = 0; i < cd.Length; i++)
        {
            if (cd[i].ctbNum <= 0 && cd[i].isMagic &&
                !cd[i].isWaitUnison) count++;
        }
        for (int i = 0; i < enemyCd.Length; i++)
        {
            if (enemyCd[i].ctbNum <= 0 && enemyCd[i].isMagic &&
                !enemyCd[i].isWaitUnison) count++;
        }
        return count;
    } // --- CountActionableMagic()

    // ユニゾン待機中のフラグを折る処理
    public static void EndWaitUnison(bool isPlayer,
         PlayerCharacter[] cd,
         EnemyCharacterData[] enemyCd)
    {
        for (int i = 0; i < cd.Length && isPlayer; i++)
        {
            if (cd[i].ctbNum <= 0 && cd[i].isWaitUnison)
                cd[i].EndUnison();
        }
        for (int i = 0; i < enemyCd.Length && !isPlayer; i++)
        {
            if (enemyCd[i].ctbNum <= 0 && enemyCd[i].isWaitUnison)
                enemyCd[i].EndUnison();
        }
    }

    // 詠唱中のフラグを折る処理
    public static void EndMagic(bool isPlayer,
         PlayerCharacter[] cd,
         EnemyCharacterData[] enemyCd)
    {
        for (int i = 0; i < cd.Length && isPlayer; i++)
        {
            if (cd[i].ctbNum <= 0 && cd[i].isMagic)
                cd[i].EndMagic();
        }
        for (int i = 0; i < enemyCd.Length && !isPlayer; i++)
        {
            if (enemyCd[i].ctbNum <= 0 && enemyCd[i].isMagic)
                enemyCd[i].EndMagic();
        }
    }

    // 詠唱中キャラクターの待機値の平均を求める
    public static int GetAverageMagWait(BaseCharacter[] cb)
    {
        int aveMagWait = 0;
        int co = 0;
        for (int j = 0; j < cb.Length; j++)
        {
            if (cb[j].ctbNum <= 0 && !cb[j].isWaitUnison)
            {
                aveMagWait += cb[j].magWait;
                co++;
            }
        }
        if (co != 0) aveMagWait /= co;
        else aveMagWait = -1;
        return aveMagWait;
    }

    // HP の和を求める
    public static int GetSumHp(BaseCharacter[] cb)
    {
        int sum = 0;
        for (int j = 0; j < cb.Length; j++)
        {
            sum += cb[j].hp;
        }
        return sum;
    }

    // 戦闘不能でないものがいるかを調べる
    public static bool isAllKnockout(BaseCharacter[] cb)
    {
        bool f = true;
        for (int j = 0; j < cb.Length; j++)
        {
            if (!cb[j].isknockout) f = false;
        }
        return f;
    }

    // 戦闘不能になったキャラに、戦闘不能の演出を入れる
    public static void KnockoutEffect(BaseCharacter[] cb)
    {
        for( int i=0; i<cb.Length; i++)
            cb[i].KnockoutEffect();
    }
}
