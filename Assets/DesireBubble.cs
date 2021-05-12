using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireBubble : MonoBehaviour
{
	[SerializeField] private Vector3 rotation;

#if UNITY_EDITOR
	private void OnValidate() {
		transform.eulerAngles = rotation;
	}
#endif

	private void OnEnable() {
		transform.eulerAngles = rotation;
	}
}
