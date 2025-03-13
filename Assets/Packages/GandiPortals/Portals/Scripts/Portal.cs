using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[SelectionBase]
public class Portal : MonoBehaviour
{
    [SerializeField] bool playerIsInBoundingBox;

    //Settings
    [SerializeField]
    private Portal linkedPortal;
    public Portal LinkedPortal { get => linkedPortal; }
    [SerializeField]
    private Renderer ScreenRenderer;
    [SerializeField]
    private Camera ownCamera;
	[SerializeField]
	private Material portalMaterial;
	[SerializeField]
    private float CameraNearClipOffset = -0.49f;
	[SerializeField]
	private float CameraNearClipLimit = 0.25f;
	[SerializeField]
    private Transform[] renderBoundingBoxCorners = default;


    //Globals
    public static Camera MainCamera { get; set; }
    private RenderTexture screenTextrue;
    public Matrix4x4 TeleportMatrix { get; private set; }
    [SerializeField] private Bounds renderBoundingBox;


    //Functions
    private void Awake()
    {
        CalculateBoundingBox();

        if (linkedPortal != null)
        {
            CalculateTeleportMatrix();
        }

        InitScreenTexture();
    }

    public void CalculateBoundingBox()
    {
        if (renderBoundingBoxCorners.Length != 2
                    || renderBoundingBoxCorners[0] == null
                    || renderBoundingBoxCorners[1] == null)
        {
            // Debug.LogWarning("Portal needs exact 2 Render Bounding Box Corners.", this);
            renderBoundingBox = new Bounds(Vector3.zero, Vector3.positiveInfinity);
        }
        else
        {
            if (renderBoundingBoxCorners[0].localPosition.sqrMagnitude < 0.01 && renderBoundingBoxCorners[1].localPosition.sqrMagnitude < 0.01)
            {
                // Debug.Log("Portals with global render bounding box might eat up resources unnecessarily.", this);
                renderBoundingBox = new Bounds(Vector3.zero, Vector3.positiveInfinity);
            }
            else
            {
                Vector3 center = (renderBoundingBoxCorners[0].position + renderBoundingBoxCorners[1].position) * 0.5f;
                Vector3 size = (renderBoundingBoxCorners[0].position - renderBoundingBoxCorners[1].position);
                size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
                renderBoundingBox = new Bounds(center, size);
                Debug.Log($"{renderBoundingBoxCorners[0].position} | {renderBoundingBoxCorners[1].position} | {center} | {size}", this);
            }
        }
    }

    private void CalculateTeleportMatrix()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(180, Vector3.up));
        TeleportMatrix = linkedPortal.transform.localToWorldMatrix * rotationMatrix * transform.worldToLocalMatrix;
    }
    private static int total;
    private static int boundingBox;
    private static int frustumCulled;

    private void OnGUI()
    {
        if (total == 0)
        {
            //Debug.Log("No portals", this);
            //return;
        }
        string s = $"Total: {total}; BoundingBox: {boundingBox}; FrustumCulled: {frustumCulled} ";
        
        GUI.Label(new Rect(10, 10, 300, 20), s);
    }

    private void Update()
    {
        total = 0;
        boundingBox = 0;
        frustumCulled = 0;
    }

    private void LateUpdate()
    {
        if (linkedPortal == null)
        {
            return;
        }

        if (MainCamera == null)
        {
            return;
        }

        total++;

        CheckResolution();

        playerIsInBoundingBox = renderBoundingBox.Contains(MainCamera.transform.position);

        if (!renderBoundingBox.Contains(MainCamera.transform.position))
        {
            boundingBox++;
            return;
        }

        if (!linkedPortal.ScreenRenderer.isVisible)
        {
            frustumCulled++;
            return;
        }

        RenderScreen(MainCamera);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PortalTraveler>(out PortalTraveler traveler))
        {
            traveler.ApproachPortal(this);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PortalTraveler>(out PortalTraveler traveler))
        {
            traveler.LeavePortal(this);
        }
    }


    private void CheckResolution()
	{
        if (screenTextrue.width != Screen.width || screenTextrue.height != Screen.height)
		{
            Debug.Log("Res chaned.");
            InitScreenTexture();
        }
	}


    private void InitScreenTexture()
	{
        screenTextrue = new RenderTexture(Screen.width, Screen.height, 32, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
        ownCamera.targetTexture = screenTextrue;
        if (portalMaterial == null)
		{
            Debug.Log("Portal material is not set.");
            return;
		}
		ScreenRenderer.material = portalMaterial;
		ScreenRenderer.material.SetTexture("_MainTex", screenTextrue);
	}


    private void RenderScreen(Camera camera)
    {
        linkedPortal.ScreenRenderer.enabled = false;

        Matrix4x4 m = TeleportMatrix * camera.transform.localToWorldMatrix;
        ownCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
        SetNearClipPlane();

        ownCamera.Render();

        linkedPortal.ScreenRenderer.enabled = true;
    }


    //Use custom projection matrix to align portal camera's near clip plane with the Screen of the portal
    //Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO
    private void SetNearClipPlane()
    {
        //Learning resource:
        //http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform clipPlane = linkedPortal.transform;
        
        Vector3 camSpacePos = ownCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = ownCamera.worldToCameraMatrix.MultiplyVector(-clipPlane.forward);
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + CameraNearClipOffset;

        //Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > CameraNearClipLimit)
        {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            //Update projection based on new clip plane
            //Calculate matrix with player cam so that player camera settings (fov, etc) are used
            ownCamera.projectionMatrix = ownCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            ownCamera.ResetProjectionMatrix();
        }
    }


    public static void Link(Portal portalA, Portal portalB)
	{
        if (portalA.linkedPortal != null || portalB.linkedPortal != null)
		{
            throw new System.ArgumentException("Portals are already linked.");
		}

        portalA.linkedPortal = portalB;
        portalB.linkedPortal = portalA;

        portalA.CalculateTeleportMatrix();
        portalB.CalculateTeleportMatrix();

        portalA.CalculateBoundingBox();
        portalB.CalculateBoundingBox();
    }

    internal void Unlink()
    {
        linkedPortal = null;
    }
}
