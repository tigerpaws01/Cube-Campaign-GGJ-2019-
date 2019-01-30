using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
// Singleton Class GameManager
public class GameManager {
    // Singleton Instance
    private static GameManager _instance = null;
    public static GameManager Instance {
        get {
            if (_instance == null) _instance = new GameManager ();
            return _instance;
        }
    }

    static public void Reset () {
        _instance = new GameManager ();
    }

    // Game Logic Reference
    private GameLogic gameLogic = null;

    // Menu Object Reference
    public GameObject Menu = null;

    public GameObject Camera = null;

    // Start Status
    public bool hasStarted = false;

    // Make constructor inaccessible
    private GameManager () {
        // Get access to the outside world
        gameLogic = GameObject.FindObjectOfType<GameLogic> ();
        if (gameLogic == null) throw new System.Exception ("No GameLogic Object Found.");

        // Connect the first 2 keyboard players.
        gameLogic.Players[0].Connected = true;
        gameLogic.Players[1].Connected = true;

        // Airconsole Events
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    // When a AirConsole Device connects
    void OnConnect (int device_id) {
        int active_player_num = AirConsole.instance.ConvertDeviceIdToPlayerNumber (device_id);
        Debug.Log ("Player_num = " + active_player_num); // -1, 1

        // Distinguish between players
        if (active_player_num == -1) {
            gameLogic.Players[2].Connected = true;
            Debug.Log ("Player 3 Connected.");
        } else {
            gameLogic.Players[3].Connected = true;
            Debug.Log ("Player 4 Connected.");
        }

        if (AirConsole.instance.GetActivePlayerDeviceIds.Count == 0) {
            if (AirConsole.instance.GetControllerDeviceIds ().Count >= 2) {
                AirConsole.instance.SetActivePlayers (2);
            } else {
                Debug.Log ("GIMME MORE PLAYERS");
            }
        }
    }

    // Disconnection Event
    void OnDisconnect (int device_id) {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber (device_id);
        if (active_player != -1) {
            if (AirConsole.instance.GetControllerDeviceIds ().Count < 2) {
                AirConsole.instance.SetActivePlayers (0);
                Debug.Log ("NEED MORE PLAYERS");
            } else {
                AirConsole.instance.SetActivePlayers (2);
            }
        }
    }

    // Message Event: When AirConsole sends a message
    private void OnMessage (int deviceID, JToken data) {
        if (data is null) Debug.LogError ("Data is Null");
        Debug.Log ("DeviceID = " + deviceID + ", data" + data);

        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber (deviceID);
        Debug.Log ("Player_num_OnMessage = " + active_player);

        int cmd = data["move"] != null ? (int) data["move"] : -100;
        int skill = data["skill"] != null ? (int) data["skill"] : -100;

        // If a player sends a skill while the game is not yet started,
        // It means he/she gets ready
        if (skill == 1 && !hasStarted) {
            // Player Get READY
            GetReady (active_player + 2);

        }

        if (skill == 1 && hasStarted) {
            gameLogic.Players[active_player + 2].DashSkill ();
        }
        if (!hasStarted || (active_player != 0 && active_player != 1)) return;

        // Turn Left
        if (cmd == Values.CMD.TurnL) {
            Debug.Log ("TurnL  ");
            gameLogic.Players[active_player + 2].Dir = Player.Direction.LEFT;
        }
        // Turn Right
        if (cmd == Values.CMD.TurnR) {
            Debug.Log ("TurnR = ");
            gameLogic.Players[active_player + 2].Dir = Player.Direction.RIGHT;
        }
        if (cmd == 0) {
            gameLogic.Players[active_player + 2].Dir = Player.Direction.FORWARD;
        }
    }

    public void GetReady (int player) {
        var menu = Menu.GetComponent<MenuWaitConnection> ();
        menu.PlayerTypes[player].transform.GetChild (0).GetComponent<Animator> ().SetBool ("ready", true);
        // display ready status
        menu.PlayerTypes[player].transform.GetChild (4).gameObject.SetActive (true);
        menu.Status[player] = true;
        menu.JoinSoundEffect.Play ();
    }

    // Start Game Flow
    public void Start () {
        hasStarted = true;
        AirConsole.instance.SetActivePlayers (2);
    }

    public void Stop () {
        hasStarted = false;
    }

}