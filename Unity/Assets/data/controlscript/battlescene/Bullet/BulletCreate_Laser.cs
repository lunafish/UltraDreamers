using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class BulletCreate_Laser : BulletCreate {
	
	[SerializeField] Transform _headTf;
	[SerializeField] tk2dSpriteAnimator _crushEffect;
	[SerializeField] tk2dSprite _drowSprite;
	[SerializeField] string _drowIdleImgName = "";

	[SerializeField] Transform _ImgScaleTF;
	[SerializeField] float _scaleSize = 1;

	private Vector3 _drowScale = Vector3.one;
	private int _originalImgIndex = 0;

	protected override void Awake(){
		base.Awake();

		_originalImgIndex = _drowSprite.spriteId;
		/*Vector3 baseEangle = _selfTF.localEulerAngles;
		baseEangle.z = baseEangle.y * -1f;
		baseEangle.y = 0;
		baseEangle.x = 53;
		
		int maxCount = _selfTF.childCount;
		for(int i = 0; i < maxCount; i++)
			_selfTF.GetChild(0).localEulerAngles = baseEangle;*/
	}

	private int _lastIndex = 0;
	private int _deleteIndex = 0;
	private int _changeHead = 0;
	private float _changeImgAni = 0;
	private bool _positionRe = false;
	public override void crushChildBullet(BullectControl copyBullet){

		_lastIndex = _controlBulletList.IndexOf(copyBullet);
		_controlBulletList.Remove(copyBullet);
		for(_deleteIndex = 0; _deleteIndex < _lastIndex; _deleteIndex++)
		{
			_controlBulletList[0].stopBulletObject(notDestroyParent:false);
			_controlBulletList.RemoveAt(0);
		}
		_bulletListCount = _controlBulletList.Count;

		if(_lastIndex > 0) _positionRe = false;
		else _positionRe = true;

		_changeImgAni = 0;
		if(_changeHead == 0) {
			_changeHead = 1;
			_crushEffect.Play(_drowIdleImgName);
		}
	}

	public void allChildRemove(){
		_headTf.localPosition = Vector3.zero;
		_ImgScaleTF.localScale = Vector3.one;
		_bulletListCount = _controlBulletList.Count;
		for(_deleteIndex = 0; _deleteIndex < _bulletListCount; _deleteIndex++)
			_controlBulletList[_deleteIndex].stopBulletObject(notDestroyParent:false);
		_bulletListCount = 0;
		_controlBulletList.Clear();
	}

	void LateUpdate () {
		if(_bulletListCount <= 0) return;

		if(_changeHead == 1){
			_changeImgAni += Time.deltaTime;
			if(_changeImgAni > 0.1f){
				_changeImgAni = _changeHead = 0;
				_crushEffect.Stop();
				_drowSprite.SetSprite(_originalImgIndex);
				_positionRe = false;
			}
		}

		if(_positionRe) return;

		_headTf.position = _controlBulletList[0].VPosition;
		_drowScale.y = _headTf.localPosition.z * _scaleSize;
		_ImgScaleTF.localScale = _drowScale;
	}
}
