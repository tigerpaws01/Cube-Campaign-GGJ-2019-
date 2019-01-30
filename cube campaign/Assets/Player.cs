using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public Player WhoBumped = null;
    public enum Direction { LEFT, RIGHT, FORWARD }

    // Whether this player is connected
    [HideInInspector]
    public bool Connected = false;

    public float CollisionRadius = 1.2f;

    public ParticleSystemControl DashPS;

    public AudioSource[] ponSoundEffect;
    public AudioSource explosionSoundEffect;
    public GameObject Blast;

    public bool UsingKeyboard = false;
    public int Dash, Left, Right;
    public bool CanDash = true;
    public float DashDuration = 1.5f, DashDecay = 0.5f;

    // Player Current Direction
    public Direction Dir = Direction.FORWARD;
    public int energy = 0;
    // Player VelocityImpactManager
    VelocityManager velManager = new VelocityManager ();

    public Text DmgText;
    public float Speed = 1.0f;
    public float RotationSpeed = 600f;

    public int Power = 0;

    //public enum PlayerState { Normal, Super, Hurting, Die }

    public void TurnL () {
        transform.Rotate (new Vector3 (0, -RotationSpeed * Time.deltaTime, 0));
    }

    public void TurnR () {
        transform.Rotate (new Vector3 (0, RotationSpeed * Time.deltaTime, 0));
    }

    //TODO
    public void DashSkill () {
        if (!CanDash) return;

        DashPS.CompletelyStop ();
        DashPS.Play ();
        CanDash = false;
        velManager.AddImpact (
            new VelocityImpact (Vector3.forward * Speed * 2 * (Power + 1), true, false, true, DashDuration, DashDecay, ResetDash)
        );
        Power = 0;
    }

    public void ResetDash () {
        CanDash = true;
    }

    private void Start () {
        // Add a constant forward vel
        velManager.AddImpact (
            new VelocityImpact (Vector3.forward * Speed, true, false, false)
        );
    }

    private void FixedUpdate () {
        if (GameManager.Instance.hasStarted) {

            // MOVE UR ASS!!
            var playerVel = velManager.RefreshImpacts (transform.rotation * Vector3.forward);
            transform.Translate (
                playerVel * Time.deltaTime, Space.World
            );

            // Rotate UR [CENSORED]!!
            if (UsingKeyboard) {
                bool l = Input.GetKey ((KeyCode) Left),
                    r = Input.GetKey ((KeyCode) Right);

                if (l ^ r) Dir = l ? Direction.LEFT : Direction.RIGHT;
                else Dir = Direction.FORWARD;

                if (Input.GetKeyDown ((KeyCode) Dash)) {
                    DashSkill ();
                }
            }
            if (Dir == Direction.LEFT) TurnL ();
            if (Dir == Direction.RIGHT) TurnR ();

            // Do I collide with others?
            PlayerCollide (playerVel);
        }

    }

    private void PlayerCollide (Vector3 vel) {
        // If a player is near the immediate position relative to my velocity
        // Then we consider it collided, and add a vel impact to the two
        // TODO: Let the maxDistance differ with regards to angle? Adopt projection when bumped?

        /*RaycastHit hit;
        if (Physics.Raycast (transform.position, vel, out hit, CollisionRadius) && hit.rigidbody.gameObject.CompareTag ("Player")) {
            // Let the Hit one have a burst vel impact along my vel
            hit.rigidbody.gameObject.GetComponent<Player> ().velManager.AddImpact (
                new VelocityImpact (vel * 2, false, true, true, 1.2f, 0.2f)
            );
            // Let me have a burst impact against my vel
            velManager.AddImpact (
                new VelocityImpact (vel * -1.5f, false, true, true, 0.8f, 0.05f)
            );
        }*/

        // BoxCast
        BoxCollider coll = GetComponent<BoxCollider> ();
        RaycastHit hit;
        // Debug.Log ("" + hit.rigidbody.gameObject.CompareTag ("Player"));
        if (Physics.BoxCast (coll.bounds.center, transform.localScale / 2, vel, out hit, transform.rotation, 0.15f)) {
            if (hit.rigidbody == null || !hit.rigidbody.gameObject.CompareTag ("Player")) return;

            Vector3 dir = hit.point - transform.position;
            dir.Normalize ();

            // deal damage
            var target = hit.rigidbody.gameObject.GetComponent<Player> ();
            // this.transform.GetChild (3).gameObject.SetActive (false);
            Power = 0;
            //target.DmgText.text = $"{target.Dmg}%";

            float angle = Vector3.Angle (dir, vel);

            hit.rigidbody.gameObject.GetComponent<Player> ().WhoBumped = this;

            hit.rigidbody.gameObject.GetComponent<Player> ().velManager.AddImpact (
                new VelocityImpact (dir * vel.magnitude * Mathf.Cos (angle * Mathf.Deg2Rad) * 2, false, true, true, 1.2f, 0.2f)
            );

            velManager.AddImpact (
                new VelocityImpact (dir * vel.magnitude * Mathf.Cos (angle * Mathf.Deg2Rad) * -1.5f, false, true, true, 0.8f, 0.05f)
            );

            if (vel.magnitude > 1) Instantiate (Blast, hit.point, Quaternion.identity);

            // play pon sound effect
            this.ponSoundEffect[Random.Range (0, this.ponSoundEffect.Length)].Play ();
        }
    }

    private void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag != "Player") return;

        /*Vector3 Colli_dir = transform.position - collision.transform.position;
        isHitting = true;
        HitTmr = 0;
        SumSpeed += collision.relativeVelocity;
        HitSpd = SumSpeed.normalized * 18;*/

    }

    public void GainScore (int val) {
        DmgText.text = (int.Parse (DmgText.text) + val).ToString ();
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position, CollisionRadius);
    }
}

