using UnityEngine;
using System.Collections;

public class LookAtControl : MonoBehaviour {

	[SerializeField] Transform _lookAtTF;
	private Transform _selfTF;
	void Awake () {
		_selfTF = transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(_lookAtTF == null){
			this.enabled = false;
			return;
		}
		_selfTF.LookAt(_lookAtTF.position);
	}
}
