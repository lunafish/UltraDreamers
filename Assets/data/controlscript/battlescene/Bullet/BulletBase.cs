using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class bullectOption{
	
	[SerializeField] BulletBase bullectOriginal;
	[SerializeField] int _drowShotCount = -1;
	[SerializeField] float _createSpeed = 9.6f;
	[SerializeField] List<FlightOption> _flightType = new List<FlightOption>();
	
	public FlightOption flightType(int index){
		if(index < _flightType.Count) return _flightType[index];
		return null;
	}
	
	public int drowShotCount { get { return _drowShotCount; } }
	public float createSpeed { get { return _createSpeed; } }
	public int maxFlightTypeCount { get { return _flightType.Count; } }
	public BullectControl CreateBullectValue(Transform baseTF, Vector3 startPosition, BulletCreate parentCreate, bool createParent, bool dropPowerUp){
		if(bullectOriginal == null) return null;
		return (BullectControl)bullectOriginal.CreateBullectValue(this, baseTF, startPosition, parentCreate, createParent,dropPowerUp);
	}
}

[System.Serializable]
public class FlightOption{
	
	public enum FlightTypeList{
		liner,
		chaser,
		delay, 
		loop,
		delete,
		rotate
	}
	
	public enum rotateTypeList{
		all,
		sprite,
		non_sprite
	}
	
	public enum LinerTypeList{
		forward,
		left,
		right,
		backward,
		left_forward,
		left_backward,
		Right_forward,
		Right_backward
	}
	
	public enum moveSpeedOption{
		linear,
		FastAndSlow,
		SlowAndFast
	}
	
	[System.Serializable]
	private class LinerOption{
		public float _moveSpeed = 4;
		public float _drowTime = 0;
		public LinerTypeList _linerType = LinerTypeList.forward;
		
		public int _randomLinerCount = 0;
		public bool _randomLiner = false;
		public int _randomForwardCount = 1;
	}
	
	[System.Serializable]
	private class chaserOption{
		public Transform _chasesTF = null; // onother player objet
		public List<Transform> _randromChaseTf = new List<Transform>();
		public float _moveSpeed = 4;
		public float _drowTime = 0;
		//public bool _TrackAfterCrash = false;
		public int _chaseRenewCount = 0;
		public float _chaseRenewTime = 0.1f;
	}
	
	[System.Serializable]
	private class delayOption{
		public float _drowTime = 0;
	}
	
	
	[System.Serializable]
	private class RotateOption{
		public LinerTypeList _linerType = LinerTypeList.forward;
		public bool _rotateAni = false;
		public float _rotateSpeed = 0;
		public float _rotateTime = 0;
	}
	
	[System.Serializable]
	private class loopOption{
		//public int _loopCount = 0;
		//public float _delayTime = 0;
		public int _loopStartIndex = 0;
	}
	
	[SerializeField] moveSpeedOption _smoothMoveChack = moveSpeedOption.linear;
	[SerializeField] FlightTypeList _flightType = FlightTypeList.liner;
	[SerializeField] LinerOption _linerOption = null;
	[SerializeField] chaserOption _chaseOption = null;
	[SerializeField] delayOption _delayOption = null;
	[SerializeField] loopOption _loopOption = null;
	[SerializeField] RotateOption _rotateOption = null;

	private int _currentLinerRandomCount = 0;
	public void resetValue(bool nonLinerReset = false){
		_currentTime = 0;
		_chaseRenewCount = _chaseOption._chaseRenewCount;
		_chaseRenewTime = _chaseOption._chaseRenewTime;
		if(_chaseRenewTime <= 0) _chaseRenewTime = 0.1f;

		if(!nonLinerReset) _currentLinerRandomCount = _linerOption._randomLinerCount;
		//_loopCount = _loopOption._loopCount;
		//if(_loopCount < 0) _loopCount = 0;
	}
	
	private float _currentTime = 0;
	private int _chaseRenewCount = 0;
	private float _chaseRenewTime = 0;
	public bool chackChaseCount{
		get{
			switch(_flightType){			
			case FlightTypeList.chaser:
				if(_chaseRenewCount != 0){
					_currentTime += Time.deltaTime;
					if(_chaseRenewTime <= _currentTime){
						_currentTime -= _chaseRenewTime;
						_chaseRenewCount--;
						return true;
					}
				}
				return false; 
			}
			
			return false;
		}
	}
	
	public Vector3 getChasePosition{
		get {
			switch(_flightType){			
			case FlightTypeList.chaser:
				if(_chaseOption._chasesTF != null) return _chaseOption._chasesTF.localPosition; 
				else{
					int maxCount = _chaseOption._randromChaseTf.Count;
					if(maxCount == 0) return Vector3.zero;
					return _chaseOption._randromChaseTf[Random.Range(0,maxCount)].localPosition;
				}
			}
			
			return Vector3.zero;
			
		}
	}
	
