using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour {
    public float depth, height;
    public Transform _camera;
    public Transform[] Players;

    private void Update () {
        Vector3 average = Vector3.zero;
        foreach (var t in Players) average += t.position;
        average /= Players.Length;
        average.z = 0;

        _camera.position = Vector3.Slerp (_camera.position, average + new Vector3 (0, 0, _camera.localPosition.z), 0.2f);
    }
}