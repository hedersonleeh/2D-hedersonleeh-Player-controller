using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if(UNITY_EDITOR)
public class PlayerMonitor : MonoBehaviour {

	[SerializeField]Rigidbody2D playerRb;
	[SerializeField]Text txt;
	
	void Update () 
	{
		
		txt.text = string.Concat(Mathf.Abs(Mathf.Round(playerRb.velocity.x)),"m/s");	
	}
}
#endif
