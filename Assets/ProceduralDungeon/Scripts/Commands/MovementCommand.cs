using UnityEngine;
using System.Collections;

public class MovementCommand : Command
{

	public MovementCommand (Transform actor, Vector2 dir) : base (actor, dir)
	{

	}

	public override void Perform ()
	{
		actor.Translate (dir.normalized);
	}

	public override void Undo ()
	{
		throw new System.NotImplementedException ();
	}
}
