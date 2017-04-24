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
 * 
 *  [ Call, Get, Set, Count ]
 */
public class CtbManager : MonoBehaviour
{
    // 対象: CTB = 0 のキャラ( プレイヤー and 敵 )
    // 処理内容: CTB 値をセット，Wait 値の再設定
    public static void SetCtbNum(PlayerCharacter[] cd, EnemyCharacterData[] ecd)
    {
        SetCtbNum(cd);
        SetCtbNum(ecd);
    }

    // 対象: CTB = 0 のキャラ全て( プレイヤー or 敵 )
    // 処理内容: CTB 値をセット，Wait 値の再設定
    public static void SetCtbNum( BaseCharacter[]cb )
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
    public static int SubCtbNum(BaseCharacter[] cb)
    {
        int count = 0;
        for (int i = 0; i < cb.Length; i++)
        {
            // ctbゲージを進める
            if (cb[i].stunCount == 0) cb[i].ctbNum--;
            if (cb[i].ctbNum < 0) cb[i].ctbNum = 0;
            // アクション可能キャラがいたら、フラグを立てる 
            if (cb[i].ctbNum <= 0 && !cb[i].isWaitUnison)
            {
                count++;
            }
        }
        return count;
    }

    // 対象: スタン中の全てのキャラクター
    // 処理内容: スタン値を1減らす
    public static void SubStun(BaseCharacter[] cb, BaseCharacter[] ecb)
    {
        SubStun(cb);
        SubStun(ecb);
    }
    // 対象: スタン中の全てのキャラクター
    // 処理内容: スタン値を1減らす
    public static void SubStun(BaseCharacter[]cb)
    {
        for (int i = 0; i < cb.Length; i++)
        {
            // スタン値を1減らす
            if (cb[i].stunCount > 0)
            {
                cb[i].stunCount--;
                if (cb[i].stunCount == 0)
                {
                    // スタン表示終了
                    cb[i].EndStun();
                }
            }
        }
    }
}