	public int loopStartIndex{
		get{
			switch(_flightType){			
			case FlightTypeList.loop:
				return _loopOption._loopStartIndex; 
			}
			return 0;
		}
	}
	/*
	private int _loopCount = 0;
	public bool loopCountChack {
		get{
			switch(_flightType){			
			case FlightTypeList.loop:
				if(0 < _loopCount--) return true;
				return false; 
			}
			return false;
		}
	}*/
	
	public LinerTypeList linerType{
		get{
			switch(_flightType){
			case FlightTypeList.liner:
				return _linerOption._linerType;
			case FlightTypeList.rotate:
				return _rotateOption._linerType;
			}
			
			return LinerTypeList.forward;
		}
	}

	public int randomLinerChack{
		get {
			switch(_flightType){
			case FlightTypeList.liner:
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

	public int randomForwardCount{
		get{
			switch(_flightType){
			case FlightTypeList.liner:
				if(_linerOption._randomLiner){
					return _linerOption._randomForwardCount;
				}
				break;
			}
			
			return 0;
		}
	}
	
	public FlightTypeList flightType { get { 
			return _flightType; 
		} 
	}
	
	public float MoveSpeed { get { 
			switch(_flightType){
			case FlightTypeList.liner:
				return _linerOption._moveSpeed; 
				
			case FlightTypeList.chaser:
				return _chaseOption._moveSpeed; 
				
			case FlightTypeList.rotate:
				return _rotateOption._rotateAni ? _rotateOption._rotateSpeed : 0;
			}
			
			return 0;
		} 
	}
	
	public moveSpeedOption smoothOption{
		
		get { 
			if(drowTIme > 0) return _smoothMoveChack;
			return moveSpeedOption.linear;
		}
	}
	
	public float drowTIme { get { 
			switch(_flightType){
			case FlightTypeList.liner:
				return _linerOption._drowTime; 
				
			case FlightTypeList.chaser:
				return _chaseOption._drowTime; 
				
			case FlightTypeList.delay:
				return _delayOption._drowTime; 
				
			case FlightTypeList.rotate:
				return _rotateOption._rotateAni ? _rotateOption._rotateTime : 0;
			}
			
			return 0;
		} 
	}
	/*
	public bool TrackAfterACrash { get { 
			switch(_flightType){
			case FlightTypeList.chaser:
				return _chaseOption._TrackAfterCrash; 
			}

			return false;
		} BulletBase
	}*/
}

public class BulletBase : MonoBehaviour {
	public enum objectPosition{
		enemy,
		palyer,
		coin,
		powerUp
	}

	protected float _chackDrowTime = 0;
	protected float _currentTime = 0;

	protected bool _nextObjectChack = false;
	protected Transform _selfTF;
	protected int _currentCount = 0;
	protected Bounds _copyBounds;
	protected Vector3 _copyVPosition = Vector3.zero;
	protected Vector3 _drowPoint = Vector3.zero;
	protected FlightOption.moveSpeedOption _smoothOption = FlightOption.moveSpeedOption.linear;

	protected void SetDestorySize(float widthSize, float heightSize){
		_copyBounds.size = new Vector3(widthSize,0,heightSize);
	}

	public Vector3 LPosition { get { return _selfTF.localPosition; } }
	public Vector3 VPosition { get { return _selfTF.position; } }
	public Bounds destroyBound { get { return _copyBounds; } }
	public virtual bool destroyChack { get { return true; } }
	public virtual BulletBase CreateBullectValue(bullectOption bullectOp, Transform baseTF, Vector3 startPosition, BulletCreate parentCreate, bool createParent, bool dropPowerUp){
		return null;
	}

	public virtual void allStopBulletValue(bool enabledControl){}
	public virtual bool stopBulletObject(bool notAddStorage = false, bool notDestroyParent = true){ return false; }
	public virtual int chackCollisionValue(BullectControl chackBullet){ return 0; }
	
	public virtual float returnMagnitude(Vector3 basePositon){ return 0; }
	public virtual bool destroyBullectChack(bool bladeDelete){ return false;}
	public virtual int chackCrushPlayerValue(Vector3 _playerPS){ return 0;}
	public virtual void coinChaseFlowAct(bool actChack){}
	protected virtual void nextBlockObject(){}

	protected virtual void resetBullet(bool resetChack, bool bladeDelete = false){
		_randomLinerControl.Clear();
	}

	protected Vector3 getDirectionValue(FlightOption.LinerTypeList linerType, Transform baseTFValue){

 		Vector3 endPosition = Vector3.zero;
		switch(linerType){
		case FlightOption.LinerTypeList.forward:
			endPosition = baseTFValue.forward;
			break;
		case FlightOption.LinerTypeList.backward:
			endPosition = baseTFValue.forward * -1; 
			break;
		case FlightOption.LinerTypeList.left:
			endPosition = baseTFValue.right;
			break;
		case FlightOption.LinerTypeList.right: 
			endPosition = baseTFValue.right * -1; // 플레이어 기준
			break;
		case FlightOption.LinerTypeList.left_forward:
			endPosition = baseTFValue.forward + baseTFValue.right;
			break;
		case FlightOption.LinerTypeList.left_backward:
			endPosition = (baseTFValue.forward * -1) + baseTFValue.right;
			break;
		case FlightOption.LinerTypeList.Right_forward:
			endPosition = baseTFValue.forward + (baseTFValue.right * -1);
			break;
		case FlightOption.LinerTypeList.Right_backward:
			endPosition = (baseTFValue.forward * -1) + (baseTFValue.right * -1);
			break;
		}
		
		return endPosition;
	}

	protected Vector3 getEulerAnglesValue(FlightOption.LinerTypeList linerType, Transform baseTFValue){

		Vector3 EulerAngles = baseTFValue.eulerAngles;
		EulerAngles.x = EulerAngles.z = 0;
		switch(linerType){
		case FlightOption.LinerTypeList.backward:
			EulerAngles.y *= -1;
			break;
		case FlightOption.LinerTypeList.left:
			EulerAngles.y += 90;
			break;
		case FlightOption.LinerTypeList.right: 
			EulerAngles.y -= 90;
			break;
		case FlightOption.LinerTypeList.left_forward:
			EulerAngles.y += 45;
			break;
		case FlightOption.LinerTypeList.left_backward:
			EulerAngles.y += 135;
			break;
		case FlightOption.LinerTypeList.Right_forward:
			EulerAngles.y -= 45;
			break;
		case FlightOption.LinerTypeList.Right_backward:
			EulerAngles.y -= 135;
			break;
		}
		
		return EulerAngles;
	}

	private List<int> _randomLinerControl = new List<int>();
	protected Vector3 linerOptionControl(int randomLinerChack, FlightOption.LinerTypeList linerType, Transform baseTFValue, int randomCount){
		
		Vector3 endPosition = Vector3.zero;
		switch(randomLinerChack){
		case 0:
			endPosition = getDirectionValue(linerType, baseTFValue);
			break;
		case 1:
			nextBlockObject();
			return Vector3.zero;
		case 2:// 주변을 렌덤으로 둥둥 떠다니는 옵션
			
			if(_randomLinerControl.Count == 0){
				for(int i = 0; i < 8; i++){
					_randomLinerControl.Add(i);
				}
				
				// forward 타입을 추가로 더 줘서 점점더 앞으로 전진하도록 수정
				for(int i = 0; i < randomCount; i++)
					_randomLinerControl.Add(0);
				
			}

			linerType = FlightOption.LinerTypeList.forward;
			
			int chackIndex = Random.Range(0, _randomLinerControl.Count);
			switch(_randomLinerControl[chackIndex]){
			case 1:
				linerType = FlightOption.LinerTypeList.backward;
				break;
			case 2:
				linerType = FlightOption.LinerTypeList.left;
				break;
			case 3:
				linerType = FlightOption.LinerTypeList.right;
				break;
			case 4:
				linerType = FlightOption.LinerTypeList.left_backward;
				break;
			case 5:
				linerType = FlightOption.LinerTypeList.Right_backward;
				break;
			case 6:
				linerType = FlightOption.LinerTypeList.left_forward;
				break;
			case 7:
				linerType = FlightOption.LinerTypeList.Right_forward;
				break;
			}
			
			_randomLinerControl.RemoveAt(chackIndex);
			endPosition = getDirectionValue(linerType, baseTFValue);
			break;
		}
		
		return endPosition;
	}

	protected void settingLookAtTF(Transform drowSpriteTF, tk2dSprite drowSprite, objectPosition obPoint, Vector3 startSpriteScale){

		if(drowSpriteTF != null)
		{
			Vector3 loatEG = _selfTF.localEulerAngles;
			loatEG.z = 360 - loatEG.y;
			loatEG.x = 54;
			loatEG.y = 0;
			drowSpriteTF.eulerAngles = loatEG;
			
			switch(obPoint){
			case objectPosition.palyer:
				break;
			default:
				loatEG = startSpriteScale;
				loatEG.y *= -1;
				drowSprite.scale = loatEG;
				break;
			}
		}
	}

	protected Vector3 MoveControlFunction(){
		float delta = 1;

		if(_currentTime >= 0 && _chackDrowTime > 0)
		switch(_smoothOption){
		case FlightOption.moveSpeedOption.FastAndSlow:
			delta = ((_chackDrowTime - _currentTime)/_chackDrowTime*90);
			delta = Mathf.Sin(delta/180*Mathf.PI);
			break;
		case FlightOption.moveSpeedOption.SlowAndFast:
			delta = 0.5f + ((_chackDrowTime - _currentTime)/_chackDrowTime*90);
			delta = Mathf.Sin(delta/180*Mathf.PI);
			break;
		}
		
		_selfTF.localPosition += (_drowPoint*Time.deltaTime*delta);
		_copyVPosition = _selfTF.position;
		_copyVPosition.y = 0;
		_copyBounds.center = _copyVPosition;
		
		return _copyVPosition;
	}

	protected bool chackDrowTime(){
		_currentTime += Time.deltaTime;
		if(_nextObjectChack && _chackDrowTime <= _currentTime){
			_nextObjectChack = false;
			_currentTime -= _chackDrowTime;
			return true;
		}

		return false;
	}
}
