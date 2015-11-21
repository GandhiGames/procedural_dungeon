using UnityEngine;
using System.Collections;

namespace ProceduralDungeon
{
	public interface ITexturePack
	{
		Vector2 TileSize { get; }
		
		Sprite GetSpriteForCellType (CellType type);
	}
}
