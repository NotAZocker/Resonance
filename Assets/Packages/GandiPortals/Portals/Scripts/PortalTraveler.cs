using UnityEngine;


[RequireComponent(typeof(IPortalTravelerTeleport))]
public class PortalTraveler : MonoBehaviour
{
	//Settings
	private readonly float teleportOffset = 0.1f;


	//Globals
	private static Transform cloneParent = null;
	private IPortalTravelerTeleport ownTeleport = default;
	private Portal approachedPortal = null;
	private Transform clone = null;


	//Functions
	private void OnValidate()
	{
		if (!TryGetComponent<IPortalTravelerTeleport>(out _))
		{
			Debug.LogError("PortalTraveler needs a Component of type IPortalTravelerTeleport as well.", this);
		}
	}


	private void Awake()
	{
		ownTeleport = GetComponent<IPortalTravelerTeleport>();

		if (cloneParent == null)
		{
			cloneParent = new GameObject(this.GetType().Name + " Clones").transform;
		}
	}


	private void Start()
	{
		CreateClone();
	}



	private void Update()
	{
		UpdateClone();
	}



	private void FixedUpdate()
	{
		CheckTeleportation();
	}


	private void UpdateClone()
	{
		if (approachedPortal != null)
		{
			Matrix4x4 m = approachedPortal.TeleportMatrix * transform.localToWorldMatrix;
			clone.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
		}
	}


	private void CheckTeleportation()
	{
		if (approachedPortal != null)
		{
			Vector3 offsetFromPortal = approachedPortal.transform.InverseTransformPoint(transform.position);
			if (offsetFromPortal.z > teleportOffset)
            {
                ownTeleport.Teleport(approachedPortal.TeleportMatrix);
				ApproachPortal(approachedPortal.LinkedPortal);
			}
		}
	}


	public void ApproachPortal(Portal portal)
	{
		approachedPortal = portal;
		clone.gameObject.SetActive(true);
	}


	public void LeavePortal(Portal portal)
	{
		if (approachedPortal == portal)
		{
			approachedPortal = null;
			clone.gameObject.SetActive(false);
		}
	}


	private void CreateClone()
	{
		clone = MimicObjectAndChildren(transform, cloneParent, gameObject.name + "Clone");
		clone.gameObject.SetActive(false);
	}


	private Transform MimicObjectAndChildren(Transform original, Transform parent, string name)
	{
		Transform copy = MimicObject(original, parent);
		copy.name = name;
		MimicChildren(original, copy);
		return copy;
	}


	private void MimicChildren(Transform original, Transform parent)
	{
		foreach (Transform child in original)
		{
			Transform copy = MimicObject(child, parent);
			MimicChildren(child, copy);
		}
	}


	private Transform MimicObject(Transform original, Transform parent)
	{
		GameObject copy;

		copy = new GameObject();
		copy.layer = gameObject.layer;
		copy.name = original.name;
		copy.transform.parent = parent;
		CopyTransform(original, copy.transform);

		//add mesh renderer
		if (original.TryGetComponent<MeshRenderer>(out MeshRenderer originalMeshRenderer) &&
			original.TryGetComponent<MeshFilter>(out MeshFilter originalMeshFilter))
		{
			MeshFilter copyMeshFilter = copy.AddComponent<MeshFilter>();
			copyMeshFilter.sharedMesh = originalMeshFilter.sharedMesh;

			MeshRenderer copyMeshRenderer = copy.AddComponent<MeshRenderer>();
			copyMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			copyMeshRenderer.sharedMaterial = originalMeshRenderer.sharedMaterial;
		}

		return copy.transform;
	}


	private void CopyTransform(Transform original, Transform copy)
	{
		copy.transform.localPosition = original.transform.localPosition;
		copy.transform.localRotation = original.transform.localRotation;
		copy.transform.localScale = original.transform.localScale;
	}
}


public interface IPortalTravelerTeleport
{
	public void Teleport(Matrix4x4 teleportMatrix);
}