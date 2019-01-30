using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timeline : MonoBehaviour {

    public GameObject gameLogic;

    public Text[] Score;
    public Text Timer_String;
    public GameObject Timer_Image;

    public int total_seconds;
    public bool IsTiming;
    public AudioSource clockTick;
    public GameObject ResultMenu;

    public AudioSource BattleBackgroundMusic;
    public AudioSource BattleFinalBackgroundMusic;
    private int seconds;
    private float tmr_register;
    private bool _finalStage;
    // Start is called before the first frame update
    void Start () {
        this.tmr_register = 0;
        this.IsTiming = true;
        this._finalStage = false;
        this.BattleBackgroundMusic.Play ();
    }

    void Update () {
        if (this.IsTiming) {
            StartCoroutine (this.clock ());
            this.IsTiming = false;
        }

    }

    IEnumerator clock () {
        yield return new WaitForSeconds (1);
        this.IsTiming = true;
        this.tmr_register++;
        this.seconds = this.total_seconds - (int) this.tmr_register;
        if (this.seconds >= 0) {
            // update timer string and image width
            string time = (this.seconds / 60 > 9 ? "" : "0") +
                $"{this.seconds / 60} : " + (this.seconds % 60 > 9 ? "" : "0") + $"{this.seconds % 60}";
            this.Timer_String.text = time;
            var transform = this.Timer_Image.transform as RectTransform;
            transform.sizeDelta = new Vector2 (2000 * this.seconds / this.total_seconds, transform.sizeDelta.y);
            if (this.seconds < 11) {
                Debug.Log ("Play clock tick sound effect");
                this.clockTick.Play ();
            }
            if (this.seconds < 31 && !this._finalStage) {
                StartCoroutine (this.audioFadeOut (this.BattleBackgroundMusic, 1.0f));
                StartCoroutine (this.audioFadeIn (this.BattleFinalBackgroundMusic, 1.0f));
                this._finalStage = true;
            }

        } else {
            // Times up
            int idx, highest = -1, highestIdx = 0;
            for (idx = 0; idx < 4; idx++) {
                if (int.Parse (Score[idx].text) > highest) {
                    highest = int.Parse (Score[idx].text);
                    highestIdx = idx;
                }
            }

            ResultMenu.transform.GetChild (0).GetComponent<Text> ().text = $"P{highestIdx + 1} Wins!!";
            this.IsTiming = false;
            this.ResultMenu.SetActive (true);
            this.BattleFinalBackgroundMusic.Stop ();
            GameManager.Instance.Stop ();
            GameManager.Instance.Menu = this.ResultMenu;
            // GameManager.Instance.;
        }
    }

    IEnumerator audioFadeOut (AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop ();
        audioSource.volume = startVolume;
    }
    IEnumerator audioFadeIn (AudioSource audioSource, float FadeTime) {
        audioSource.volume = 0;
        audioSource.Play ();
        while (audioSource.volume < 100) {
            audioSource.volume += 100 * Time.deltaTime / FadeTime;

            yield return null;
        }
        audioSource.volume = 100;
    }
}