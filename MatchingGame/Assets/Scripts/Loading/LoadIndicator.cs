using UnityEngine;
using System.Collections;

public class LoadIndicator : MonoBehaviour {
	public RectTransform barRectTrans;
	public Vector3 rotateVector = new Vector3(0,0,-8);
	void Update () {
		barRectTrans.Rotate(rotateVector);
	}
}
