using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProceduralDungeon
{
	
	
	
	public abstract class Environment : MonoBehaviour, IEnvironment
	{
		public bool IsBuilt { get; set; }
		public DungeonArray2D<CellType> Cells { get; set; }

		protected Dictionary<int, List<Vector2>> regions = new Dictionary<int, List<Vector2>> ();
		protected Vector2 levelSize;
		protected readonly Vector2[] directions = new Vector2[]{Vector2.up, -Vector2.up, Vector2.right, -Vector2.right};
	
		private static readonly string SCRIPT_NAME = typeof(Environment).Name;
		
		public virtual void BuildWithSeed (Vector2 levelSize, int seed)
		{
			if (!IsSizeInitialised (levelSize)) {
				Debug.LogError (SCRIPT_NAME + ": for best effect level sides should not be even");
			}
		
			this.levelSize = levelSize;
			
			Random.seed = seed;
			
			Cells = new DungeonArray2D<CellType> ((int)levelSize.x, (int)levelSize.y);
		}

		public Vector2 GetRandomCellFloorCoord ()
		{
			Vector2? regionVector = null;

			do {
				var randIndex = (int)Random.Range (0, regions.Keys.Count);
			
				var randVector = (int)Random.Range (0, regions [randIndex].Count);
			
				regionVector = regions [randIndex] [randVector];

			} while (!regionVector.HasValue && !Cells.NodeContainsValue (regionVector.Value, CellType.PATH_FLOOR));
			
			return regionVector.Value;
		}
	
		
		private bool IsSizeInitialised (Vector2 levelSize)
		{
			return levelSize.x % 2 != 0 && levelSize.y % 2 != 0;
		}
		
		
		
	}
	
	
}
