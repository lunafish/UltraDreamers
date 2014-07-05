using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartGameControl : MonoBehaviour {

	[SerializeField] List<GameObject> _gameStartObject;
	[SerializeField] GameObject _interfaceObject;
	[SerializeField] GameObject _LoadingCloudObject;
	[SerializeField] MoveStageControl _startCloudObject;
	[SerializeField] TimeLineControl _timeLineValue;
	[SerializeField] MoveStageControl _cloudStageControl;

	protected GameObject _touchObject;

	void Awake(){
		controlGameStart(false);
	}

	void controlGameStart(bool activeControl){
		int maxCount = _gameStartObject.Count;
		_LoadingCloudObject.SetActive(!activeControl);
		for(int i = 0; i < maxCount; i++){
			_gameStartObject[i].SetActive(activeControl);
		}

		_interfaceObject.SetActive(false);
	}

	void gameStartButton(GameObject touchObject){
		_touchObject = touchObject;
		touchObject.SetActive(false);
		controlGameStart(true);
		_timeLineValue.resetOriginalData();
		_cloudStageControl.resetMovePosition();
		_startCloudObject.resetMovePosition();
	}

	public void stopGameControl(){
		controlGameStart(false);
		_touchObject.SetActive(true);
	}

	public void viewInterface(){
		_interfaceObject.SetActive(true);
	}
}
