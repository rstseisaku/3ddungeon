using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * =============================================
 * やっていること
 * =============================================
 * CTB 操作処理
 * キャラクター全体の情報取得・情報操作
 * キャラクター全体の処理の実行
 */
public class CtbManager : MonoBehaviour
{
    // 対象: CTB = 0 のキャラ( プレイヤー and 敵 )
    // 処理内容: CTB 値をセット，Wait 値の再設定
    public static void SetCtbNum( CharacterData[] cd, EnemyCharacterData[] ecd)
    {
        SetCtbNum(cd);
        SetCtbNum(ecd);
    }

    // 対象: CTB = 0 のキャラ( プレイヤー or 敵 )
    // 処理内容: CTB 値をセット，Wait 値の再設定
    public static void SetCtbNum( CharacterBase[] cb )
    {
        for (int i = 0; i <cb.Length; i++)
        {
            if (cb[i].ctbNum <= 0)
            {
                cb[i].ctbNum = cb[i].waitAction;
                cb[i].SetWaitTime();
            }
        }
    }

    // 対象: CTB != 0 のキャラ( プレイヤー or 敵 )
    //       ユニゾン待機キャラは無視
    // 処理内容: CTB値を -1 する
    // 戻り値: 行動可能な人数
    public static int SubCtbNum(CharacterBase[] cb)
    {
        int count = 0;
        for (int i = 0; i < cb.Length; i++)
        {
            // ctbゲージを進める
            cb[i].ctbNum--;
            if ( cb[i].ctbNum < 0) cb[i].ctbNum = 0;
            // アクション可能キャラがいたら、フラグを立てる 
            if (cb[i].ctbNum <= 0 && !cb[i].isWaitUnison)
            {
                Debug.Log(i + "i, " + cb[i].ctbNum);
                count++;
            }
        }
        return count;
    }


    // 行動可能人数をカウント
    //  ┗ 第一引数: ユニゾンで待機しているキャラを無視
    public static Vector2 CountActionableCharacter(
        bool isIgnoreWaitUnison,
         CharacterData[] cd,
         EnemyCharacterData[] ecd)
    {
        // 行動可能人数を算出する
        int PlayerCount = 0;
        int EnemyCount = 0;
        for (int i = 0; i < cd.Length; i++)
        {
            if ( isIgnoreWaitUnison && cd[i].isWaitUnison) continue;
            if ( cd[i].ctbNum <= 0) PlayerCount++;
        }
        for (int i = 0; i < ecd.Length; i++)
        {
            if ( isIgnoreWaitUnison && ecd[i].isWaitUnison) continue;
            if ( ecd[i].ctbNum <= 0) EnemyCount++;
        }
        return new Vector2(PlayerCount, EnemyCount);
    } // --- CountActionCharacter()

    // 行動可能キャラクターのノックバック値の和を算出
    public static int GetSumKnockback( CharacterBase[] cd )
    {
        int sumKnockback = 0;
        for (int i = 0; i < cd.Length; i++)
        {
            if (cd[i].ctbNum <= 0) sumKnockback += cd[i].knockback;
        }
        return sumKnockback;
    } // --- GetSumKnockback()


    // 行動可能キャラ( = CTB が0)で、詠唱しているキャラをカウント
    public static int CountActionableMagic(
         CharacterData[] cd,
         EnemyCharacterData[] enemyCd)
    {
        // どちらサイドのキャラかを気にする必要はない
        // 【 CTB = 0 かつ 詠唱中】 の条件で
        //  攻撃対象キャラは候補から外せる
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
         CharacterData[] cd,
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
         CharacterData[] cd,
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
    public static int GetAverageMagWait( CharacterBase[] cb)
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
}
