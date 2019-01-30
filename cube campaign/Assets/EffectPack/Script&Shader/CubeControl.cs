using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : MonoBehaviour {

	public GameObject[] Cubes;
	private int index = 0;

	private void Start()
	{
		foreach (var c in Cubes) c.SetActive(c == Cubes[index]);
	}

	void Update () {
		int legacyIdx = index;

		if (Input.GetKeyDown(KeyCode.Z)) index = (index + Cubes.Length - 1) % Cubes.Length;
		if (Input.GetKeyDown(KeyCode.X)) index = (index + 1) % Cubes.Length;

		if (legacyIdx != index)
			foreach (var c in Cubes) c.SetActive(c == Cubes[index]);
	}
}
