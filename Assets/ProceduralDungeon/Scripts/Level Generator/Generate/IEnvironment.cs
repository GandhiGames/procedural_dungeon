using UnityEngine;
using System.Collections;

namespace ProceduralDungeon
{
	public interface IEnvironment
	{
		bool IsBuilt { get; set; }
		DungeonArray2D<CellType> Cells { get; set; }
		void BuildWithSeed (Vector2 levelSize, int seed);
		Vector2 GetRandomCellFloorCoord ();
	}
}
