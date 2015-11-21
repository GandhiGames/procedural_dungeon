using UnityEngine;
using System.Collections;

namespace ProceduralDungeon
{
	public class DungeonTexturePack : MonoBehaviour, ITexturePack
	{
		public Sprite pathFloor;
		public Sprite roomFloor;
		public Sprite roomNWall;
		public Sprite roomNEWall;
		public Sprite roomNWWall;
		public Sprite roomSWall;
		public Sprite roomEWall;
		public Sprite roomSEWall;
		public Sprite roomSWWall;
		public Sprite roomWWall;
		public Sprite openDoor;
		public Sprite closedDoor;
		
		private Vector2 tileSize;
		public Vector2 TileSize {
			get {
				return tileSize;
			}
		}

		private static readonly string SCRIPT_NAME = typeof(DungeonTexturePack).Name;
		
		void OnEnable ()
		{
			tileSize = GetTileSize (pathFloor);
		}
		
		private Vector2 GetTileSize (Sprite sprite)
		{
			return new Vector2 (sprite.bounds.size.x, sprite.bounds.size.y);
		}
		
		public Sprite GetSpriteForCellType (CellType type)
		{
			
			switch (type) {
			case CellType.PATH_FLOOR:
				return pathFloor;
			case CellType.ROOM_FLOOR:
				return roomFloor;
			case CellType.ROOM_N_WALL:
				return roomNWall;
			case CellType.ROOM_NE_WALL:
				return roomNEWall;
			case CellType.ROOM_NW_WALL:
				return roomNWWall;
			case CellType.ROOM_S_WALL:
				return roomSWall;
			case CellType.ROOM_SE_WALL:
				return roomSEWall;
			case CellType.ROOM_SW_WALL:
				return roomSWWall;
			case CellType.ROOM_W_WALL:
				return roomWWall;
			case CellType.OPENDOOR:
				return openDoor;
			case CellType.CLOSEDDOOR:
				return closedDoor;
			case CellType.ROOM_E_WALL:
				return roomEWall;
			default:
				Debug.LogError (SCRIPT_NAME + ": texture not found");
				return null;
			}

		}
	}
}
