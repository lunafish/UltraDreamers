using UnityEngine;
using System.Collections;

public class BlockControlScript : TouchControl {

	[SerializeField] PlayerControl _playScript;
	[SerializeField] Transform _cameraObject;
	[HideInInspector] public bool _chaseCoinControl = false;
	[HideInInspector] public bool _enableTouchControl = true;


	private Transform _moveTF;
	private tk2dSprite _chracterSprite = null;
	private tk2dSpriteAnimator _characterAni = null;
	private Transform _wingTransform = null;
	private GameObject _wingObject = null;
	private Vector3 _startEulerAngles;
	private Vector3 _startPosition;
	private GameObject _selfObject = null;

	private Vector3 _leftEulerAngles = new Vector3(27.66381f, 66.72912f, 47.22709f);
	private Vector3 _rightEulerAngles = new Vector3(30.16306f, 295.9201f, 314.0796f);
	private Vector3 _UderAttackEulerAngles = new Vector3(53, 0, -30);

	void Awake(){
		Application.targetFrameRate = 60;
		_moveTF = _playScript.transform;
		_selfObject = gameObject;

		_wingTransform = _moveTF.FindChild("Wing");
		_wingObject = _wingTransform.gameObject;
		_startPosition = _wingTransform.localPosition;
		_startEulerAngles = _wingTransform.eulerAngles;
		_chracterSprite = _moveTF.FindChild("FlightUnit").GetComponent("tk2dSprite") as tk2dSprite;
		_characterAni =  _moveTF.FindChild("FlightUnit").GetComponent("tk2dSpriteAnimator") as tk2dSpriteAnimator;
	}

	protected override bool getMoveChack(Vector3 currentMove){
		return _moveChack;
	}

	private float _StopTouchTime = 0;
	private float _moveTouchTime = 0;
	private float _Sensitivity = 2;
	
	private int _oldDirection = 1;
	private Vector3 _LastTouchPoint = Vector3.zero;
	protected override void touchBegan(Vector3 moveP){ _LastTouchPoint = moveP; }
	protected override void touchMove(Vector3 moveP){
		Vector3 drowPosition = (moveP - _LastTouchPoint);
		_LastTouchPoint = moveP;
		if(!_enableTouchControl) return;

		drowPosition.x *= _Sensitivity;
		drowPosition.z = drowPosition.y*_Sensitivity;
		drowPosition.y = 0;

		Vector3 endPosition = _moveTF.localPosition + drowPosition;
		if(endPosition.z <= -1.65f) endPosition.z = -1.65f;
		else if(endPosition.z >= 5.5f) endPosition.z = 5.5f;

		if(endPosition.x <= -1.4f) endPosition.x = -1.4f;
		else if(endPosition.x >= 1.4f) endPosition.x = 1.4f;

		_chaseCoinControl = (endPosition.z > 1.2f);
		_moveTF.localPosition = endPosition;

		Vector3 localEg = _cameraObject.eulerAngles;
		localEg.z = _moveTF.localPosition.x;
		_cameraObject.eulerAngles = localEg;

		_moveTouchTime += Time.deltaTime;
		if(_moveTouchTime < 0.1f) return;
		_moveTouchTime = 0;

		bool reverseChackX = false;
		bool reverseChackY = false;
		float xPosition = drowPosition.x;
		float yPosition = drowPosition.z;
		if(xPosition < 0) {
			reverseChackX = true;
			xPosition*= -1;
		}
		if(yPosition < 0) {
			reverseChackY = true;
			yPosition*= -1;
		}

		int directionChack = -1;
		if(xPosition < yPosition){
			if(!reverseChackY) directionChack = 1;
			else directionChack = 0;
		}
		else if(xPosition > yPosition){
			if(!reverseChackX) directionChack = 3;
			else directionChack = 2;
		}

		if(_oldDirection == directionChack) return;
		_StopTouchTime = 0;
		_oldDirection = directionChack;

		if(_stopSpriteAni) return;

		Vector3 imgScale = Vector3.one;
		//localEg = _startEulerAngles;
		switch(directionChack){
		case 0: // bottom 
			controlSprite("Character_1_Back");
			break;
		case 1: // up
			if(_oldDirection != 0) return;
			touchEnd(moveP);
			return;
		case 2: // left
			if(_oldDirection == 0) return;
			controlSprite("Character_1_R");
			imgScale.x = -1;
			if(!_stopWingObjectMoveChack){
				endPosition = _wingTransform.localPosition;
				endPosition.x = -0.06f;
				endPosition.y = 0.05f;
				_wingTransform.localPosition = endPosition;
			}

			//localEg = _leftEulerAngles;
			break;
		case 3: // right
			if(_oldDirection == 0) return;
			controlSprite("Character_1_R");

			if(!_stopWingObjectMoveChack){
				endPosition = _wingTransform.localPosition;
				endPosition.x = 0.06f;
				endPosition.y = 0.05f;
				_wingTransform.localPosition = endPosition;
			}

			//localEg = _rightEulerAngles;
			break;
		default:
			return;
		}

		_chracterSprite.scale = imgScale;
		rotatePlayerWingObject(directionChack);
		//_characterAni.Stop();
	}

	[HideInInspector] public bool _stopWingObjectMoveChack = false;
	public void rotatePlayerWingObject(int directionChack, float drowTime = 0.2f){
		if(_stopWingObjectMoveChack) return;

		Vector3 drowEulerAngles = _startEulerAngles;
		string endFunction = "";
		switch(directionChack){
		case 2: // left
			drowEulerAngles = _leftEulerAngles;
			break;
		case 3: // right
			drowEulerAngles = _rightEulerAngles;
			break;
		case 4: // under attack
			drowEulerAngles = _UderAttackEulerAngles;
			endFunction = "underAttackEndChack";
			break;
		}

		//Destroy(_wingTransform.GetComponent("iTween") as iTween);
		//iTween.Stop(_wingObject);
		ObjectRotateControl.rotateToObject(_wingObject, drowTime, drowEulerAngles, _selfObject, endFunction);
	}

	void underAttackEndChack(){
		rotatePlayerWingObject(0, 0.1f);
	}

	private bool _stopSpriteAni = false;
	void controlSprite(string indexControl){
		if(_stopSpriteAni) return;

		_characterAni.Play(_headString + indexControl);
		_chracterSprite.scale = Vector3.one;
	}

	protected override void touchMoveStop(Vector3 moveP){ 
		_StopTouchTime += Time.deltaTime;
		if(_StopTouchTime < 0.1f) return;
		touchEnd(moveP);
		_StopTouchTime = 0;
	}
	protected override void touchCancel(Vector3 moveP){ touchEnd(moveP);}
	protected override void touchEnd(Vector3 moveP){ 

		controlSprite(_oldDirection == 0 ? "Character_1_BackReturn" : "Character_1");
		_oldDirection = 1; 
		if(!_stopWingObjectMoveChack){
			_wingTransform.localPosition = _startPosition;
			rotatePlayerWingObject(0);
		}
	}

	private string _headString = "";
	public void ActSpriteAni(bool tfChack = false){
		_stopSpriteAni = false;
		if(!tfChack) _headString = "";
		else _headString = "tf_";
		controlSprite("Character_1");
		rotatePlayerWingObject(0);
	}

	public void stopSpriteAni(){
		_stopSpriteAni = true;

		if(_stopWingObjectMoveChack) return;
		_wingTransform.localPosition = _startPosition;
		_wingTransform.eulerAngles = _startEulerAngles;
	}
}
