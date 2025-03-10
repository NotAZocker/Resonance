using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class PortalMainCamera : MonoBehaviour
{
    //Globals
    private Camera ownCamera;


    void Awake()
    {
        ownCamera = GetComponent<Camera>();

        if (Portal.MainCamera == null)
        {
            Portal.MainCamera = ownCamera;
        }
        else
		{
            Debug.LogError("Multiple PortalMainCamera Instances.");
		}
    }


    void OnDestroy()
    {
        if (Portal.MainCamera == ownCamera)
        {
            Portal.MainCamera = null;
        }
    }
}
