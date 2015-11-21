using UnityEngine;
using System.Collections;

namespace ProceduralDungeon
{
	public class InputHandler : MonoBehaviour
	{

		public Command GetCommand ()
		{
			Vector2 move = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			
			
			if (move.magnitude > 0.1f) {
				return new MovementCommand (transform, move.normalized * Time.deltaTime);
			} 
			
			
			
			return null;
		}
	}
}
