using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DiedEffect : MonoBehaviour {
    public GameObject spreadPrefab;
    // Start is called before the first frame update
    void Start () {

    }

    void OnTriggerEnter (Collider other) {
        var targetLocation = other.gameObject.transform.localPosition;
        GameObject spread = Instantiate (spreadPrefab);
        spread.transform.localPosition = targetLocation;
        other.GetComponent<Player> ().Power = 0;
        other.transform.position = new Vector3 (1, 7, 0.36f);
        if (other.GetComponent<Player> ().WhoBumped != null) {
            other.GetComponent<Player> ().WhoBumped.GainScore (10);
        }
        GameManager.Instance.Camera.transform.DOShakePosition (0.5f, new Vector3 (.3f, .3f, .3f));
    }
    // Update is called once per frame
    void Update () {

    }
}