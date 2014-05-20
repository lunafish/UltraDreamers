using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

	[SerializeField] BlockControlScript _blockScript;
	[SerializeField] bullectOption _bladeEffect;
	[SerializeField] bullectOption _coinEffect;
	[SerializeField] bullectOption _PowerUpItem;
	[SerializeField] NumberControl _coinNumberCount;
	[SerializeField] Player_Hp _hpControlOption;
	[SerializeField] Tk2dFadeInOutControl _brokenGralss;
	//[SerializeField] collisionChack _collisionChack;

	private int _PowerControl = 0;
	private BulletCreate[] _createBullet;

	private Transform _selfTF;
	//private GameObject _selfObject;

	private tk2dSprite _drowSprite;
	private tk2dSpriteAnimator _drowSpriteAni;
	private tk2dSprite _bladeSprite;
	private tk2dSpriteAnimator _BladeAni;
	private int _drowSwitchIndex = 0;
	private GameObject _bulletTransformObject;

	private GameObject _barrierObject;
	private GameObject _barrierChild;
	private Transform _barrierSmall;
	private tk2dSpriteAnimator _barrierSmallAni;

	private Bounds _copyBounds;
	private Vector3 _bladePosition = Vector3.zero;
	private Vector3 _createBaldEffect = Vector3.zero;
	//private Vector3 _startPosition = Vector3.zero;

	private Animator _wing_Normal_Left = null;
	private Animator _wing_Normal_RIght = null;
	private Transform _wingNormalTF = null;
	private Vector3 _wingNormalOriPosition = Vector3.zero;
	
	private GameObject _actTransformObject;
	private Animator _wing_TF_Left = null;
	private Animator _wing_TF_RIght = null;
	private Transform _wingTF_Transform = null;
	private Vector3 _wingTF_OriPosition = Vector3.zero;

	private GameObject _PowerUpEffect;
	private BulletCreate_Laser[] _laserComponent;
	void Awake(){
		_selfTF = transform;
		//_selfObject = _selfTF.gameObject;
		_createBaldEffect.y = _selfTF.position.y;
		//_startPosition = _createBaldEffect = _selfTF.position;

		Transform spriteTF =  _selfTF.FindChild("FlightUnit");
		_drowSprite = spriteTF.GetComponent("tk2dSprite") as tk2dSprite;
		_drowSpriteAni = spriteTF.GetComponent("tk2dSpriteAnimator") as tk2dSpriteAnimator;

		spriteTF = _selfTF.FindChild("PowerUpEffect");
		spriteTF.localScale = Vector3.zero;
		_PowerUpEffect = spriteTF.gameObject;

		spriteTF = _selfTF.FindChild("BladeChack");
		_BladeAni = spriteTF.GetComponent("tk2dSpriteAnimator") as tk2dSpriteAnimator;
		_bladeSprite = spriteTF.GetComponent("tk2dSprite") as tk2dSprite;
		_bladeSprite.color = Color.clear;

		Vector3 drowScale = _bladeSprite.scale;
		drowScale.x *= -1;
		_bladeSprite.scale = drowScale;

		BoxCollider colliderSize = (BoxCollider)(spriteTF.collider);

		_copyBounds.center = colliderSize.center;
		_copyBounds.size = colliderSize.size;
		_bladePosition = _copyBounds.center;

		float dymyPositionY =  _bladePosition.y;
		_bladePosition.z = dymyPositionY + spriteTF.localPosition.z;
		_bladePosition.y = 0;

		Vector3 BoundSize = _copyBounds.size;
		BoundSize.z = BoundSize.y;
		BoundSize.y = 0;
		_copyBounds.size = BoundSize;

		_wingTF_Transform = _selfTF.FindChild("Transform");
		_wing_TF_Left = _wingTF_Transform.FindChild("TF_Left_Wing").GetComponent("Animator") as Animator;
		_wing_TF_RIght = _wingTF_Transform.FindChild("rightWingPosition").FindChild("TF_Left_Wing").GetComponent("Animator") as Animator;
		_actTransformObject = _wingTF_Transform.gameObject;
		_wingTF_OriPosition = _wingTF_Transform.localPosition;

		_actTransformObject.SetActive(true);
		ObjectRotateControl[] rotateObject = _wingTF_Transform.transform.GetComponentsInChildren<ObjectRotateControl>();

		bool reverseControl = false;
		foreach(ObjectRotateControl rotateOption in rotateObject){
			rotateOption._reverseChack = reverseControl;
			rotateOption.rotateToPosition(new Vector3(0,0, 360), 4);
		}

		_actTransformObject.SetActive(false);

		spriteTF = _selfTF.FindChild("Transform_Bullet");
		_bulletTransformObject = spriteTF.gameObject;

		_bulletTransformObject.SetActive(true);
		_laserComponent = spriteTF.GetComponentsInChildren<BulletCreate_Laser>();
		_bulletTransformObject.SetActive(false);

		spriteTF = _selfTF.FindChild("Barrier");
		_barrierObject = spriteTF.gameObject;
		_barrierObject.SetActive(false);
		_barrierChild = spriteTF.FindChild("Barrier").gameObject;
		_barrierChild.transform.localScale = Vector3.zero;
		//_barrierAni = _barrierChild.transform.GetComponent("tk2dSpriteAnimator") as tk2dSpriteAnimator;

		_barrierSmall = spriteTF.FindChild("Barrier_small");
		_barrierSmallAni = _barrierSmall.GetComponent("tk2dSpriteAnimator") as tk2dSpriteAnimator;

		_wingNormalTF = _selfTF.FindChild("Wing");
		_wing_Normal_Left = _wingNormalTF.FindChild("NL_LeftWing").GetComponent("Animator") as Animator;
		_wing_Normal_RIght = _wingNormalTF.FindChild("NL_RighttWing").GetComponent("Animator") as Animator;
		_wingNormalOriPosition = _wingNormalTF.localPosition;

		_PowerControl = 1;
		_createBullet = _selfTF.GetComponentsInChildren<BulletCreate>();
		bulletEnabledControl(false);

		_drowSwitchIndex = 0;
		_blockScript.enabled = false;
		this.enabled = true;
	}

	void bulletEnabledControl(bool enabledControl){
		int maxCount = 0;
		if(enabledControl) {
			maxCount = _PowerControl;
			if(maxCount >= _createBullet.Length) maxCount = _createBullet.Length;
		}else maxCount = _createBullet.Length;

		for(int i = 0; i < maxCount; i++){
			_createBullet[i].resetBullet(enabledControl, false);
		}

		if(enabledControl && maxCount >= 2 )_createBullet[0].resetBullet(false, false);
	}

	IEnumerator nomalWingAppearChack(){

		while((_wing_Normal_Left.GetCurrentAnimatorStateInfo(0).IsName("NL_LeftWing") || 
		       _wing_Normal_Left.GetCurrentAnimatorStateInfo(0).IsName("NL_LeftWing_Out")))
		      yield return null;

		bulletEnabledControl(true);
		_blockScript.enabled = true;
		_blockScript._stopWingObjectMoveChack = false;
		if(_viewPlayerHPObject){
			_viewPlayerHPObject = false;
			_hpControlOption.ViewPlayerHpObject(_dieCount);
		}
	}

	private bool _viewPlayerHPObject = true;
	void Start(){
		_coinNumberCount.ViewNumberCount(0, true);
		StartCoroutine("nomalWingAppearChack");
		this.enabled = false;
	}

	private tk2dSpriteAnimator _dieBoombAni = null;
	public void playerCollisionChack(Vector3 drowPosition){
		//if(_drowSwitchIndex != 0) return;

		collisionChack._allstopBulletCreate = true;
		_blockScript._enableTouchControl = false;
		_blockScript.rotatePlayerWingObject(4 , 0.1f);
		bulletEnabledControl(false);
		//_collisionChack.allStopControlBullet(false);

		_drowChackTime = 0;

		_blockScript.stopSpriteAni();
		_BladeAni.Stop();
		_bladeSprite.color = Color.clear;
		this.enabled = true;
		_drowSpriteAni.Stop();
		_drowSpriteAni.Play("Character_1_underAttack");
		_hpControlOption.minusHpValueChack();
		_brokenGralss.startFadeInAndOut(0.05f, 0.5f, Tk2dFadeInOutControl.fadeControlAni.hide, Tk2dFadeInOutControl.fadeControlAni.view, 0.5f, 1);

		if(_dieCount > 1){
			_dieCount--;
			_drowSwitchIndex = 6;
			activeBarrierObject();

			Vector3 copyScale = Vector3.one;
			_barrierSmall.position = drowPosition;
			if(_barrierSmall.localPosition.x > 0) copyScale.x *= -1;
			_barrierSmall.localScale = copyScale;
		}
		else {
			_wing_Normal_Left.SetTrigger("Wing_DisAppear");
			_wing_Normal_RIght.SetTrigger("Wing_DisAppear");

			_selfTF.FindChild("DieBoomb").gameObject.SetActive(true);
			_dieBoombAni = _selfTF.FindChild("DieBoomb").FindChild("Center2").GetComponent("tk2dSpriteAnimator") as tk2dSpriteAnimator;
			_drowSwitchIndex = 8;
		}
	}

	private bool _actBarrierChack = false;
	void activeBarrierObject(){
		_actBarrierChack = true;
		_barrierObject.SetActive(true);
		
		_barrierSmallAni.Play("barrierAni_small");
		//_barrierAni.Play("barrierAni");
		ObjectScaleControl.scaleToObject(_barrierChild, 0.1f, Vector3.one, endObject:gameObject, EndFunction:"endActiveBarrier", delayValue:0.1f);
	}
	
	void removeBarrierObject(){
		ObjectScaleControl.scaleToObject(_barrierChild, 0.1f, Vector3.zero, endObject:gameObject, EndFunction:"removeActBarrier");
	}
	
	void endActiveBarrier(){
		_actBarrierChack = false;
		if(_drowSwitchIndex == 7) this.enabled = true;
	}

	public void PowerUpItemChack(){
		_PowerControl++;

		float drowTime = 0.3f;
		_PowerUpEffect.transform.eulerAngles = new Vector3(53,0,0);
		ObjectRotateControl.rotateToObject(_PowerUpEffect, drowTime*2, new Vector3(53,0,-180));
		ObjectScaleControl.scaleToObject(_PowerUpEffect, drowTime, Vector3.one);
		ObjectScaleControl.scaleToObject(_PowerUpEffect, drowTime, Vector3.zero, delayValue:drowTime);

		if(_PowerControl >= _createBullet.Length) {
			_currentCoinValue += 5;
			_coinNumberCount.ViewNumberCount(_currentCoinValue, 0.3f);
		}else bulletEnabledControl(true);
	}

	public enum enemyCrushType{
		bullet,
		bullet_Die,
		blade,
		blade_Die
	}

	public void setScorePlayerBound(enemyCrushType indexControl){
		switch(indexControl){
		case enemyCrushType.bullet:
			_currentCoinValue += 1;
			break;
		case enemyCrushType.bullet_Die:
			_currentCoinValue += 5;
			break;
		case enemyCrushType.blade:
			_currentCoinValue += 2;
			break;
		case enemyCrushType.blade_Die:
			_currentCoinValue += 7;
			break;
		default:
			return;
		}
	
		_coinNumberCount.ViewNumberCount(_currentCoinValue, 0.3f);
	}

	private int _currentCoinValue = 0;
	public void coindCollisionChack(){
		_currentCoinValue += 10;
		_coinNumberCount.ViewNumberCount(_currentCoinValue, 0.3f);
	}

	private Vector3 _copyVPosition = Vector3.zero;
	public Vector3 VPosition { get { 

			_copyVPosition = _selfTF.position;
			_copyVPosition.y = 0;

			_copyBounds.center = _copyVPosition + _bladePosition;
			return _copyVPosition; 
		} 
	}

	public bool colliderBoxChack(Bounds destroyBound){
		switch(_drowSwitchIndex){
		case 0:
			if(chackColliderSword(destroyBound)){
				aniPlayAttackBladAttack();
				return true;
			}
			break;
		case 1:
			if(chackColliderSword(destroyBound)) return true;
			break;
		}

		return false;
	}

	private int _boundXChackIndex = 0;
	private int _boundYChackIndex = 0;
	bool chackColliderSword(Bounds destroyBound){
		
		//if (_copyBounds.Contains(destroyBound.min) || _copyBounds.Contains(destroyBound.max)) return false;
		//if (destroyBound.max.x < _copyBounds.min.x || _copyBounds.max.x < destroyBound.min.x) return false;
		//if (destroyBound.max.z < _copyBounds.min.z || _copyBounds.max.z < destroyBound.min.z) return false;
		/*
		Debug.Log(destroyBound.max.x + " < " + _copyBounds.min.x + " " + (destroyBound.max.x < _copyBounds.min.x) + " .. " + 
		          _copyBounds.max.x + " < " + destroyBound.min.x + " " + (_copyBounds.max.x < destroyBound.min.x) + "\n" +
		          destroyBound.max.z + " < " + _copyBounds.min.z + " " + (destroyBound.max.z < _copyBounds.min.z) + " .. " + 
		          _copyBounds.max.z + " < " + destroyBound.min.z + " " + (_copyBounds.max.z < destroyBound.min.z));*/
		
		/*if(Random.Range(0,2) == 0) _createBaldEffect.x = (destroyBound.max.x + _copyBounds.min.x)/2;
		else _createBaldEffect.x = (destroyBound.min.x + _copyBounds.max.x)/2;

		_createBaldEffect.z = (destroyBound.max.z + _copyBounds.min.z)*2/5;
		_bladeEffect.CreateBullectValue(null, _createBaldEffect, null, false);

		return true;*/
		
		_boundYChackIndex = _boundXChackIndex = 0;
		if (destroyBound.max.x < _copyBounds.min.x) return false;
		else if (_copyBounds.min.x < destroyBound.max.x) _boundXChackIndex = 1;
		
		if (_copyBounds.max.x < destroyBound.min.x) return false;
		else if (destroyBound.min.x < _copyBounds.max.x) _boundXChackIndex += 2;
		
		if (destroyBound.max.z < _copyBounds.min.z) return false;
		else if (_copyBounds.min.z < destroyBound.max.z) _boundYChackIndex = 1;
		
		if (_copyBounds.max.z < destroyBound.min.z) return false;
		else if (destroyBound.min.z < _copyBounds.max.z) _boundYChackIndex += 2;

		switch(_drowSwitchIndex){
		case 1: case 2:
			return true;
		}
		
		switch(_boundXChackIndex){
		case 1:
			_createBaldEffect.x = (destroyBound.min.x + _copyBounds.max.x)/2;
			break;
		case 2:
			_createBaldEffect.x = (destroyBound.max.x + _copyBounds.min.x)/2;
			break;
		case 3:
			_createBaldEffect.x = _copyBounds.center.x;
			break;
		default:
			return false;
		}

		switch(_boundYChackIndex){
		case 1:
			_createBaldEffect.z = (destroyBound.max.z + _copyBounds.min.z)/2;
			break;
		case 2:
			_createBaldEffect.z = (destroyBound.min.z + _copyBounds.max.z)/2;
			break;
		case 3:
			_createBaldEffect.z = destroyBound.center.z;
			break;
		default:
			return false;
		}

		//_createBaldEffect.z = destroyBound.center.z;
		//_createBaldEffect.z = _selfTF.position.z + _copyBounds.min.z;
		_bladeEffect.CreateBullectValue(null, _createBaldEffect, null, false, false);
		return true;
	}

	public void createPowerUpItem(Vector3 _drowPosition){
		if(_PowerControl >= _createBullet.Length) return;
		_PowerUpItem.CreateBullectValue(null, _drowPosition , null, false, false);
	}
	
	private int controlBulletListMaxCount;
	public void createCoinValue(List<BullectControl> controlBulletList, Vector3 _drowPosition){
		if(controlBulletList != null){
			controlBulletListMaxCount = controlBulletList.Count;
			for(int i = 0; i < controlBulletListMaxCount; i++) {
				if(controlBulletList[i].stopBulletObject(notDestroyParent:false))
					_coinEffect.CreateBullectValue(null, controlBulletList[i].VPosition , null, false, false);
			}
		}else _coinEffect.CreateBullectValue(null, _drowPosition , null, false, false);
	}

	void aniPlayAttackBladAttack(){
		switch(_drowSwitchIndex){
		case 1: case 2:
			return;
		}

		_blockScript.stopSpriteAni();
		_drowSprite.SetSprite("Unit_Sword");
		_BladeAni.Play("bladeAttack");

		Vector3 drowScale = _bladeSprite.scale;
		drowScale.x *= -1;
		_bladeSprite.scale = drowScale;
		_bladeSprite.color = Color.white;

		_drowSprite.scale = Vector3.one;
		bulletEnabledControl(false);
		_drowSwitchIndex = 1;
		_drowChackTime = 0;

		this.enabled = true;
	}

	private int _dieCount = 5;
	private float _drowChackTime = 0;
	void Update(){
		switch(_drowSwitchIndex){
		case 1:
			if(!_BladeAni.Playing){
				_bladeSprite.color = Color.clear;
				_drowSwitchIndex = 2;
				_blockScript.ActSpriteAni();
			}else _selfTF.localPosition -= _selfTF.forward *Time.deltaTime;
			break;
		case 2:
			_drowChackTime += Time.deltaTime;
			_selfTF.localPosition += _selfTF.forward *Time.deltaTime * 2f;
			if(_drowChackTime >= 0.1f){
				_drowSwitchIndex = 3;
			}
			break;
		case 3: // bullet create restart
			_drowSwitchIndex = 0;
			this.enabled = false;
			bulletEnabledControl(true);
			break;
		case 4: // transform wing appear end chack
			if(_wing_TF_Left.GetCurrentAnimatorStateInfo(0).IsName("TF_LeftWIng") || 
			   _wing_TF_Left.GetCurrentAnimatorStateInfo(0).IsName("TF_LeftWing_Out")){
				return;
			}

			_drowChackTime = 0;
			_drowSwitchIndex = 5;
			_bulletTransformObject.SetActive(true);

			break;
		case 5: // transfor wing attack end chack
			_drowChackTime += Time.deltaTime;
			if(_drowChackTime >= 5){
				_drowChackTime = 0;
				_drowSwitchIndex = 3;
				_blockScript.ActSpriteAni();

				int maxCount = _laserComponent.Length;
				for(int i = 0; i < maxCount; i++){
					_laserComponent[i].allChildRemove();
				}

				_bulletTransformObject.SetActive(false);
				_wingTF_Transform.parent = _selfTF.parent;
				_wing_TF_Left.SetTrigger("Wing_DisAppear");
				_wing_TF_RIght.SetTrigger("Wing_DisAppear");

				_wingNormalTF.parent = _selfTF;
				_wingNormalTF.localPosition = _wingNormalOriPosition;
				_wing_Normal_Left.SetTrigger("wing_Appear");
				_wing_Normal_RIght.SetTrigger("wing_Appear");

				collisionChack._allstopBulletCreate = false;
				bulletEnabledControl(true);
				StartCoroutine("nomalWingAppearChack");
			}
			break;
		case 6: // crush Player

			if(_drowSpriteAni.IsPlaying("Character_1_underAttack")) return;
		
			_drowChackTime = 0;
			_blockScript.ActSpriteAni();
			_blockScript._enableTouchControl = true;
			bulletEnabledControl(true);

			_drowSwitchIndex = 7;
			if(_actBarrierChack) this.enabled = false;

			break;
		case 7:
			_drowChackTime += Time.deltaTime;
			if(_drowChackTime < 0.6f) return;
			
			removeBarrierObject();
			_drowSwitchIndex = 0;
			this.enabled = false;
			break;
		case 8: // die player
			if(_dieBoombAni.Playing) return;

			_drowSpriteAni.Play("Character_die");
			_drowSwitchIndex = 9;
			break;
		case 9: // end scene
			if(_drowSpriteAni.IsPlaying("Character_die")) return;
			//Application.LoadLevelAsync("startScene"); // pro vestion
			Application.LoadLevel("startScene");
			StopAllCoroutines();
			break;
		}

		/*
		switch(_wingDisAppearChack){
		case 1:
			_wingDisAppearChack++;
			break;
		case 2:
			if(!_wing_TF_Left.GetCurrentAnimatorStateInfo(0).IsName("TF_LeftWing_Out")){
				_wingDisAppearChack = 0;
				_wing_TF_Left.ResetTrigger("Wing_DisAppear");
				_wing_TF_RIght.ResetTrigger("Wing_DisAppear");
				_wing_TF_Left.enabled = _wing_TF_RIght.enabled = false;
				_actTransformObject.SetActive(false);

				if(_drowSwitchIndex == 0) this.enabled = false;
			}
			break;
		}*/
	}

	void gameResumeOption(){
		_blockScript.ActSpriteAni();
		_blockScript._enableTouchControl = true;
		bulletEnabledControl(true);

		_drowSwitchIndex = 0;
		this.enabled = false;
		collisionChack._allstopBulletCreate = false;
	}

	void removeActBarrier(){
		collisionChack._allstopBulletCreate = false;
		_barrierObject.SetActive(false);
	}

	void ActTransformUnit(){
		if(_drowSwitchIndex == 0 || _drowSwitchIndex == 10) {
			bulletEnabledControl(false);
			_blockScript.ActSpriteAni(true);
			_blockScript._stopWingObjectMoveChack = true;

			if(_actTransformObject != null){
				_actTransformObject.SetActive(true);
				_actTransformObject = null;
			}else{
				_wing_TF_Left.SetTrigger("wing_Appear");
				_wing_TF_RIght.SetTrigger("wing_Appear");
			}

			_wingTF_Transform.parent = _selfTF;
			_wingTF_Transform.localPosition = _wingTF_OriPosition;

			_wingNormalTF.parent = _selfTF.parent;
			_wing_Normal_Left.SetTrigger("Wing_DisAppear");
			_wing_Normal_RIght.SetTrigger("Wing_DisAppear");

			_drowSwitchIndex = 4;
			_drowChackTime = 0;
			this.enabled = true;
			collisionChack._allstopBulletCreate = true;
		}
	}
}
