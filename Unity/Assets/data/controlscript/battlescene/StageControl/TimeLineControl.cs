
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeLineControl : MonoBehaviour {

	[SerializeField] float _moveTime = 5;
	[SerializeField] bool _loopChack = false;
	//[SerializeField] StartGameControl _startControl;

	private Transform _selfTF;
	private Vector3 _startPosition = Vector3.zero;
	private Vector3 _startOriPosition = Vector3.zero;
	private Vector3 _drowPosition = Vector3.zero;

	private int _maxCount = 0;
	private int _currectCount = 0;
	private List<float> _keyList = new List<float>();
	private Dictionary<float, List<Transform>> _controlSpotPoint = new Dictionary<float, List<Transform>>();

	void Awake () {
		_selfTF = transform;
		_startOriPosition = _startPosition = _selfTF.localPosition;
		_startPosition.z = 0;
		_drowPosition.z = _moveTime;

		BulletCreate[] BulletCreateList = _selfTF.GetComponentsInChildren<BulletCreate>();

		int maxCount = BulletCreateList.Length;

		float zPosition = 0;
		for(int i = 0; i < maxCount; i++){
			_selectControl = BulletCreateList[i].transform;
			zPosition = _selectControl.position.z;
			_createList = null;

			try
			{
				_createList = _controlSpotPoint[zPosition];
			}
			catch (KeyNotFoundException)
			{
				_keyList.Add(zPosition);
				_createList = new List<Transform>();
				_createList.Add(_selectControl);
				_controlSpotPoint.Add(zPosition, _createList);
				continue;
			}

			_createList.Add(_selectControl);
		}

		_maxCount = _keyList.Count;
		if(_maxCount == 0) {
			this.enabled = false;
			return;
		}

		_currectCount = 0;
		_keyList.Sort();
		nextObjectData();
	}

	private Transform _selectControl = null;
	private List<Transform> _createList = null;
	bool nextObjectData(){
		if(_maxCount <= _currectCount) {
			_selectControl = null;
			return false;
		}

		_createList = _controlSpotPoint[_keyList[_currectCount++]];
		_selectControl = _createList[0];
		return true;
	}

	void resetObjectData(){
		_delayTime = 0;
		_currectCount = 0;
		_selfTF.localPosition = _startPosition;
		nextObjectData();
		this.enabled = true;
	}

	//private bool _interfaceViewChack = false;
	public void resetOriginalData(){
		if(_selfTF == null) return;

		resetObjectData();
		_selfTF.localPosition = _startOriPosition;
		//_interfaceViewChack = true;
	}

	private float _delayTime = 0;
	void Update () {
		if(_delayTime > 0){
			if(_delayTime < 0.5f){
				_delayTime += Time.deltaTime;
				return;
			}

			resetObjectData();
			return;
		}

		Vector3 position = (_drowPosition*Time.deltaTime);
		_selfTF.position -= position;
		/*
		if(_interfaceViewChack){
			if(_selfTF.position.z < 6){
				_startControl.viewInterface();
				_interfaceViewChack = false;
			}
			return;
		}*/

		if(_selectControl.position.z < 8.5f){
			for(int i = 0; i < _createList.Count; i++)
				(_createList[i].GetComponent("BulletCreate") as BulletCreate).resetBullet(true, false);

			if(!nextObjectData()){
				if(_loopChack) _delayTime = Time.deltaTime;
				else this.enabled = false;
			}
		}
	}
}
