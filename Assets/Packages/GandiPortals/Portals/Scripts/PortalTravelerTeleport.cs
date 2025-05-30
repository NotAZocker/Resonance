using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PortalTravelerTeleport : MonoBehaviour, IPortalTravelerTeleport
{
	//Globals
	private Rigidbody ownRigidbody;


	//Functions
	protected virtual void Awake()
	{
		ownRigidbody = GetComponent<Rigidbody>();
	}


	public virtual void Teleport(Matrix4x4 teleportMatrix)
	{
		Matrix4x4 m = teleportMatrix * transform.localToWorldMatrix;

        transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        ownRigidbody.position = m.GetColumn(3);
		ownRigidbody.rotation = m.rotation;
		ownRigidbody.linearVelocity = teleportMatrix.MultiplyVector(ownRigidbody.linearVelocity);
		ownRigidbody.angularVelocity = teleportMatrix.MultiplyVector(ownRigidbody.angularVelocity);
	}
}
