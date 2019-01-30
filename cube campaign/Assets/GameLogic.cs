using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {

    public Player[] Players;
    //======================

    public GameObject Item;
    public float PutItemDelay;
    public GameObject BattleArea;

    private float _lastPutItemTime;
    private bool _generating;

    void Start () {
        this._generating = true;
        GameManager.Instance.Camera = GameObject.FindGameObjectWithTag ("MainCamera");
    }
    void Update () {
        /*if (this._generating) {
            StartCoroutine (this.generateItem ());
            this._generating = false;
        }*/
        // TODO: Put Item, use StartCoroutine or Timer?
        /*
        if ((Time.time - this._lastPutItemTime) < this.PutItemDelay) return;
        var itemCount = GameObject.FindGameObjectsWithTag ("Item").Length;
        Debug.Log ($"item count: {itemCount}");
        if (itemCount < 10) {
			var areaPosition = this.BattleArea.transform.position;
			var areaRadius = this.BattleArea.transform.localScale.x * 2F;
			this._lastPutItemTime = Time.time;
            var item = Instantiate (this.Item);
            var positionR = Random.Range (0F, areaTransform.localScale.x / 2F);
            var positionA = Random.Range (0F, Mathf.PI * 2F);
            item.transform.localScale = new Vector3 (0.2F, 0.2F, 0.2F);
            item.transform.position = new Vector3 (
                areaTransform.position.x + positionR * Mathf.Sin (positionA),
                1.5F,
                areaTransform.position.z + positionR * Mathf.Cos (positionA));
        }*/
    }

    IEnumerator generateItem () {
        yield return new WaitForSeconds (this.PutItemDelay);
        Debug.Log ("generating");
        var areaTransform = this.BattleArea.transform;
        var itemCount = GameObject.FindGameObjectsWithTag ("Item").Length;
        if (itemCount < 10) {
            this._lastPutItemTime = Time.time;
            var item = Instantiate (this.Item);
            var positionR = Random.Range (0F, areaTransform.localScale.x / 2F);
            var positionA = Random.Range (0F, Mathf.PI * 2F);
            item.transform.localScale = new Vector3 (0.2F, 0.2F, 0.2F);
            item.transform.position = new Vector3 (
                areaTransform.position.x + positionR * Mathf.Sin (positionA),
                1.5F,
                areaTransform.position.z + positionR * Mathf.Cos (positionA));
        }
    }

}