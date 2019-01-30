using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour {
    public float RotationSpeed = 120f;
    public GameObject _as;

    private void Update () {
        transform.Rotate (new Vector3 (0, RotationSpeed * Time.deltaTime, 0));
    }

    private void OnTriggerEnter (Collider col) {
        if (!col.gameObject.CompareTag ("Player")) return;
        Player player = col.gameObject.GetComponent<Player> ();
        float DesRotationSpeed = RotationSpeed * 3;
        TweenParams tParms = new TweenParams ().SetEase (Ease.OutCubic);
        //transform.DOMove(Vector3.up * 24, 1.5f).SetAs(tParms);
        col.GetComponent<Player> ().GainScore (1);
        Instantiate (_as);
        Destroy (this.gameObject);

    }
}