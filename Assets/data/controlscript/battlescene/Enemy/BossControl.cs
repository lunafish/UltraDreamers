using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class bossControlOption{

	public enum selectControlOption{
		delay_Option,
		animation_Control,
		LinerOption,
		loopOption
	}

	[Serializable]
	private class delay_Option{

		public float _drowTime = 0;
		public List<GameObject> _EndDelayActObject = new List<GameObject>();
	}

	[Serializable]
	private class animation_Control{
		[Serializable]
		public class animationList{
			public List<Animator> _actObject = new List<Animator>();
			public string _playChackAni = "";
		}

		public string _swapChackAni = "";
		public List<string> _stopChackAni = new List<string>();
		public List<animationList> _animationList = new List<animationList>();
	}

	[Serializable]
	private class LinerOption{
		public float _moveSpeed = 4;
		public float _drowTime = 0;
		public FlightOption.LinerTypeList _linerType = FlightOption.LinerTypeList.forward;
		public FlightOption.moveSpeedOption _smoothMoveChack = FlightOption.moveSpeedOption.linear;

		public int _randomLinerCount = 0;
		public bool _randomLiner = false;
		public int _randomForwardCount = 1;
	}

	[System.Serializable]
	private class loopOption{
		public int _loopStartIndex = 0;
	}

	[SerializeField] selectControlOption _flightType = selectControlOption.delay_Option;
	[SerializeField] delay_Option _delayOption = null;
	[SerializeField] animation_Control _animationControl = null;
	[SerializeField] LinerOption _linerOption = null;
	[SerializeField] loopOption _loopOption = null;

	private int _controlAniChackIndex = 0;
	private int _maxStopAniCount = 0;
	private string _swapChackAni = "";
	private List<string> _stopChackAni = new List<string>();
	private int _currentLinerRandomCount = 0;
	private Animator _chackSelectAni = null;
	public void resetControl(){

		_controlAniChackIndex = 0;
		_maxStopAniCount = 0;
		_stopChackAni.Clear();
		_swapChackAni = "";

		_chackSelectAni = null;
		_currentLinerRandomCount = 0;

		switch(_flightType){
		case selectControlOption.LinerOption:
			if(_linerOption._randomLiner) _currentLinerRandomCount = _linerOption._randomLinerCount;
			break;
		}
	}

	public selectControlOption nextPattern{ get { return _flightType; } }
	public int randomLinerChack{
		get {
			switch(_flightType){
			case selectControlOption.LinerOption:
				if(_linerOption._randomLiner){
					if(_currentLinerRandomCount != 0){
						_currentLinerRandomCount--;
						return 2;
					}
					
					_currentLinerRandomCount = _linerOption._randomLinerCount;
					return 1;
				}
				break;
			}
			
			return 0;
		}
	}

	public int loopStartIndex{
		get{
			switch(_flightType){			
			case selectControlOption.loopOption:
				return _loopOption._loopStartIndex; 
			}
			return 0;
		}
	}

	public int randomForwardCount{
		get{
			switch(_flightType){
			case selectControlOption.LinerOption:
				if(_linerOption._randomLiner){
					return _linerOption._randomForwardCount;
				}
				break;
			}
			
			return 0;
		}
	}

	public float MoveSpeed { get { 
			switch(_flightType){
			case selectControlOption.LinerOption:
				return _linerOption._moveSpeed; 
			}
			
			return 0;
		} 
	}

	public FlightOption.moveSpeedOption moveSpeedType{
		get{
			switch(_flightType){
			case selectControlOption.LinerOption:
				return _linerOption._smoothMoveChack; 
			}
			
			return FlightOption.moveSpeedOption.linear;
		}
	}

	public float drowTime { get { 
			switch(_flightType){
			case selectControlOption.LinerOption:
				return _linerOption._drowTime; 
			case selectControlOption.delay_Option:
				return _delayOption._drowTime; 
			}
			
			return 0;
		} 
	}

	public FlightOption.LinerTypeList linerType{
		get{
			switch(_flightType){
			case selectControlOption.LinerOption:
				return _linerOption._linerType;
			}

			return FlightOption.LinerTypeList.forward;
		}
	}

	public void EndDelayActObject(){
		switch(_flightType){
		case selectControlOption.delay_Option:
			int maxCount = _delayOption._EndDelayActObject.Count;
			for(int i = 0; i < maxCount; i++)
				_delayOption._EndDelayActObject[i].SetActive(true);
			break;
		}
	}

	public void startAnimationControl(){
		switch(_flightType){
		case selectControlOption.animation_Control:

			_maxStopAniCount = _animationControl._stopChackAni.Count;
			if(_maxStopAniCount > 0) _stopChackAni.AddRange(_animationControl._stopChackAni);
			else{
				_maxStopAniCount = 0;
				_stopChackAni.Clear();
			}

			_swapChackAni = _animationControl._swapChackAni;
			if(!string.IsNullOrEmpty(_swapChackAni)) _controlAniChackIndex = 1;
			else _controlAniChackIndex = 0;

			int maxCount = _animationControl._animationList.Count;
			animation_Control.animationList selectAnimationList = null;

			for(int i = 0; i < maxCount; i++){
				selectAnimationList = _animationControl._animationList[i];
				if(i == 0) {
					//Debug.Log("_playChackAni  : " + selectAnimationList._playChackAni);
					_chackSelectAni = selectAnimationList._actObject[0];
				}

				if(!string.IsNullOrEmpty(selectAnimationList._playChackAni)){
					int nMaxCount = selectAnimationList._actObject.Count;
					for(int j = 0; j < nMaxCount; j++)
						selectAnimationList._actObject[j].SetTrigger(selectAnimationList._playChackAni);
				}
			}

			break;
		}
	}

	public bool endAnimationChack{
		get{
			switch(_flightType){
			case selectControlOption.animation_Control:
				switch(_controlAniChackIndex){
				case 1:
					if(_chackSelectAni.GetCurrentAnimatorStateInfo(0).IsName(_swapChackAni)) {
						return false; 
					}

					_swapChackAni = "";
					_controlAniChackIndex = 0;
					break;
				default:
					//Debug.Log("chackANi :" + _maxStopAniCount);
					//Debug.Log(_chackSelectAni.GetCurrentAnimatorStateInfo(0).nameHash);

					for(int i = 0; i < _maxStopAniCount; i++){
						//Debug.Log("pause ani : " + Animator.StringToHash("Base Layer." + _stopChackAni[i]));
						if(_chackSelectAni.GetCurrentAnimatorStateInfo(0).IsName(_stopChackAni[i])) {
							return false; 
						}
					}
					
					//Debug.Log("End_ANi");
					_chackSelectAni = null;
					return true;
				}
				break;
			}
			
			return false;
		}
	}

}

