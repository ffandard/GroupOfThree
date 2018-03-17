using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFrustrum : MonoBehaviour
{
	[SerializeField]
	private Camera cam;

	[SerializeField]
	private Transform quad;


	void Start ()
	{
		ScaleVerticaly ();
	}

	#if UNITY_EDITOR

	void LateUpdate ()
	{
		if (!Application.isPlaying)
		{
			ScaleVerticaly ();
		}
	}

	#endif

	private void ScaleVerticaly ()
	{
		float screenAspect = (float)Screen.height / (float)Screen.width;

		float quadAspect = quad.localScale.y / quad.localScale.x;

		cam.ResetProjectionMatrix();

		if(screenAspect > quadAspect)
		{
			cam.fieldOfView = Mathf.Atan2(quad.localScale.y*0.5f,quad.position.z-cam.transform.position.z)*2* Mathf.Rad2Deg;
			Matrix4x4 matrix  = cam.projectionMatrix;
			matrix.m00 = matrix.m11 * quadAspect;
			matrix.m11 = matrix.m00 / screenAspect;
			cam.projectionMatrix = matrix;
		}
		else
		{
			cam.fieldOfView = Mathf.Atan2(quad.localScale.y*0.5f,quad.position.z-cam.transform.position.z)*2* Mathf.Rad2Deg;
		}
	}

	private void OnDrawGizmos ()
	{
		Gizmos.color = Color.grey;
		Gizmos.DrawWireCube(quad.position, new Vector3(quad.localScale.x, quad.localScale.y, 0));
	}
}