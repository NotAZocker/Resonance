using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FirstPersonController))]
[RequireComponent(typeof(Rigidbody))]
public class PortalTravalerTeleportPlayer : MonoBehaviour, IPortalTravelerTeleport
{
	//Settings
	//[SerializeField]
	//private GameEvent playerTeleportedEvent;


	//Globals
	private FirstPersonController ownControler;
	private Rigidbody ownRigidbody;


	//Functions
	private void Awake()
	{
		ownControler = GetComponent<FirstPersonController>();
		ownRigidbody = GetComponent<Rigidbody>();
	}


	public void Teleport(Matrix4x4 teleportMatrix)
	{
		Matrix4x4 m = teleportMatrix * transform.localToWorldMatrix;

		ownRigidbody.position = m.GetColumn(3);
		ownRigidbody.rotation = m.rotation;

        Vector3 eulerRotation = ownRigidbody.rotation.eulerAngles;
        ownRigidbody.rotation = Quaternion.Euler(0, eulerRotation.y, 0);


        ownRigidbody.linearVelocity = teleportMatrix.MultiplyVector(ownRigidbody.linearVelocity);
		ownRigidbody.angularVelocity = teleportMatrix.MultiplyVector(ownRigidbody.angularVelocity);

		//playerTeleportedEvent.Invoke();
	}
}
