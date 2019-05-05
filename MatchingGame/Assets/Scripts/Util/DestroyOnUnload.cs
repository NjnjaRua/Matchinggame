using UnityEngine;
using System.Collections;

public class DestroyOnUnload : MonoBehaviour {

	public bool dontDestroyOnLoad = true;

	void Awake()
	{
		if (dontDestroyOnLoad) 
			Object.DontDestroyOnLoad (gameObject);
	}
}
