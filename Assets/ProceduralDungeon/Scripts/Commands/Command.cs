using UnityEngine;
using System.Collections;

public abstract class Command
{

	protected Transform actor;
	protected Vector2 dir;

	public Command (Transform actor, Vector2 dir)
	{
		this.actor = actor;
		this.dir = dir;
	}

	public abstract void Perform ();
	public abstract void Undo ();
}