// Describes the impacts on velocity
public class VelocityImpact {

    public Vector3 Direction;
    public bool ForcedToFaceDir, Decay, TimeOut;
    private float DecayRate, Timer;

    public delegate void Termination ();
    public event Termination TerminationEvent;

    public VelocityImpact (Vector3 direction, bool forcedToFaceDir, bool decay, bool timeOut, float timer = 0, float decayRate = 0, Termination ter = null) {
        Direction = direction;
        ForcedToFaceDir = forcedToFaceDir;
        Decay = decay;
        DecayRate = decayRate;
        TimeOut = timeOut;
        Timer = timer;
        if (ter != null) TerminationEvent += ter;
    }

    public void Update () {
        // Make Decay
        if (Decay) Direction *= Mathf.Pow (DecayRate, Time.deltaTime);
        // Make Outdated
        if (Timer >= 0) Timer -= Time.deltaTime;
    }

    public bool IsDead () {
        return TimeOut && Timer <= 0;
    }

    public void Terminate () {
        if (TerminationEvent != null) TerminationEvent ();
    }
}

// Manages a list of VelocityImpacts
public class VelocityManager {
    private List<VelocityImpact> impacts = new List<VelocityImpact> { };

    public void AddImpact (VelocityImpact impact) {
        impacts.Add (impact);
    }

    public Vector3 RefreshImpacts (Vector3 FaceDir) {
        Vector3 result = Vector3.zero;
        // Sum up all the impacts!
        foreach (var i in impacts) {
            if (!i.ForcedToFaceDir) result += i.Direction;
            else result += i.Direction.magnitude * FaceDir / FaceDir.magnitude;
        }

        // Update And Kill All impacts
        for (int idx = impacts.Count - 1; idx >= 0; idx--) {
            if (impacts[idx].IsDead ()) {
                impacts[idx].Terminate ();
                impacts.RemoveAt (idx);
            } else impacts[idx].Update ();
        }
        return result;
    }
}