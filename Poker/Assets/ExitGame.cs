using UnityEngine;
using System.Collections;

public class ExitGame : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (Input.anyKey)
			Debug.Log (Input.inputString);

		if (Input.GetKey("q"))
			Application.Quit();
	}
}
