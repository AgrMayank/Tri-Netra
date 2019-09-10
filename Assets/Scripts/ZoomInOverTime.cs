using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOverTime : MonoBehaviour {

	private IEnumerator applyPowerUp(string type, GameObject player)
	{
		GameObject originPlayer = new GameObject();
		originPlayer = player;

		if (type == "Bigger")
		{
			Debug.Log("make it big");
			player.transform.localScale = new Vector3(1.25f, 2.9f, 0);
			player.transform.position = new Vector2(player.transform.position.x + 50f, player.transform.position.y + 50f);
			yield return new WaitForSeconds(3);
			player = originPlayer;
		}
	}
}
