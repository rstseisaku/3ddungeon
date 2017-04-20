﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/*
 * ================================================
 *  コンボ関係の処理を扱う
 * ================================================
 */
public class ComboManager
{
    // コンボ情報
    public int playerCombo; // プレイヤーのコンボ数
    public int enemyCombo; // 敵のコンボ数
    public int magnificationDamage; // ダメージの倍率
    public string comboString; // 表示用のテキスト

    public void Init()
    {
        playerCombo = 0;
        enemyCombo = 0;
    }
    // プレイヤーサイドのコンボ処理
    public void AddPlayerCombo()
    {
        playerCombo++;
        enemyCombo = 0;
        magnificationDamage = 90 + 10 * playerCombo;
        comboString = playerCombo + "COMBO!";
    }
    // 敵サイドのコンボ処理
    public void AddEnemyCombo()
    {
        playerCombo = 0;
        enemyCombo++;
        magnificationDamage = 90 + 10 * enemyCombo;
        comboString = enemyCombo + "COMBO!";
    }
}