using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {


    Firebase fb = new Firebase();
	// Use this for initialization
	void Start () {
		
	}


    System.Action<string> test1 = (text) =>
    {
        Debug.Log(text);
    };

    System.Action<bool> test2 = (text) =>
    {
        Debug.Log(text);
    };



	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(fb.CloseBox("locker01", "password01", test1));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(fb.OpenBox("locker01", "box01", test2));
        }

    }
}
