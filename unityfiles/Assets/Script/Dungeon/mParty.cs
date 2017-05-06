using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Variables;
using Utility;

public class mParty : MonoBehaviour {

    Party party;
    CharacterStatus characterstatus;
    GameObject[] icon;

	// Use this for initialization
	void Start () {

        if (GameObject.Find(Variables.Party.SingletonObjectName) != null)
        {
            party = GameObject.Find(Variables.Party.SingletonObjectName).GetComponent<Party>();
            icon = new GameObject[party.partyCharacter.Length];

            for (int i = 0; i < party.partyCharacter.Length; i++)
            {

                characterstatus = party.GetPartyCharacterBattleStatus(i);

                GameObject ch = _Object.MyInstantiate("Prefabs/Map/CharacterIcon", this.gameObject, characterstatus.faceGraphicPath);
                ch.name = "ch" + (i+1).ToString();
                ch.transform.localPosition = new Vector3(-560 + 220*i, 0, 0);
                icon[i] = ch;
            }
        }
        else
        {
            Debug.Log("パーティーが居ない！");
        }
		
	}
	
	// Update is called once per frame
	void Update () {
        
        for (int i = 0; i < party.partyCharacter.Length; i++)
        {
            icon[i].transform.FindChild("HP").GetComponent<mHPcolor>().remainingHP = party.partyCharacter[i].hp;
            icon[i].transform.FindChild("HP").GetComponent<mHPcolor>().maxHP = party.partyCharacter[i].bcs.maxHp;
        }


    }
}
