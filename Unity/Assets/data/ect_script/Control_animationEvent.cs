using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Control_animationEvent : MonoBehaviour {

	[System.Serializable]
	private class endAnimationControl{
		public GameObject _LinkObject = null;
		public string _LinkFunction = "";
	}

	[SerializeField] List<endAnimationControl> _animationTriger = new List<endAnimationControl>();
	
	public void controlAnimationEvent(int selectIndex){
		if(_animationTriger.Count <= selectIndex) return;

		GameObject LinkObject = _animationTriger[selectIndex]._LinkObject;
		string LinkFunction = _animationTriger[selectIndex]._LinkFunction;

		if (string.IsNullOrEmpty(LinkFunction) || LinkObject == null) return;
		LinkObject.SendMessage(LinkFunction, null, SendMessageOptions.DontRequireReceiver);
	}
}
