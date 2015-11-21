using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProceduralDungeon
{
	public class ObjectPool : MonoBehaviour
	{
		public GameObject[] objectPrefabs;

		public List<GameObject>[] pooledObjects;

		public int[] amountToBuffer;

		public int defaultBufferAmount = 3;

		protected GameObject containerObject;

		private static ObjectPool _instance;
		public static ObjectPool instance { get { return _instance; } }

		void Awake ()
		{
			_instance = this;

			containerObject = new GameObject ("ObjectPool");
			
			pooledObjects = new List<GameObject>[objectPrefabs.Length];
			
			int i = 0;
			foreach (GameObject objectPrefab in objectPrefabs) {
				pooledObjects [i] = new List<GameObject> (); 
				
				int bufferAmount;
				
				if (i < amountToBuffer.Length)
					bufferAmount = amountToBuffer [i];
				else
					bufferAmount = defaultBufferAmount;
				
				for (int n = 0; n < bufferAmount; n++) {
					GameObject newObj = Instantiate (objectPrefab) as GameObject;
					newObj.name = objectPrefab.name;
					newObj.SetActive (false);
					PoolObject (newObj);
				}
				
				i++;
			}
		}

		public GameObject GetObjectForType (string objectType, bool onlyPooled)
		{
			for (int i = 0; i < objectPrefabs.Length; i++) {
				GameObject prefab = objectPrefabs [i];
				if (prefab.name == objectType) {

					if (pooledObjects [i].Count > 0) {
						GameObject pooledObject = pooledObjects [i] [0];
												
						if (pooledObject) {
							pooledObjects [i].RemoveAt (0);
							pooledObject.transform.SetParent (null, false);
						} else {
							Debug.LogError (objectType + ": not found in pool");
						}
											
						return pooledObject;

					} else if (!onlyPooled) {
						GameObject newObj = (GameObject)Instantiate (prefab);
						newObj.name = prefab.name;
						return newObj;
					}

					break;

				}
			}

			return null;
		}

		public void PoolObject (GameObject obj)
		{
			for (int i = 0; i < objectPrefabs.Length; i++) {
				if (objectPrefabs [i].name == obj.name) {
					obj.SetActive (false);
					obj.transform.SetParent (containerObject.transform);
					pooledObjects [i].Add (obj);
					return;
				}
			}
						
			Debug.LogError (obj.name + ": not setup to use object pool");
		}



	}


}
