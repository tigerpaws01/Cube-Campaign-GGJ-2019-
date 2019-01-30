using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour {
	public GameObject GetSoundEffect;
	private void OnTriggerEnter (Collider other) {
		if (!other.gameObject.CompareTag ("Player")) return;

		Player player = other.gameObject.GetComponent<Player> ();
		if (player.Power < 3) player.Power += 1;
		Debug.Log ($"{player.name}'s power is {player.Power}");
		// other.gameObject.transform.GetChild (3).gameObject.SetActive (true);
		Instantiate (GetSoundEffect);
		Destroy (this.gameObject);
	}
}