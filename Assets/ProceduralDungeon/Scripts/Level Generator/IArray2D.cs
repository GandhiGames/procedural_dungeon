using UnityEngine;
using System.Collections;

public interface IArray2D<T>
{

	T Get (int x, int y);
	
	T Get (Vector2 coord);
	
	void Set (int x, int y, T value);
	
	void Set (Vector2 coord, T value);
	
	bool NodeContainsValue (Vector2 coord, T value);
	
	bool NodeContainsValue (int x, int y, T value);
	
	void Fill (T value);
	
	bool IsValid (Vector2 coord);
	
	T[,] ToArray2D ();
}
