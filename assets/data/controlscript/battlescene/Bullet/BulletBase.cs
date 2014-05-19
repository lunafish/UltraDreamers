using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class bullectOption{
	
	[SerializeField] BullectControl bullectOriginal;
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
		return bullectOriginal.CreateBullectValue(this, baseTF, startPosition, parentCreate, createParent,dropPowerUp);
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
			return -1;
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
						Debug.Log(_currentLinerRandomCount);
						return 2;
					}

					Debug.Log("reset : "+_currentLinerRandomCount);
					_currentLinerRandomCount = _linerOption._randomLinerCount;
					return 1;
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

}
