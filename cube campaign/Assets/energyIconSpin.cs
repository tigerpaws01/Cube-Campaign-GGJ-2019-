using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class energyIconSpin : MonoBehaviour {
    public GameObject Camera;
    public GameObject player;
    public GameObject[] Energy;
    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        this.transform.LookAt (this.Camera.transform);
        Energy[0].SetActive (
            this.player.GetComponent<Player> ().Power >= 1
        );
        Energy[1].SetActive (
            this.player.GetComponent<Player> ().Power >= 2
        );
        Energy[2].SetActive (
            this.player.GetComponent<Player> ().Power >= 3
        );

    }
}