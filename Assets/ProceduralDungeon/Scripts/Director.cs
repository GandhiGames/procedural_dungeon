using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProceduralDungeon
{
	[RequireComponent (typeof(Dungeon))]
	[RequireComponent (typeof(EnvironmentBuilder))]
	[RequireComponent (typeof(DungeonTexturePack))]
	public class Director : MonoBehaviour
	{
		public Vector2 LevelSize;
		public int Seed;
		
		private static readonly string SCRIPT_NAME = typeof(Director).Name;
		
		private ITexturePack[] texturePacks;

		// Use this for initialization
		void Start ()
		{
			LoadTextures ();
				
			var dungeon = BuildEnvironment ();

			GenerateEnvironment (dungeon, texturePacks [Random.Range (0, texturePacks.Length)]);
			 
		}

		private IEnvironment BuildEnvironment ()
		{
			IEnvironment dungeon = GetComponent<Dungeon> ();
			
			dungeon.BuildWithSeed (LevelSize, Seed);

			return dungeon;
		}

		private void GenerateEnvironment (IEnvironment env, ITexturePack textures)
		{
			IEnvironmentBuilder dungeonBuilder = GetComponent<EnvironmentBuilder> ();
			
			dungeonBuilder.Generate (env, textures);
		}
		
		private void LoadTextures ()
		{
			texturePacks = gameObject.GetComponents<DungeonTexturePack> ();
			
			if (texturePacks.Length == 0) {
				Debug.LogError (SCRIPT_NAME + ": must have at least one texture pack attached");
				enabled = false;
			}
			
			
		}
	

	}
}
