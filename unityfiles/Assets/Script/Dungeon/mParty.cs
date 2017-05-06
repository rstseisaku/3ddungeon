using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Variables;
using Utility;

public class mParty : MonoBehaviour {

    PartyCharacter[] partycharacter;

	// Use this for initialization
	void Start () {

        if (GameObject.Find(Variables.Party.SingletonObjectName) != null)
        {
            partycharacter = GameObject.Find(Variables.Party.SingletonObjectName).GetComponent<Party>().partyCharacter;

            for (int i = 0; i < partycharacter.Length; i++)
            {
                GameObject ch = _Object.MyInstantiate("Prefabs/Map/CharacterIcon", this.gameObject);
                ch.name = "ch" + (i+1).ToString();
                Image image = ch.AddComponent<Image>();
                //image.sprite = partycharacter[i].
            }
        }
        else
        {
            Debug.Log("パーティーが居ない！");
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
