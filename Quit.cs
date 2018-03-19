using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour {

	
	void Start () {


        if (Input.GetKey("escape"))
            Application.Quit();
    }

    public void QuitClick()
    {
        Application.Quit();
    }

}
