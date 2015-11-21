using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProceduralDungeon
{
	public class ActorController : MonoBehaviour
	{

		private static readonly string SCRIPT_NAME = typeof(ActorController).Name;
		private List<GameObject> actors = new List<GameObject> ();
		private int currentActor = 0;

		// Use this for initialization
		private void Init ()
		{
			actors.AddRange (GameObject.FindGameObjectsWithTag ("Actor"));

			if (actors.Count == 0) {
				Debug.LogError (SCRIPT_NAME + ": no objects with actor tag found");
			}	
		}
	
		// Update is called once per frame
		void Update ()
		{
			if (actors.Count == 0) {
				Init ();
			} else {

				var command = actors [currentActor].GetComponent<Actor> ().GetCommand ();

				if (command != null) {
					command.Perform ();
					currentActor = (currentActor + 1) % actors.Count;
				}
			}
	
		}
	}
}
