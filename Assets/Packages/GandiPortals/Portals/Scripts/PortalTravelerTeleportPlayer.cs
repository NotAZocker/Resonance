using ECM2;
using System;
using UnityEngine;

public class PortalTravelerTeleportPlayer : MonoBehaviour, IPortalTravelerTeleport
{
    Character character;

    public event Action OnTeleport;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Teleport(Matrix4x4 teleportMatrix)
    {
        Matrix4x4 m = teleportMatrix * transform.localToWorldMatrix;

        character.TeleportPosition(teleportMatrix.GetColumn(3));
        character.TeleportRotation(teleportMatrix.rotation);

        OnTeleport?.Invoke();
    }
}
