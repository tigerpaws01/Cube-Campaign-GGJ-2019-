using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemControl : MonoBehaviour {

	public ParticleSystem[] ps;

	public void CompletelyStop()
	{
		foreach(var _ps in ps) _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void Play()
	{
		foreach (var _ps in ps) _ps.Play();
	}
}
