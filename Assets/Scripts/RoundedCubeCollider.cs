// Rounded Cube Collider
// https://gist.github.com/edgpedroso/850d3606bceef61bb3c6a858d29fce69
// Code is modified from @jasper-flick/CatLikeCoding
// https://catlikecoding.com/unity/tutorials/rounded-cube/
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Composed physics collider representing a Cube with rounded edges using Unity's primitive colliders.
///
/// !!! WARNING !!!
/// - This script will possibly destroy any Cube/Capsule colliders attached to this GameObject,
///   preferably use it as a standalone GameObject.
/// - Does not support transform scaling, please use this component's size vars.
/// </summary>
public class RoundedCubeCollider : MonoBehaviour
{
	public float XSize = 1;
	public float YSize = 1;
	public float ZSize = 1;
	public float Roundness = 0.05f;

	[SerializeField] private PhysicMaterial _material;
	public PhysicMaterial Material
	{
		get => _material;
		set => ValidateAndSetMaterial(value);
	}

	[SerializeField] private bool _isTrigger;
	public bool IsTrigger
	{
		get => _isTrigger;
		set => ValidateAndSetIsTrigger(value);
	}

	private List<BoxCollider> _boxColliders = new List<BoxCollider>();
	private List<CapsuleCollider> _capsuleColliders = new List<CapsuleCollider>();

#if UNITY_EDITOR
    private bool _wasTrigger;
    private PhysicMaterial _lastMaterial;
    private void OnValidate()
    {
        EditorApplication.delayCall += () =>
        {
            // Avoid null reference errors when reloading unity.
            // Since we are using a delayedCall,
            // we need to check if this instance still exists.
            if (this == null)
                return;
            
            Generate();
            ValidateAndSetMaterial(_material);
            ValidateAndSetIsTrigger(_isTrigger);

            // Destroy any additional box and capsule colliders
            var boxColliders = GetComponents<BoxCollider>();
            for (int i = 0; i < boxColliders.Length; i++)
            {
                if (!_boxColliders.Contains(boxColliders[i]))
                {
                    DestroyImmediate(boxColliders[i]);
                }
            }
            var capsuleColliders = GetComponents<CapsuleCollider>();
            for (int i = 0; i < capsuleColliders.Length; i++)
            {
                if (!_capsuleColliders.Contains(capsuleColliders[i]))
                {
                    DestroyImmediate(capsuleColliders[i]);
                }
            }
        };
    }

    [MenuItem("GameObject/Create Other/Rounded Cube Collider")]
    private static void CreateRoundedCube()
    {
        var go = new GameObject("RoundedCubeCollider");
        Undo.RegisterCreatedObjectUndo(go, "RoundedCubeCollider");
        go.AddComponent<RoundedCubeCollider>();

        var selection = Selection.activeGameObject;
        if (selection != null)
        {
            go.transform.SetParent(selection.transform);
        }
    }
#endif

	private void Generate()
	{
		// BOXES

		PopulateComponents(ref _boxColliders, 3);

		float doubledRoundness = Roundness * 2;

		_boxColliders[0].size = new Vector3(XSize,
											YSize - doubledRoundness,
											ZSize - doubledRoundness);
		_boxColliders[1].size = new Vector3(XSize - doubledRoundness,
											YSize,
											ZSize - doubledRoundness);
		_boxColliders[2].size = new Vector3(XSize - doubledRoundness,
											YSize - doubledRoundness,
											ZSize);

		// CAPSULES

		Vector3 min = Vector3.one * Roundness;
		Vector3 half = new Vector3(XSize, YSize, ZSize) * 0.5f;
		Vector3 max = new Vector3(XSize, YSize, ZSize) - min;

		PopulateComponents(ref _capsuleColliders, 12);

		SetCapsuleCollider(_capsuleColliders[0], 0, half.x, min.y, min.z);
		SetCapsuleCollider(_capsuleColliders[1], 0, half.x, min.y, max.z);
		SetCapsuleCollider(_capsuleColliders[2], 0, half.x, max.y, min.z);
		SetCapsuleCollider(_capsuleColliders[3], 0, half.x, max.y, max.z);

		SetCapsuleCollider(_capsuleColliders[4], 1, min.x, half.y, min.z);
		SetCapsuleCollider(_capsuleColliders[5], 1, min.x, half.y, max.z);
		SetCapsuleCollider(_capsuleColliders[6], 1, max.x, half.y, min.z);
		SetCapsuleCollider(_capsuleColliders[7], 1, max.x, half.y, max.z);

		SetCapsuleCollider(_capsuleColliders[8], 2, min.x, min.y, half.z);
		SetCapsuleCollider(_capsuleColliders[9], 2, min.x, max.y, half.z);
		SetCapsuleCollider(_capsuleColliders[10], 2, max.x, min.y, half.z);
		SetCapsuleCollider(_capsuleColliders[11], 2, max.x, max.y, half.z);
	}

	private void PopulateComponents<T>(ref List<T> components,
									   int count) where T : Component
	{
		count = Mathf.Max(1, count);
		components = GetComponents<T>().ToList();

		// Remove exceeding from the list
		for (int i = components.Count - 1; i >= count; i--)
		{
			components.RemoveAt(i);
		}
		for (int i = 0; i < count; i++) // Add remaining
		{
			if (components.Count > i)
				continue;

			components.Add(gameObject.AddComponent<T>());
		}
	}

	private void SetCapsuleCollider(CapsuleCollider target,
									int direction,
									float x,
									float y,
									float z)
	{
		var half = new Vector3(XSize, YSize, ZSize) * 0.5f;
		var v = new Vector3(x, y, z);
		target.center = v - half;
		target.direction = direction;
		target.radius = Roundness;
		target.height = v[direction] * 2f;
	}

	protected void ValidateAndSetMaterial(PhysicMaterial newValue)
	{
		bool needsToUpdate = _material != newValue;
#if UNITY_EDITOR
        if (_lastMaterial != newValue)
        {
            needsToUpdate = true;
            _lastMaterial = newValue;
        }
#endif
		if (!needsToUpdate)
			return;

		_boxColliders.ForEach(c => c.material = newValue);
		_capsuleColliders.ForEach(c => c.material = newValue);
		_material = newValue;
	}

	protected void ValidateAndSetIsTrigger(bool newValue)
	{
		bool needsToUpdate = _isTrigger != newValue;
#if UNITY_EDITOR
        if (_wasTrigger != newValue)
        {
            needsToUpdate = true;
            _wasTrigger = newValue;
        }
#endif
		if (!needsToUpdate)
			return;

		_boxColliders.ForEach(c => c.isTrigger = newValue);
		_capsuleColliders.ForEach(c => c.isTrigger = newValue);
		_isTrigger = newValue;
	}

	private void Reset()
	{
		name = "RoundedCube";
	}
}