using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestoryOption : MonoBehaviour{
	
	public enum objectPosition{
		enemy,
		palyer,
		coin,
		powerUp
	}
	
	[System.Serializable]
	protected class destroyOption{
		
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
		public int _dieCreateCoinNumber = 1;
	}
	
	[SerializeField] protected destroyOption _destroyOption;

	protected Transform _selfTF;
	private Bounds _copyBounds;

	public Vector3 VPosition { get { return _selfTF.position; } }
	public Vector3 LPosition { get { return _selfTF.localPosition; } }
	public Bounds destroyBound { get { return _copyBounds; } }
	protected void SetDestoryCenter(Vector3 centerV) { _copyBounds.center = centerV; }
	protected void SetDestorySize(float widthSize, float heightSize){ _copyBounds.size = new Vector3(widthSize,0,heightSize); }
	
	public virtual int chackCollisionValue(BullectControl chackBullet){ return 0; }

	protected virtual void crushAndLiveControl(){}
	protected virtual void crushAndDieControl(bool PlayerChack, bool bladeDelete){}

	public bool destroyBullectChack(bool bladeDelete = false){
		if(_currentPenetrate < 0) return false;
		if(--_currentPenetrate > 0){
			crushAndLiveControl();
			return false;
		}
		
		crushAndDieControl(!destroyChack, bladeDelete);
		return true;
	}

	public int chackCrushPlayerValue(Vector3 _playerPS){
		if(returnMagnitude(_playerPS) <= _destroyOption._destroySize){
			return destroyBullectChack(false) ? 2 : 1;
		}
		return 0;
	}

	private int _currentPenetrate = 0;
	protected void setDrowStage(Transform bullect, BulletBase copyBullet, BulletBase.objectPosition obPoint, bool stopTimeLine, bool NonDestroyChack){
		_currentPenetrate = _destroyOption._penetrateCount;
		_destroyOption._collistionControl.setDrowStage(bullect, copyBullet, obPoint, stopTimeLine, NonDestroyChack);
	}
	
	public virtual bool destroyChack { get { return _destroyOption._destroyChack; } }
	public virtual float returnMagnitude(Vector3 basePositon){
		return (VPosition - basePositon).magnitude;
	}
	
	
	public virtual void coinChaseFlowAct(bool actChack){}
}