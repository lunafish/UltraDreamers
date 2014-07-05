using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletCreate : MonoBehaviour {

	[SerializeField] float _fistDelayDuration = 0.2f;
	[SerializeField] bool _createParent = false;
	[SerializeField] PlayerControl _playerControl;
	[SerializeField] bullectOption _bullectOption;
	[SerializeField] int _powerUpOptionChack = -1;
	[SerializeField] float _delayLoopShotTime = 0;
	[SerializeField] bool _closeAllDelete = false;
	[SerializeField] int _popLoopCount = 0;

	private float _currentSpeed = 0;
	protected Transform _selfTF = null;
	private int _copyDrowCount = 0;
	private bool _EnabledControl = true;
	private int _switchOption = 0;
	private int _popLoopSave = 0;
	//private bool _ChildChaseOption = false;

	protected virtual void Awake(){
		_selfTF = transform;
		_popLoopSave = _popLoopCount;
		_copyDrowCount = _bullectOption.drowShotCount;
		//_ChildChaseOption = _beamBaseImgControl != null;
	}

	public void resetBullet(bool enableV, bool bladeDelete){

		if(--_popLoopSave > 0) return;
		_popLoopSave = _popLoopCount;

		if(!enableV){
			if(bladeDelete && _controlBulletList.Count > 0)
				_playerControl.createCoinValue(_controlBulletList, Vector3.zero);
			else if(_closeAllDelete){
				_bulletListCount = _controlBulletList.Count;
				for(int i = 0; i < _bulletListCount; i++)
					_controlBulletList[i].stopBulletObject(notDestroyParent:false);
			}
		}

		if(!enableV) _controlBulletList.Clear();
		resetBullet(enableV);
	}

	void resetBullet(bool enableV = false){
		resetOptionValue();
		_bulletListCount = 0;
		_EnabledControl = this.enabled = enableV;
	}

	private int currentPowerOption = -1;
	void resetOptionValue(){

		_switchOption = 0;
		currentPowerOption = _powerUpOptionChack;
		_copyDrowCount = _bullectOption.drowShotCount;
		if(_fistDelayDuration <= 0) {
			_fristChack = true;
			_currentSpeed = _bullectOption.createSpeed;
		}
		else{
			_currentSpeed = 0;
			_fristChack = false;
		}
	}

//	private int _lastIndex = 0;
//	private int _deleteIndex = 0;
	//private Vector3 _copyEg = Vector3.zero;
	private BullectControl _copyBullet = null;
	protected int _bulletListCount = 0;
	protected List<BullectControl> _controlBulletList = new List<BullectControl>();
	public virtual void crushChildBullet(BullectControl copyBullet){
		_controlBulletList.Remove(copyBullet);
		_bulletListCount = _controlBulletList.Count;
		/*
		_lastIndex = _controlBulletList.IndexOf(copyBullet) -1;
		for(_deleteIndex = 0; _deleteIndex < _lastIndex; _deleteIndex++)
		{
			_controlBulletList[0].stopBulletObject(notDestroyParent:false);
			_controlBulletList.RemoveAt(0);
		}
		_controlBulletList.RemoveAt(0);*/
		/*
		if(parentChack != null) {
			parentChack.parent = _selfTF;
			parentChack.localEulerAngles = Vector3.zero;
		}
		Vector3 childCrushPosition = copyBullet.VPosition;
		_copyEg = _beamBaseImgControl.localScale;
		_copyEg.y = (childCrushPosition - _selfTF.position).magnitude*3.3f;
		_beamBaseImgControl.localScale = _copyEg;*/
	}

	private bool _fristChack = false;
	public float getXPosition { get { return _selfTF.position.x; } }
	void Update(){
		if(!_EnabledControl) return;

		switch(_switchOption){
		case 0:
			_currentSpeed += Time.deltaTime;
			if(!_fristChack) {
				if(_currentSpeed >= _fistDelayDuration){
					_currentSpeed = _bullectOption.createSpeed;
					_fristChack = true;
				}
				return;
			}

			if(_currentSpeed >= _bullectOption.createSpeed){
				_currentSpeed -= _bullectOption.createSpeed;

				_copyBullet = _bullectOption.CreateBullectValue(_selfTF, Vector3.zero, this, _createParent, --currentPowerOption == 0);
				if(/*_ChildChaseOption &&*/ _copyBullet != null){
					_controlBulletList.Add (_copyBullet);
					_bulletListCount = _controlBulletList.Count;
				}
	
				if(--_copyDrowCount == 0){
					resetBullet(); // code clear chack, dumy
					if(_delayLoopShotTime > 0){
						_switchOption = 1;
						_EnabledControl = this.enabled = true;
					}
				}
			}
			break;
		case 1:
			_currentSpeed += Time.deltaTime;
			if(_delayLoopShotTime > _currentSpeed) return;

			resetBullet(true);
			break;
		}
		

	}
}
