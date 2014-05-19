using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BullectControl : BulletBase {
	/*
	[System.Serializable]
	private class rotateOption{
		public enum rotateDirection{
			left,
			right
		}

		public bool _rotateEnabled = false;
		public rotateDirection _direction = rotateDirection.left;
		public float _rotateSpeed = 2;
	}*/

	[System.Serializable]
	private class destroyOption{

		public enum crushEffectList{
			CrushEffect,
			BackDrowEffect,
			non
		}

		public collisionChack _collistionControl = null;
		public bool _destroyChack = false;
		public bool _NonDestroyChack = false;
		public bool _backDrowOption = false;
		public float _destroySize = 0.3f;
		public int _penetrateCount = 1;
		public crushEffectList _drowCrushEffectAni = crushEffectList.CrushEffect;
	}

	public enum idelAnimation{
		non,
		bladeEffect,
		CoinAni,
		EnemyBullet,
		homing_enemy,
		homing_player,
		Enemy_1,
		Enemy_2,
		Enemy_3,
		PowerUp_Bear
	}
	
	[SerializeField] objectPosition _obPoint = objectPosition.enemy;
	[SerializeField] idelAnimation _selectIdel = idelAnimation.non;
	[SerializeField] bool _stopTimeLine = false;
	[SerializeField] bool _RandomAngle = false;
	[SerializeField] bool _CrushRandomAngle = false;
	[SerializeField] bool _LookAtOption = false;
	[SerializeField] bool _notResetAngle = false;
	[SerializeField] List<GameObject> _enabledObjectList = new List<GameObject>();
	//[SerializeField] bool _LookAtCameraOption = false;

	[SerializeField] tk2dSpriteAnimator _crushEffect;
	[SerializeField] tk2dSprite _drowSprite;

	//[SerializeField] rotateOption _rotateOption;
	[SerializeField] destroyOption _destroyOption;
	
	private List<BullectControl> _bullectList = new List<BullectControl>();
	private int _nameObject = 0;

	private Vector3 _drowPoint = Vector3.zero;
	private Vector3 _backDrowPoint = Vector3.zero;
	private int _currentPenetrate = 0;

	private bool _nextObjectChack = false;
	private float _chackDrowTime;
	private BullectControl _originalData;
	private int _currentCount = 0;
	private bullectOption _bullectOp;

	private Transform _baseTFValue = null;

	private Bounds _copyBounds;
	private Vector3 _copyVPosition = Vector3.zero;

	private FlightOption.moveSpeedOption _smoothOption = FlightOption.moveSpeedOption.linear;
	private bool _curshEffectChack = false;
	private BulletCreate _createBullet = null;
	private bool _powerOption = false;
	
	public Vector3 VPosition { get { return _selfTF.position; } }
	public Bounds destroyBound { get { return _copyBounds; } }
	public bool destroyChack { get { return _destroyOption._destroyChack; } }
	public BullectControl CreateBullectValue(bullectOption bullectOp, Transform baseTF, Vector3 startPosition, BulletCreate parentCreate, bool createParent, bool dropPowerUp){

		BullectControl copyBullect = null;
		if(_bullectList.Count == 0) 
		{
			copyBullect = Instantiate(this) as BullectControl;
			copyBullect.name = gameObject.name + "_" + _nameObject++ ;
			copyBullect.ResetValue();
			copyBullect._originalData = this;
		}
		else{
			copyBullect = _bullectList[0];
			_bullectList.RemoveAt(0);
		}

		//destroyChack
		copyBullect.transform.position = baseTF != null ? baseTF.position : startPosition;
		copyBullect._bullectOp = bullectOp;
		copyBullect._createBullet = parentCreate;
		copyBullect._baseTFValue = baseTF;
		copyBullect.resetCurrent(createParent);

		copyBullect.resetBullet(true);
		copyBullect.nextBlockObject();
		copyBullect._powerOption = dropPowerUp;

		return copyBullect;
	}
	
	void addCollisitionControl(bool createParent){
		_fristChack = _curshEffectChack = false;
		_currentPenetrate = _destroyOption._penetrateCount;
		_destroyOption._collistionControl.setDrowStage(/*_childBullet? null : */_selfTF, this, _obPoint, _stopTimeLine, _destroyOption._NonDestroyChack);
		if(createParent && _baseTFValue != null) {
			_selfTF.parent = _baseTFValue; 
			if(!_notResetAngle){
				Vector3 loatEG = Vector3.zero;
				loatEG.z = _baseTFValue.localEulerAngles.y*-1;
				loatEG.x = 54;
				loatEG.y = 0;
				
				_selfTF.localEulerAngles = loatEG;
			}
		}
		//if(_otherTfChack == 1 && _LookAtCameraOption) settingLookAtTF();
	}

	void resetCurrent(bool createParent){
		if(_crushEffect != null){
			_crushEffect.Stop();
			
			switch(_selectIdel){
			case idelAnimation.non:
				break;
			default:
				_crushEffect.Play(_selectIdel.ToString());
				break;
			}
		}

		_currentCount = 0;
		addCollisitionControl(createParent);
		/*int maxCount = _bulletControlList.Count;
		for(int i = 0; i < maxCount; i++){
			_bulletControlList[i].addCollisitionControl(createParent);
			_bulletControlList[i].resetBullet(true);
		}*/
	}

	private FlightOption _selectFlight = null;
	void nextBlockObject(){

		_selectFlight = _bullectOp.flightType(_currentCount++);
		if(_selectFlight == null) {
			_nextObjectChack = false;
			return;
		}

		Vector3 endPosition = Vector3.zero;
		if(_RandomAngle){
			endPosition = _selfTF.eulerAngles;
			endPosition.z = Random.Range(0, 360);
			_selfTF.eulerAngles = endPosition;
		}

		_selectFlight.resetValue(_nonresetValue);

		_drowPoint = Vector3.zero;
		_chackDrowTime = _selectFlight.drowTIme;
		_smoothOption = _selectFlight.smoothOption;
		_currentTime = 0;

		if(_currentCount < _bullectOp.maxFlightTypeCount && _chackDrowTime > 0) _nextObjectChack = true;
		else _nextObjectChack = false;

		this.enabled = true;
		controlMoveOption();
	}

	Vector3 getDirectionValue(FlightOption.LinerTypeList linerType){
		Vector3 endPosition = Vector3.zero;
		switch(linerType){
		case FlightOption.LinerTypeList.forward:
			endPosition = _baseTFValue.forward;
			break;
		case FlightOption.LinerTypeList.backward:
			endPosition = _baseTFValue.forward * -1; 
			break;
		case FlightOption.LinerTypeList.left:
			endPosition = _baseTFValue.right;
			break;
		case FlightOption.LinerTypeList.right: 
			endPosition = _baseTFValue.right * -1; // 플레이어 기준
			break;
		case FlightOption.LinerTypeList.left_forward:
			endPosition = _baseTFValue.forward + _baseTFValue.right;
			break;
		case FlightOption.LinerTypeList.left_backward:
			endPosition = (_baseTFValue.forward * -1) + _baseTFValue.right;
			break;
		case FlightOption.LinerTypeList.Right_forward:
			endPosition = _baseTFValue.forward + (_baseTFValue.right * -1);
			break;
		case FlightOption.LinerTypeList.Right_backward:
			endPosition = (_baseTFValue.forward * -1) + (_baseTFValue.right * -1);
			break;
		}

		return endPosition;
	}

	Vector3 getEulerAnglesValue(){
		Vector3 EulerAngles = _baseTFValue.eulerAngles;
	
		EulerAngles.x = EulerAngles.z = 0;
		switch(_selectFlight.linerType){
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
	
	private Vector3 _endPosition = Vector3.zero;
	private Vector3 _chaseEndPosition = Vector3.zero;
	private List<int> _randomLinerControl = new List<int>();
	private bool _nonresetValue = false;
	void controlMoveOption(){
		_chaseEndPosition = _endPosition = Vector3.zero;
		_bullectModeControl = 0;

		_copyVPosition = _selfTF.position;
		_copyVPosition.y = 0;
		_copyBounds.center = _copyVPosition;

		switch(_selectFlight.flightType){
		case FlightOption.FlightTypeList.chaser:
			_bullectModeControl = 2;

			switch(_obPoint){
			case BullectControl.objectPosition.palyer:
				BullectControl copyBullet = _destroyOption._collistionControl.enemyChasePosition(_copyVPosition);
				//_endPosition
				if(copyBullet == null){
					_bullectModeControl = 0;
					_endPosition = getDirectionValue(FlightOption.LinerTypeList.forward);
					if(_endPosition.z < 0) _endPosition.z *= -1;
					else if(_endPosition.z == 0) _endPosition.z = 0.1f;
				}else _endPosition = copyBullet._selfTF.localPosition;
				break;
			default:
				_endPosition = _selectFlight.getChasePosition;
				if(_endPosition.Equals(Vector3.zero)) _endPosition = _destroyOption._collistionControl.PlayerPosition;

				break;
			}

			if(_LookAtOption && _otherTfChack == 1) {
				_selfTF.LookAt(_endPosition);
				settingLookAtTF();
			}
			_endPosition -= _selfTF.localPosition;
			_chaseEndPosition = _endPosition;
			break;
		case FlightOption.FlightTypeList.liner:
			if(_baseTFValue == null) _baseTFValue = _selfTF;

			switch(_selectFlight.randomLinerChack){
			case 0:
				_endPosition = getDirectionValue(_selectFlight.linerType);
				break;
			case 1:
				nextBlockObject();
				return;
			case 2:
				if(_randomLinerControl.Count == 0){
					for(int i = 0; i < 8; i++)
						_randomLinerControl.Add(i);
				}
				
				_currentCount--;
				_nonresetValue = true;
				FlightOption.LinerTypeList linerType = FlightOption.LinerTypeList.forward;
				
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
				_endPosition = getDirectionValue(linerType);
				break;
			}

			if(_LookAtOption && _otherTfChack == 1){
				//_selfTF.LookAt(_endPosition);
				_selfTF.localEulerAngles = getEulerAnglesValue();
				settingLookAtTF();
			}

			break;
		case FlightOption.FlightTypeList.delay:
			_bullectModeControl = -1;
			return;
		case FlightOption.FlightTypeList.delete:
			stopBulletObject();
			return;
		case FlightOption.FlightTypeList.loop:
			/*_bullectModeControl = -1;
			_currentCount = 0;
			_nextObjectChack = true;*/
			_currentCount = _selectFlight.loopStartIndex;
			nextBlockObject();
			return;
		case FlightOption.FlightTypeList.rotate:
			if(_selectFlight.MoveSpeed == 0 && _selectFlight.drowTIme == 0){
				_selfTF.localEulerAngles = getEulerAnglesValue();
				//settingLookAtTF();
				nextBlockObject();
				return;
			}

			return;
		}
		
		_drowPoint = _endPosition.normalized * _selectFlight.MoveSpeed;
		_drowPoint.y = 0;
		_backDrowPoint = _endPosition.normalized/2;
	}

	//private bool _childBullet = false;
	private Transform _selfTF;
	private int _originalImgIndex = 0;
	private Transform _drowSpriteTF;
	private int _otherTfChack = 0;
	private Vector3 _startEulerAngles = Vector3.zero;
	private Vector3 _startSpriteEulerAngles = Vector3.zero;
	private Vector3 _startSpriteScale = Vector3.zero;
	private List<BulletCreate> _bulletCreateList = new List<BulletCreate>();
	
	private string _selectEffectAni = "";
	//private List <BullectControl> _bulletControlList = new List<BullectControl>();

	void ResetValue(){
		childResetValue();
		//findBulletControl(_selfTF, false);

		/*BullectControl[] findBulletControl = _selfTF.GetComponentsInChildren<BullectControl>();
		if(findBulletControl != null){
			int maxCount = findBulletControl.Length;
			if(maxCount > 0){
				_bulletControlList.AddRange(findBulletControl);
				for(int i = 0; i < maxCount; i++){
					findBulletControl[i]._childBullet = true;
					findBulletControl[i].childResetValue();
				}
			}
		}*/
		//_childBullet = false;

		BulletCreate[] findBulletCreate = _selfTF.GetComponentsInChildren<BulletCreate>();
		if(findBulletCreate != null && findBulletCreate.Length > 0) _bulletCreateList.AddRange(findBulletCreate);
	}

	void childResetValue(){
		this.enabled = false;
		_selfTF = transform;
		_startEulerAngles = _selfTF.localEulerAngles;
		switch(_destroyOption._drowCrushEffectAni){
		case destroyOption.crushEffectList.non:
			_selectEffectAni = "";
			break;
		default:
			_selectEffectAni = _destroyOption._drowCrushEffectAni.ToString();
			break;
		}
		
		_otherTfChack = 0;
		if(_drowSprite != null){
			_drowSpriteTF = _drowSprite.transform;
			_startSpriteEulerAngles = _drowSpriteTF.localEulerAngles;
			_originalImgIndex = _drowSprite.spriteId;
			_startSpriteScale = _drowSprite.scale;
			
			if(!_drowSpriteTF.Equals(_selfTF)) _otherTfChack = 1;
		}

		_copyBounds.size = new Vector3(_destroyOption._destroySize,0,_destroyOption._destroySize);
	}

	public void allStopBulletValue(bool enabledControl){
		this.enabled = enabledControl;
		if(_crushEffect != null) _crushEffect.enabled = enabledControl;

		int maxCount = _bulletCreateList.Count;
		for(int i = 0; i < maxCount; i++) _bulletCreateList[i].enabled = enabledControl;
	}

	void resetBullet(bool resetChack, bool bladeDelete = false){
		int maxCount = _bulletCreateList.Count;
		if(maxCount > 0) for(int i = 0; i < maxCount; i++) _bulletCreateList[i].resetBullet(resetChack, bladeDelete);
		else if(!resetChack && bladeDelete) _destroyOption._collistionControl.createCoinValue(_selfTF.position);

		maxCount = _enabledObjectList.Count;
		for(int i = 0; i < maxCount; i++)
			_enabledObjectList[i].SetActive(resetChack);
	}

	private float _currentTime = 0;
	private float zData = 0;
	private float xData = 0;
	private Vector3 position;
	private int _bullectModeControl = 0;
	private float _backMoveTime = 0;
	void Update(){
	
		switch(_bullectModeControl){
		case 0: // move to position
			if(!moveTFControl()) return;
			break;
		case 1: // destroy bullet Object
			if(!_crushEffect.IsPlaying(_selectEffectAni)){
				/*if(!_childBullet)*/ resetDrowSprite();
				stopBulletObject(true);
			}
			return;
		case 2: // chase to position
			if(_selectFlight.chackChaseCount) controlMoveOption();
			if(!moveTFControl()) return;

			if(_nextObjectChack){
				float current = (_chaseEndPosition - _selfTF.localPosition).magnitude;
				if(current < 0.005) _currentTime = _chackDrowTime;
			}

			break;
		case 3: // collider chack backdrow option
			_backMoveTime += Time.deltaTime;
			moveTFControl();
			if(_backMoveTime > 0.1f) controlMoveOption();
			return;
		}

		_currentTime += Time.deltaTime;
		if(_nextObjectChack && _chackDrowTime <= _currentTime){
			_nextObjectChack = false;
			_chackDrowTime -= _currentTime;
			nextBlockObject();
		}

	}

	void resetDrowSprite(){
		if(_crushEffect != null) _crushEffect.Stop();
		if(_drowSprite != null){
			_drowSprite.scale = Vector3.one;
			_drowSprite.SetSprite(_originalImgIndex);
		}
	}

	private bool _fristChack = false;
	bool moveTFControl(){

		float delta = 1;
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
		position = _selfTF.position;
		_copyVPosition = position;
		_copyVPosition.y = 0;
		_copyBounds.center = _copyVPosition;

		zData = position.z;
		xData = position.x;
		if(/*!_childBullet && */(zData < 0 || 9 < zData || xData < -2.5f || 2.5f < xData)){
			if(_fristChack){
				stopBulletObject();
				return false;
			}

			_fristChack = true;
			return true;
		}

		return true;
	}
	
	public bool stopBulletObject(bool notAddStorage = false, bool notDestroyParent = true/*, bool deleteBullet = false*/){
		if(!notDestroyParent && destroyChack/* && !deleteBullet*/) return false;

		_bullectModeControl = -1;
		_powerOption = _fristChack = this.enabled = false;

		if(!notAddStorage && notDestroyParent) destroyReturnchack_ChildBullet();
		_destroyOption._collistionControl.addStroageData(/*!_childBullet ?*/ _selfTF /*: null*/, notAddStorage ? null : this, _stopTimeLine);
		/*if(!_childBullet) */_originalData._bullectList.Add(this);

		resetRotateTF();
		/*if(notAddStorage){
		if(!_childBullet) {
			int maxCount = _bulletControlList.Count;
			for(int i = 0; i < maxCount; i++) _bulletControlList[i].resetDrowSprite();
		}}*/

		return true;
	}

	void settingLookAtTF(){
		if(_otherTfChack == 1)
		{
			Vector3 loatEG = _selfTF.localEulerAngles;
			
			loatEG.z = loatEG.y*-1;
			loatEG.x = 54;
			loatEG.y = 0;
			
			_drowSpriteTF.eulerAngles = loatEG;
			
			loatEG = _startSpriteScale;
			loatEG.y *= -1;
			_drowSprite.scale = loatEG;
		}
	}

	void resetRotateTF(){

		_selfTF.localEulerAngles = _startEulerAngles;
		if(_drowSpriteTF != null) _drowSpriteTF.localEulerAngles = _startSpriteEulerAngles;
		if(_drowSprite != null) _drowSprite.scale = _startSpriteScale;
	}

	bool destroyBullectChack(Vector3 backDropwPoint, bool PlayerChack = false, bool bladeDelete = false){
		if(_currentPenetrate < 0) return false;
		if(--_currentPenetrate > 0){
			if(_destroyOption._backDrowOption){
				_backMoveTime = 0;
				_drowPoint *= -1.2f;
				_bullectModeControl = 3;
				//_selfTF.position += backDropwPoint;
			}
			//if(_selectFlight.TrackAfterACrash) controlMoveOption();

			return false;
		}

		if(_powerOption) _destroyOption._collistionControl.createPowerUpIter(VPosition);
		if(_crushEffect != null && !PlayerChack) crushAniControl(bladeDelete);
		else stopBulletObject();
		return true;
	}

	void crushAniControl(bool bladeDelete){
		if(_curshEffectChack) return;

		/*if(!_childBullet)*/{
			resetRotateTF();

			if(_CrushRandomAngle){
				Vector3 copyAngle = _selfTF.eulerAngles;
				copyAngle.z = Random.Range(0, 360);
				_selfTF.eulerAngles = copyAngle;
			}

			_bullectModeControl = 1;
		}//else _bullectModeControl = -1;
	
		resetBullet(false, bladeDelete);
		_curshEffectChack = true;

		if(_crushEffect != null && !string.IsNullOrEmpty(_selectEffectAni)) {
			if(_drowSprite != null) _drowSprite.scale = Vector3.one*2;
			_crushEffect.Play(_selectEffectAni);
		}

		destroyReturnchack_ChildBullet();
		_destroyOption._collistionControl.addStroageData(null, this, false);
	}

	void destroyReturnchack_ChildBullet(){
		if(_createBullet != null) _createBullet.crushChildBullet(this);
	}

	private float _copyMagnitude = 0;
	private int _returnBulletValue = 0;
	public int chackCollisionValue(BullectControl chackBullet){
		_copyMagnitude = returnMagnitude(chackBullet._copyVPosition);
		if(_copyMagnitude < _destroyOption._destroySize || _copyMagnitude < chackBullet._destroyOption._destroySize){
			_returnBulletValue = 0;
			if(destroyBullectChack(chackBullet._backDrowPoint)) _returnBulletValue = 1;
			if(chackBullet.destroyBullectChack(_backDrowPoint)) _returnBulletValue += 2;
			return _returnBulletValue;
		}

		return 0;
	}

	public float returnMagnitude(Vector3 basePositon){
		return (_copyVPosition - basePositon).magnitude;
	}

	public bool destroyBullectChack(bool bladeDelete){
		return destroyBullectChack(_backDrowPoint, !destroyChack, bladeDelete);
	}
	
	public int chackCrushPlayerValue(Vector3 _playerPS){
		if(returnMagnitude(_playerPS) <= _destroyOption._destroySize){
			return destroyBullectChack(false) ? 2 : 1;
		}
		return 0;
	}

	public void coinChaseFlowAct(bool actChack){
		if(actChack && _currentCount == 2){
			nextBlockObject();
		}
	}
}
