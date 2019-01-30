using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour {
    public float time;
    public float randTime;
    public int count = 1;
    public float offsetX;
    //決定從POOL 生成還是Instantiate
    public bool enablePool = true;

    private float coolDown = 0;
    public GameObject prefab;
    private List<GameObject> items = new List<GameObject> ();
    private List<GameObject> recycle = new List<GameObject> ();

    private void Start () {
        ResetCoolDown ();
    }

    private void ResetCoolDown () {
        coolDown = time + (randTime * Random.value);
    }

    private void Update () {
        coolDown -= Time.deltaTime;

        if (coolDown > 0) return;

        var position = transform.position;
        var ox = offsetX;

        for (int i = 0; i < count; i++) {
            position.x += Random.Range (-ox, ox);
            position.z += Random.Range (-ox, ox);

            if (enablePool && GameManager.Instance.hasStarted)
                Spawn (position);
            else
                Instantiate (prefab, position, Quaternion.identity);
        }

        ResetCoolDown ();
    }

    public GameObject Spawn (Vector3 position) {
        GameObject item = null;
        bool isRecycle = false;

        if (recycle.Count > 0) {
            item = recycle[0];
            recycle.RemoveAt (0);

            item.SetActive (true);
            isRecycle = true;
        } else {
            item = Instantiate (prefab);
        }

        item.transform.SetParent (transform, false);
        item.transform.position = position;

        //連同子物件也會被呼叫
        item.BroadcastMessage ("OnSpawn", this, SendMessageOptions.DontRequireReceiver);

        if (isRecycle)
            item.BroadcastMessage ("Start", SendMessageOptions.DontRequireReceiver);

        items.Add (item);

        return item;
    }

    public bool DeSpawn (GameObject item) {
        if (!items.Remove (item)) {
            Destroy (item);
            return false;
        }
        item.BroadcastMessage ("OnDeSpawn", this, SendMessageOptions.DontRequireReceiver);
        item.BroadcastMessage ("OnDestroy", SendMessageOptions.DontRequireReceiver);

        item.SetActive (false);
        recycle.Add (item);
        return true;
    }
}