public class BossControl : BulletBase {

	[SerializeField] float _destorySize = 0.3f;
	//[SerializeField] collisionChack _collistionControl = null;
	[SerializeField] List<bossControlOption> _bossControlOption = new List<bossControlOption>();

	private bool _awakeChack = false;
	void Awake(){
		if(_awakeChack) return;

		_awakeChack = true;
		_selfTF = transform;
		SetDestorySize(_destorySize,_destorySize);
	}

	public override BulletBase CreateBullectValue(bullectOption bullectOp, Transform baseTF, Vector3 startPosition, BulletCreate parentCreate, bool createParent, bool dropPowerUp){
		Awake();
		setDrowStage(_selfTF, this, BulletBase.objectPosition.enemy, stopTimeLine: true, NonDestroyChack: false);
		_selfTF.position = baseTF.position;
		_currentCount = 0;
		
		_copyVPosition = _selfTF.position;
		_copyVPosition.y = 0;
		SetDestoryCenter(_copyVPosition);
		
		nextBlockObject();
		return null;
	}

	private bossControlOption _selectFlight = null;
	private bossControlOption.selectControlOption _flightList = bossControlOption.selectControlOption.delay_Option;
	protected override void nextBlockObject(){

		if(_currentCount >= _bossControlOption.Count){
			_nextObjectChack = false;
			return;
		}

		_nextObjectChack = true;
		_selectFlight = _bossControlOption[_currentCount++];
		_flightList = _selectFlight.nextPattern;
		_smoothOption = _selectFlight.moveSpeedType;
		_currentTime = 0;
		_chackDrowTime = _selectFlight.drowTime;
		_selectFlight.resetControl();
		controlMoveOption();
	}
	
	void controlMoveOption(){
		//Debug.Log("--------------" + _flightList);
		switch(_flightList){
		case bossControlOption.selectControlOption.LinerOption:
			Vector3 endPosition = linerOptionControl(_selectFlight.randomLinerChack, _selectFlight.linerType, _selfTF, _selectFlight.randomForwardCount);
			if(endPosition.Equals(Vector3.zero)) return;

			_drowPoint = endPosition.normalized * _selectFlight.MoveSpeed;
			_drowPoint.y = 0;
			break;
		case bossControlOption.selectControlOption.delay_Option:
			break;
		case bossControlOption.selectControlOption.animation_Control:
			_nextObjectChack = false;
			_selectFlight.startAnimationControl();
			break;
		case bossControlOption.selectControlOption.loopOption:
			_currentCount = _selectFlight.loopStartIndex;
			nextBlockObject();
			return;
		}


	}
	
	void Update(){

		_copyVPosition = _selfTF.position;
		_copyVPosition.y = 0;
		SetDestoryCenter(_copyVPosition);

		switch(_flightList){
		case bossControlOption.selectControlOption.LinerOption:
			MoveControlFunction();
			if(chackDrowTime()) nextBlockObject();
			break;
		case bossControlOption.selectControlOption.delay_Option:
			if(chackDrowTime()){
				_selectFlight.EndDelayActObject();
				nextBlockObject();
			}
			break;
		case bossControlOption.selectControlOption.animation_Control:
			if(_selectFlight.endAnimationChack)
				nextBlockObject();
			
			break;
		}
	}

	//public override void allStopBulletValue(bool enabledControl){}
	public override bool destroyChack { get { return false; } }
	public override bool stopBulletObject(bool notAddStorage = false, bool notDestroyParent = true){ return false; }
}
