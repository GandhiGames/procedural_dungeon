using UnityEngine;
using System.Collections;

namespace ProceduralDungeon
{
	public class DungeonArray2D<T> : IArray2D<T>
	{
		/// For each open position in the dungeon, the index of the connected region
		/// that that position is a part of.
		private T[,] cells;
	
		private Vector2 arraySize;
	
		public DungeonArray2D (int width, int height)
		{
			cells = new T[width, height];
			arraySize = new Vector2 (width, height);
		}
	
		public T Get (int x, int y)
		{
			if (IsValid (x, y)) {
				return cells [x, y];
			}
		
			return default(T);
		}
	
		public T Get (Vector2 coord)
		{
			int x = (int)coord.x;
			int y = (int)coord.y;
		
			return Get (x, y);
		}
	
		public void Set (int x, int y, T value)
		{
			if (IsValid (x, y)) {
				cells [x, y] = value;
			}
		}
	
		public void Set (Vector2 coord, T value)
		{
			int x = (int)coord.x;
			int y = (int)coord.y;
		
			Set (x, y, value);
		}
	
		public bool NodeContainsValue (Vector2 coord, T value)
		{
			int x = (int)coord.x;
			int y = (int)coord.y;
		
			return NodeContainsValue (x, y, value);
		}
	
		public bool NodeContainsValue (int x, int y, T value)
		{
			if (IsValid (x, y)) {
				return cells [x, y].Equals (value);
			}
		
			return false;
		}
	
		public void Fill (T value)
		{
			for (int x = 0; x < arraySize.x; x++) {
				for (int y = 0; y < arraySize.y; y++) {
					cells [x, y] = value;
				}
			}
		}
	
		public bool IsValid (Vector2 coord)
		{
			int x = (int)coord.x;
			int y = (int)coord.y;
					
			return IsValid (x, y);
		}

	
		public bool IsValid (int x, int y)
		{
			return x > 0 && x < arraySize.x 
				&& y > 0 && y < arraySize.y;
		}

		
		public T[,] ToArray2D ()
		{
			return cells;
		}
	}
}
