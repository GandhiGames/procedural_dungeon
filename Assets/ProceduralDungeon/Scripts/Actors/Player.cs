using UnityEngine;
using System.Collections;

namespace ProceduralDungeon
{
	[RequireComponent (typeof(InputHandler))]
	public class Player : Actor
	{
		private InputHandler input;

		void Start ()
		{
			input = GetComponent<InputHandler> ();
		}

		public override Command GetCommand ()
		{
			return input.GetCommand ();
		}
	}
}
