using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PivotUtility {

	[MenuItem("IDS/Fix pivot")]
	public static void BottomCenterPivot ()
	{
		if (Selection.activeGameObject == null) return;

		Transform selectedTransform = Selection.activeTransform;
		Undo.RecordObject(selectedTransform, "Pivot point fix");
		MeshRenderer[] meshRenderers = selectedTransform.GetComponentsInChildren<MeshRenderer>();

		if (meshRenderers.Length == 0) return;

		Bounds bounds = meshRenderers[0].bounds;
		for (int i = 1; i < meshRenderers.Length; i++)
		{
			bounds.Encapsulate(meshRenderers[i].bounds);
		}

		List<Transform> children = new List<Transform>();
		foreach (Transform child in selectedTransform)
		{
			Undo.RecordObject(child, "Pivot point fix");
			children.Add(child);
		}
		selectedTransform.DetachChildren();
		selectedTransform.position = bounds.center + Vector3.down * bounds.extents.y;

		foreach (Transform child in children) child.parent = selectedTransform;
	}

}


