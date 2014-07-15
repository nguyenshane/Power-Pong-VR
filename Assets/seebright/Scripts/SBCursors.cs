using UnityEngine;
using System.Collections;

/**
 * Seebright Cursors v1.0 - Seebright, Inc
 *
 *  Copyright 2014 Seebright. 
 *  All rights reserved.
 * 
 * @author      John Murray - <john@seebright.com>
 * @author      Scott Holman - <scott@seebright.com>
 * @version     1.0
 * @data		2014
 * 
 */

public class SBCursors : MonoBehaviour
{
	public static Vector3 cursorPeriscopePosition;
	public static Vector3 cursorEyePosition;
	public static GameObject periscope;
	private static bool isPeriscopeCreated = false;

	public static void StartCursors()
	{
		periscope = new GameObject("PeriscopeCamera");
		periscope.AddComponent<Camera>();
		periscope.camera.fieldOfView = 60f;

		periscope.camera.enabled = true;
	}

	// Primarily handles the cursor locations.
	public static void CursorFixedUpdate ()
	{
		if(periscope.transform.parent != SeebrightSDK.currentCamera.sbRightCamera.gameObject.transform)
		{
			periscope.transform.parent = SeebrightSDK.currentCamera.sbRightCamera.gameObject.transform;
			OffsetCam(SeebrightSDK.currentCamera.sbRightCamera.gameObject.transform);
		}
		if(SeebrightSDK.singleton.isRightEyeDominant)
		{
			PositionCursor(SeebrightSDK.currentCamera.sbRightCamera.camera);
		}
		else
		{
			PositionCursor(SeebrightSDK.currentCamera.sbLeftCamera.camera);
		}
	}

	private static void OffsetCam(Transform dominantCamera)
	{
		periscope.transform.position = dominantCamera.position;
		periscope.transform.rotation = dominantCamera.rotation;
		periscope.transform.localPosition = new Vector3(0,0,0);
		periscope.transform.localRotation = Quaternion.identity;
		//periscope.transform.localPosition += new Vector3(43.415f / 1000f, 101.107f / 1000f, 3.759f / 1000f);		/*!<Orignal Fresnel unit offset*/
		periscope.transform.localPosition += new Vector3(53.856f / 1000f, 131.107f / 1000f, -24.973f / 1000f);		/*!<Pre-Alpha Frensel unit offset*/
		periscope.transform.localEulerAngles = new Vector3(5.30f, 0 ,0);											/*!<Pre-Alpha Frensel unit pitch*/
		periscope.transform.localPosition -= new Vector3(SeebrightSDK.currentCamera.IPD/2, 0, 0);
	}

	private static void PositionCursor(Camera trackingCamera)
	{
		SeebrightSDK.singleton.cursor3D.transform.position = periscope.transform.position;
		SeebrightSDK.singleton.cursor3D.transform.localPosition = periscope.camera.ViewportToWorldPoint (new Vector3(cursorPeriscopePosition.x, cursorPeriscopePosition.y, cursorPeriscopePosition.z));
		cursorEyePosition = trackingCamera.WorldToViewportPoint(SeebrightSDK.singleton.cursor3D.transform.position);
	}

}

