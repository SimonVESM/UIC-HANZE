using UnityEditor;
using UnityEngine;

public static class ColliderUtility 
{
	[MenuItem("IDS/Remove colliders from Prefabs")]
	private static void RemoveCollider()
	{
		foreach (GameObject gameObject in Selection.gameObjects)
		{
			// Get the Prefab Asset root GameObject and its asset path.
			GameObject assetRoot = gameObject;
			string assetPath = AssetDatabase.GetAssetPath(assetRoot);
			// Load the contents of the Prefab Asset.
			if (string.IsNullOrEmpty(assetPath))
			{
				Debug.Log("Asset path is empty, this utility only works on prefabs.");
				continue;
			}

			GameObject contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);

			Collider[] colliders = contentsRoot.GetComponentsInChildren<Collider>();
			for (int i = 0; i < colliders.Length; i++)
			{
				GameObject.DestroyImmediate(colliders[i]);
			}

			// Save contents back to Prefab Asset and unload contents.
			PrefabUtility.SaveAsPrefabAsset(contentsRoot, assetPath);
			PrefabUtility.UnloadPrefabContents(contentsRoot);
		}
	}

}
