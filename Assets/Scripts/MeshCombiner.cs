using UnityEngine;
using System.Collections.Generic;

public class MeshCombiner : MonoBehaviour
{
	void Start()
	{
		CombineMeshes();
	}

	void CombineMeshes()
	{
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		List<CombineInstance> combineInstances = new List<CombineInstance>();

		Dictionary<Material, List<CombineInstance>> materialToCombine = new Dictionary<Material, List<CombineInstance>>();

		foreach (MeshFilter mf in meshFilters)
		{
			MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
			if (renderer == null) continue;

			Material mat = renderer.sharedMaterial;
			if (!materialToCombine.ContainsKey(mat))
				materialToCombine[mat] = new List<CombineInstance>();

			CombineInstance ci = new CombineInstance
			{
				mesh = mf.sharedMesh,
				transform = mf.transform.localToWorldMatrix
			};
			materialToCombine[mat].Add(ci);

			mf.gameObject.SetActive(false);
		}

		foreach (var entry in materialToCombine)
		{
			GameObject combinedObject = new GameObject("CombinedMesh");
			combinedObject.transform.position = transform.position;
			MeshFilter mf = combinedObject.AddComponent<MeshFilter>();
			MeshRenderer mr = combinedObject.AddComponent<MeshRenderer>();
			mr.sharedMaterial = entry.Key;

			mf.mesh = new Mesh();
			mf.mesh.CombineMeshes(entry.Value.ToArray(), true);
			combinedObject.AddComponent<MeshCollider>();
		}
	}
}