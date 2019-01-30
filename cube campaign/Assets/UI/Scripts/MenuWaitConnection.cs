using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MenuWaitConnection : MonoBehaviour {
    public GameObject BattleArea;
    public GameObject PlayerResetArea;
    public GameObject BattleMenu;
    public AudioSource JoinSoundEffect;
    public List<GameObject> PlayerTypes;
    public List<bool> Status { get; private set; }
    // Start is called before the first frame update
    void Start () {
        GameManager.Instance.Menu = this.gameObject;
        this.PlayerTypes = new List<GameObject> ();
        for (int i = 2; i < 6; ++i) {
            this.PlayerTypes.Add (this.transform.GetChild (i).gameObject);
        }
        this.Status = new List<bool> ();
        for (int i = 0; i < 4; ++i) {
            this.Status.Add (false);
        }
        // TODO: Intentionally Set AirConsole Players connected
        this.Status[2] = this.Status[3] = true;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown (KeyCode.W)) GameManager.Instance.GetReady (0);
        if (Input.GetKeyDown (KeyCode.UpArrow)) GameManager.Instance.GetReady (1);

        if (!Status.Contains (false)) {
            this.BattleArea.SetActive (true);
            this.PlayerResetArea.SetActive (true);
            this.gameObject.SetActive (false);
            this.BattleMenu.SetActive (true);
            // start game
            GameManager.Instance.Start ();
            GameManager.Instance.Menu = this.BattleMenu;

        }
    }
}