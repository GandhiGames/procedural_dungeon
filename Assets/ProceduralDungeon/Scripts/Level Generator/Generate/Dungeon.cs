using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProceduralDungeon
{
	public enum CellType
	{
		NONE = 0,
		PATH_FLOOR,
		ROOM_FLOOR,
		ROOM_N_WALL,
		ROOM_NE_WALL,
		ROOM_NW_WALL,
		ROOM_S_WALL,
		ROOM_SE_WALL,
		ROOM_SW_WALL,
		ROOM_W_WALL,
		ROOM_E_WALL,
		OPENDOOR,
		CLOSEDDOOR,
		WALL
	}
	public class Dungeon : Environment
	{

		public int RoomPlacementAttempts = 300;
		
		/// Increasing this allows rooms to be larger.
		public int RoomExtraSize = 3;
		
		public int WindingPercent = 50;
		
		/// The inverse chance of adding a connector between two regions that have
		/// already been joined. Increasing this leads to more loosely connected
		/// dungeons.
		public int ExtraConnectionChance = 20;

		
		private List<Rect> rooms = new List<Rect> ();
		private List<Vector2> roomCorners = new List<Vector2> ();
		
		//private Dictionary<int, List<Vector2>> regions = new Dictionary<int, List<Vector2>> ();
		
	
		
		// Used as key for regions dictionary.
		private int currentRegion = -1;
		
		public override void BuildWithSeed (Vector2 levelSize, int seed)
		{
			base.BuildWithSeed (levelSize, seed);
			
		
			
			// Fill all cells with default cell type.
			Cells.Fill (CellType.WALL);
			
			// Add rooms.
			AddRooms ();
			
			// Fill in all of the empty space with mazes.
			for (var y = 1; y < levelSize.y; y += 2) {
				for (var x = 1; x < levelSize.x; x += 2) {
					
					if (!Cells.NodeContainsValue (x, y, CellType.WALL))
						continue;
					var pos = new Vector2 (x, y);
					GrowMaze (pos);
				}
			}

			Connect ();
			
			RemoveDeadEnds ();
			
			foreach (var room in rooms) {
				DecorateRoom (room);
			}
			
			SetBuildComplete ();
			
		}
		
		private void DecorateRoom (Rect room)
		{
			for (int y = (int) room.y; y < room.yMax; y++) {
				for (int x = (int)room.x; x < room.xMax; x++) {

					var door = false;
					var pos = new Vector2 (x, y);
					foreach (var dir in directions) {
						if (Cells.NodeContainsValue (pos + dir, CellType.PATH_FLOOR)) {
							door = true;
							break;
						}
					}

					if (door) {
						Cells.Set (x, y, CellType.CLOSEDDOOR);
						continue;
					}

					if (x == room.xMax - 1 && y == room.yMax - 1) {
						Cells.Set (x, y, CellType.ROOM_NE_WALL);
					} else if (x == room.x && y == room.y) {
						Cells.Set (x, y, CellType.ROOM_SW_WALL);
					} else if (x == room.x && y == room.yMax - 1) {
						Cells.Set (x, y, CellType.ROOM_NW_WALL);
					} else if (x == room.xMax - 1 && y == room.y) {
						Cells.Set (x, y, CellType.ROOM_SE_WALL);
					} else if (x != room.xMax && y == room.y) {
						Cells.Set (x, y, CellType.ROOM_S_WALL);
					} else if (x == room.xMax - 1 && y != room.yMax) {
						Cells.Set (x, y, CellType.ROOM_E_WALL);
					} else if (x == room.x && y != room.yMax) {
						Cells.Set (x, y, CellType.ROOM_W_WALL);
					} else if (x != room.xMax && y == room.yMax - 1) {
						Cells.Set (x, y, CellType.ROOM_N_WALL);
					} 
				}
			}


		}

		private void AddRooms ()
		{
			for (var i = 0; i < RoomPlacementAttempts; i++) {
				int size = (int)Random.Range (2, 3 + RoomExtraSize) * 2 + 1;
				int rectangularity = Random.Range (0, 1 + (int)(size / 2)) * 2;
			
				int width = size;
				int height = size;
				if ((int)Random.Range (0, 1.1f) == 0) {
					width += rectangularity;
				} else {
					height += rectangularity;
				}
			
				var coordX = (int)Random.Range (0, (int)(levelSize.x - width) / 2) * 2 + 1;
				var coordY = (int)Random.Range (0, (int)(levelSize.y - height) / 2) * 2 + 1;
			
				var room = new Rect (coordX, coordY, width, height);
			
				var overlaps = false;
				foreach (var other in rooms) {
					if (room.Overlaps (other)) {
						overlaps = true;
						break;
					}
				}
			
				if (overlaps)
					continue;
			
				rooms.Add (room);
				roomCorners.AddRange (GetRoomCorners (room));
			
				StartRegion ();
				foreach (var pos in GetCellCoordList (room)) {
					Carve (pos, CellType.ROOM_FLOOR);
				} 
			}
		}
		
		/// <summary>
		/// Implementation of the "growing tree" algorithm from here:
		/// http://www.astrolog.org/labyrnth/algrithm.htm.
		/// </summary>
		/// <param name="start">Start position.</param>
		private void GrowMaze (Vector2 start)
		{
			
			var cellCoords = new List<Vector2> ();
			
			Vector2? lastDir = null;
			
			StartRegion ();
			Carve (start, CellType.PATH_FLOOR);
			
			cellCoords.Add (start);
			
			while (cellCoords.Count > 0) {
				Vector2 cell = cellCoords [cellCoords.Count - 1];
								
				var unmadeCells = new List<Vector2> ();
				
				foreach (var dir in directions) {
					
					if (CanCarve (cell, dir)) {
						unmadeCells.Add (dir);
					}
				} 
				
				if (unmadeCells.Count > 0) {
					Vector2? dir = null;
					
					if (lastDir.HasValue && unmadeCells.Contains (lastDir.Value) && Random.Range (0, 100) < WindingPercent) {
						dir = lastDir.Value;
					} else {
						int randIndex = (int)Random.Range (0, unmadeCells.Count - 1);
						dir = unmadeCells [randIndex];
					}
					
					Carve (cell + dir.Value, CellType.PATH_FLOOR);
					Carve (cell + dir.Value * 2, CellType.PATH_FLOOR);
					cellCoords.Add (cell + dir.Value * 2);
					lastDir = dir;
					
				} else {
					cellCoords.RemoveAt (cellCoords.Count - 1);
					lastDir = null;
				}
				
				
			}
			
		}

		private void Connect ()
		{
			var openRegions = new List<int> ();
			
			for (int i = 0; i <=currentRegion; i++) {
				openRegions.Add (i);
			}
		
			// Create inverse mapping of current region dictionary.
			var inverseRegions = new Dictionary<Vector2, int> ();
			foreach (var key in regions.Keys) {
				foreach (var pos in regions[key]) {
					if (!inverseRegions.ContainsKey (pos)) {
						inverseRegions.Add (pos, key);
					} 
				}
			}
			
			var randIndex = (int)Random.Range (0, openRegions.Count);
			var mainRegion = regions [openRegions [randIndex]];
			openRegions.Remove (randIndex);

			
			bool loop = true;
			int esc = 0;
			
			var regionUsed = new List<int> ();
			
			while (loop) {
				Vector2? newRegionPosition = null;
			
				foreach (var pos in mainRegion) {
		
					if (IsRoomCorner (pos)) {
						continue;
					}
					
					foreach (var dir in directions) {
						var newPos = pos + dir;
					
						// Only include valid wall types.
						if (!Cells.NodeContainsValue (newPos, CellType.WALL) || Cells.NodeContainsValue (newPos + dir, CellType.WALL))
							continue;
							
						// Don't want to connect to end of world.
						if (!Cells.IsValid (newPos + dir))
							continue;
						
						// Don't connect to itself.
						if (regions [randIndex].Contains (newPos + dir))
							continue;
							
						// Has to connect to a region already conected.
						if ((!inverseRegions.ContainsKey (newPos + dir)) 
							|| (!openRegions.Contains (inverseRegions [newPos + dir]) && (int)Random.Range (0, 400) > 1))
							continue;

						if (IsRoomCorner (newPos + dir)) {
							continue;
						}

					
						AddJunction (newPos);
						
						openRegions.Remove (inverseRegions [newPos + dir]);
						
						newRegionPosition = newPos + dir;
						
						break;
	
					}
					
//					Debug.Log (newRegionPosition.HasValue);
					
					
					// Reached dead end - needs to step back one/2
					if (newRegionPosition.HasValue) {
					
						// No luck with this region.
//						if (regionUsed.Count > 0 
//							&& regionUsed [regionUsed.Count - 1] == inverseRegions [newRegionPosition.Value]) {
//							Debug.Log ("here");
//							regionUsed.RemoveAt (regionUsed.Count - 1);
//							// Step back one region. 
//							randIndex = regionUsed [regionUsed.Count - 1];
//							mainRegion = regions [randIndex];
//							break;
//						} else {
						regionUsed.Add (inverseRegions [newRegionPosition.Value]);
						randIndex = inverseRegions [newRegionPosition.Value];
						mainRegion = regions [randIndex];
							
						break;
						//}
					} 
	
				}
				
				
				
				if (!newRegionPosition.HasValue) {
					regionUsed.RemoveAt (regionUsed.Count - 1);
					// Step back one region. 
					randIndex = regionUsed [regionUsed.Count - 1];
					mainRegion = regions [randIndex];
				}
				
				if (openRegions.Count < 1 || esc > 2000) {
					loop = false;
				}
				esc++;
			}

		}
		

		private void RemoveDeadEnds ()
		{
			var done = false;
			int c = 0;
			while (!done) {
				done = true;
				
				for (int x = 0; x < levelSize.x; x++) {
					for (int y = 0; y < levelSize.y; y++) {
						var pos = new Vector2 (x, y);

						if (Cells.NodeContainsValue (pos, CellType.WALL))
							continue;
						
						var exits = 0;
						
						foreach (var dir in directions) {
							var newPos = pos + dir;

							if (!Cells.IsValid (newPos)) {
								continue;
							}

							if (!Cells.NodeContainsValue (newPos, CellType.WALL))
								exits++;
						}

						c++;

						if (c > 56132) {
							done = true;
							break;
						} 
						
						if (exits <= 1) {
							done = false;		
							Cells.Set (x, y, CellType.WALL); 
						} 

					}
				}
				
				
			}
			
		}
		
		#region Helper Methods


		private bool IsNextToPath (Vector2 pos)
		{
			foreach (var dir in directions) {
				if (Cells.NodeContainsValue (pos + dir, CellType.PATH_FLOOR)) {
					return true;
				}
			}

			return false;
		}
		
		private IEnumerable<Vector2> GetRoomCorners (Rect rect)
		{
			for (float y = rect.y; y <= rect.yMax - 1; y++) {
				for (float x = rect.x; x <= rect.xMax - 1; x++) {
					if ((x == rect.xMax - 1 && y == rect.yMax - 1) || (x == rect.x && y == rect.y) 
						|| (x == rect.x && y == rect.yMax - 1) || (x == rect.xMax - 1 && y == rect.y)) {
						yield return new Vector2 (x, y);
					} 
				}
			}
		}

		private bool IsRoomCorner (Vector2 pos)
		{
			return roomCorners.Contains (pos);
		}

		private Vector2? CanAddJunction (Vector2 pos)
		{
			foreach (var dir in directions) {
				
				var newPos = pos + dir * 2;
				
				if (!Cells.NodeContainsValue (newPos, CellType.WALL))
					return newPos;
			}

			return null;
		}

		private bool IsTypeOfWall (CellType type)
		{
			return type == CellType.ROOM_N_WALL ||
				type == CellType.ROOM_NE_WALL ||
				type == CellType.ROOM_NW_WALL ||
				type == CellType.ROOM_S_WALL ||
				type == CellType.ROOM_SE_WALL ||
				type == CellType.ROOM_SW_WALL ||
				type == CellType.ROOM_W_WALL ||
				type == CellType.WALL;
		}
		
		private void StartRegion ()
		{
			currentRegion++;
		}
		
		private void SetBuildComplete ()
		{
			IsBuilt = true;
		}
		
		private IEnumerable<Vector2> GetCellCoordList (Rect room)
		{
			for (int x = (int)room.x; x < room.x + room.width; x++) {
				for (int y = (int) room.y; y < room.y + room.height; y++) {
					yield return new Vector2 (x, y);
				}
			}
		} 

		private void AddJunction (Vector2 pos)
		{
			/*if ((int)Random.Range (0, 4) == 0) {
				Cells.Set (pos, (int)Random.Range (0, 3) == 0 ? CellType.OPENDOOR : CellType.PATH_FLOOR);
			} else {
				Cells.Set (pos, CellType.CLOSEDDOOR);
			} */
			
			Cells.Set (pos, CellType.PATH_FLOOR);
		}
		
		private void Carve (Vector2 position, CellType type)
		{
			Cells.Set (position, type);
			
			//regions [(int)position.x, (int)position.y] = currentRegion;
			
			if (!regions.ContainsKey (currentRegion)) {
				var positionList = new List<Vector2> ();
				positionList.Add (position);
				regions.Add (currentRegion, positionList);
			} else {
				regions [currentRegion].Add (position);
			}
			
		}
		
		/// <summary>
		/// Determines whether this instance can carve the specified coord.
		/// </summary>
		/// <returns> Returns true if coord is of type wall.</returns>
		/// <param name="coord">Coordinate.</param>
		private bool CanCarve (Vector2 startCoord, Vector2 dir)
		{
			Vector2 nextCoord = startCoord + dir * 3;
			
			//if (!Cells.NodeContainsValue (nextCoord, CellType.WALL)) {
			//	return false;
			//}

			if (!Cells.IsValid (nextCoord))
				return false;
			
			Vector2 endCoord = startCoord + dir * 2;
			
			return Cells.NodeContainsValue (endCoord, CellType.WALL);		
		}
		
		private void Carve (int x, int y, CellType type)
		{
			Carve (new Vector2 (x, y), type);
		}
		#endregion
	}
}
