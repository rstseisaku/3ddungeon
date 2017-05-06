using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utility;

public class StartMenu : MonoBehaviour {

	public void mStart()
    {

        StartCoroutine(_Scene.MoveScene("Base"));
    }

    public void mLoad()
    {
        StartCoroutine(_Scene.MoveScene("Base"));

    }
}
