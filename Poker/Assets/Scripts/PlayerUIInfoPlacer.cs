using UnityEngine;
using System.Collections;

public class PlayerUIInfoPlacer : MonoBehaviour
{
	public GameObject playerUIInfo;
	public PlayerUIInfo[] playerUIInfos { get; set; }
	//public float spaceBetweenUI { get; set; }
	//public float infoWidth{ get; set; }
	//public float infoHeight { get; set; }

	public void Start()
	{
		//Vector3[] playerUIPlaces = new Vector3[1];
		playerUIInfos = new PlayerUIInfo[5];//playerUIPlaces.Length];
		/*for (int i = 0;i < playerUIPlaces.Length; i ++)
		{
			playerUIPlaces[i] = new Vector3(Screen.width / (playerUIPlaces.Length - i) + spaceBetweenUI * i, Screen.height);
		}*/
		for (int i = 0; i < playerUIInfos.Length; i ++)
		{
			playerUIInfos[i] = Instantiate (playerUIInfo).GetComponent<PlayerUIInfo>();
		}
		ChangeUIInfoSize ();
	}

	public void ChangeUIInfoSize()
	{
		float spaceBetweenUI = Screen.width / 18;
		float infoWidth = (Screen.width - ((playerUIInfos.Length + 2) * spaceBetweenUI)) / playerUIInfos.Length;//Screen.width / 6;
		float infoHeight = Screen.height / 6;
		for (int i = 0; i < playerUIInfos.Length; i ++)
		{
			playerUIInfos[i].transform.SetParent(FindObjectOfType<Canvas>().transform,false);
			RectTransform playerRect = playerUIInfos[i].gameObject.GetComponent<RectTransform>();
			playerRect.position = new Vector3(spaceBetweenUI * (i + 1) + infoWidth * i,Screen.height);//playerUIPlaces[i];
			playerRect.sizeDelta = new Vector2(infoWidth,infoHeight);
		}
	}
}
