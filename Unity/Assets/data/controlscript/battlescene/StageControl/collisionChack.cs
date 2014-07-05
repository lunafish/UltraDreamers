using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class collisionChack : MonoBehaviour {

	public static bool _allstopBulletCreate = false;

	[SerializeField] PlayerControl _playerControlSC;
	[SerializeField] BlockControlScript _blockControlSC;
	[SerializeField] TimeLineControl _controlTimeLine;
	[SerializeField] Transform _bullectStorage;
	[SerializeField] Transform _drowStage;
	[SerializeField] List<GameObject> _moveStageControl;

	private Transform _playerTF;
	private List<DestoryOption> _playerBullectList = new List<DestoryOption>();
	private List<DestoryOption> _allEnemyList = new List<DestoryOption>();
	private List<DestoryOption> _enemyDestroyList = new List<DestoryOption>();
	private List<DestoryOption> _allCoinList = new List<DestoryOption>();
	private List<DestoryOption> _PowerUpList = new List<DestoryOption>();
	//private List<DestoryOption> _onotherList = new List<DestoryOption>();

	private List<Transform> _stopTiLine = new List<Transform>();
	public void addStroageData(Transform bullet, DestoryOption copyBullet, bool stopTimeLine){
		if(bullet != null) bullet.parent = _bullectStorage;
		if(stopTimeLine){
			_stopTiLine.Remove(bullet);
			if(_stopTiLine.Count == 0) _controlTimeLine.enabled = true;
		}

		if(copyBullet != null){
			_playerBullectList.Remove(copyBullet);
			_allEnemyList.Remove(copyBullet);
			_enemyDestroyList.Remove(copyBullet);
			_allCoinList.Remove(copyBullet);
			_PowerUpList.Remove(copyBullet);
			//_onotherList.Remove(copyBullet);
		}
	}

	public void createCoinValue(Vector3 _drowPosition, int number){
		_playerControlSC.createCoinValue(null, _drowPosition, number);
	}

	public void setDrowStage(Transform bullect, DestoryOption copyBullet, DestoryOption.objectPosition obPoint, bool stopTimeLine, bool NonDestroyChack){
		if(bullect != null) bullect.parent = _drowStage;
		//Debug.Log(bullect.name + " " + bullect.parent.name);
		//UnityEditor.EditorApplication.isPaused = true;

		if(stopTimeLine){
			if(_stopTiLine.IndexOf(bullect) < 0) _stopTiLine.Add(bullect);
			_controlTimeLine.enabled = false;
		}

		if(!NonDestroyChack && copyBullet != null)
		switch(obPoint){
		case DestoryOption.objectPosition.enemy:
			_allEnemyList.Add(copyBullet);
			if(copyBullet.destroyChack) _enemyDestroyList.Add(copyBullet);
			break;
		case DestoryOption.objectPosition.palyer:
			_playerBullectList.Add(copyBullet);
			break;
		case DestoryOption.objectPosition.coin:
			_allCoinList.Add(copyBullet);
			break;
		case DestoryOption.objectPosition.powerUp:
			_PowerUpList.Add(copyBullet);
			break;
		}

		//_onotherList.Add(copyBullet);
	}
	/*
	public void allStopControlBullet(bool endbledControl){
		this.enabled = endbledControl;
		if(_stopTiLine.Count == 0) _controlTimeLine.enabled = endbledControl;
		int maxCount = _onotherList.Count;
		for(int i = 0; i < maxCount; i++){
			_onotherList[i].allStopBulletValue(endbledControl);
		}

		maxCount = _moveStageControl.Count;
		for(int i = 0; i < maxCount; i++)
		{
			if(endbledControl) iTween.Resume(_moveStageControl[i]);
			else iTween.Pause(_moveStageControl[i]);
		}
	}*/
	/*
	public void removeAllBullet(){
		int maxCount = _onotherList.Count;
		for(int i = 0; i < maxCount; i++){
			_onotherList[0].stopBulletObject(notDestroyParent:false, deleteBullet:true);
		}
	}*/

	void Awake(){
		_allstopBulletCreate = false;
		_playerTF = _playerControlSC.transform;
	}

	public Vector3 PlayerPosition { get { return _playerTF.localPosition; } }
	public DestoryOption enemyChasePosition(Vector3 basePosition){
		int maxCount = _enemyDestroyList.Count;
		if(maxCount > 0){

			int selectIndex = 0;
			float copyMagnitude = 0;
			float chackMagnitude = _enemyDestroyList[0].returnMagnitude(basePosition);
			for(int i = 1; i < maxCount; i++){
				copyMagnitude = _enemyDestroyList[i].returnMagnitude(basePosition);
				if(chackMagnitude > copyMagnitude){
					chackMagnitude = copyMagnitude;
					selectIndex = i;
				}
			}

			return _enemyDestroyList[selectIndex];
		}


		return null;
	}

	public void createPowerUpIter(Vector3 _drowPosition){
		_playerControlSC.createPowerUpItem(_drowPosition);
	}

	private int _indexC = 0;
	private int _indexB = 0;
	private int _drowC = 0;
	private int _drowB = 0;
	private int _max1Count = 0;
	private int _max2Count = 0;
	private bool _removeChack = false;
	private DestoryOption _select1TF = null;
	private DestoryOption _select2TF = null;
	private Vector3 _copyVector3 = Vector3.zero;
	private bool _chaseCoinControl = false;
	void Update() {

		//_allCoinList
		_drowC = 0;
		_max1Count = _allCoinList.Count;
		_copyVector3 = _playerControlSC.VPosition;
		_chaseCoinControl = _blockControlSC._chaseCoinControl;
		int playerStatus = _playerControlSC.getPlayerStatus;

		for(_indexC = 0; _indexC < _max1Count; _indexC++){
			_select1TF = _allCoinList[_drowC++];
			if(_select1TF.chackCrushPlayerValue(_copyVector3) > 0){
				_drowC -= 1;
				_playerControlSC.coindCollisionChack();
			}else _select1TF.coinChaseFlowAct(_chaseCoinControl);
		}

		if(playerStatus == 0){
			_drowC = 0;
			_max1Count = _PowerUpList.Count;
			for(_indexC = 0; _indexC < _max1Count; _indexC++){
				_select1TF = _PowerUpList[_drowC++];
				if(_select1TF.chackCrushPlayerValue(_copyVector3) > 0){
					_drowC -= 1;
					_playerControlSC.PowerUpItemChack();
				}
			}
		}

		_drowC = 0;
		_max1Count = _allEnemyList.Count;
		for(_indexC = 0; _indexC < _max1Count; _indexC++){
			_drowB = 0;
			_removeChack = false;
			_select1TF = _allEnemyList[_drowC++];

			if(_select1TF.destroyChack){
				if(_playerControlSC.colliderBoxChack(_select1TF.destroyBound)){
					if(_select1TF.destroyBullectChack(bladeDelete:true))
					{
						_removeChack = true;
						_drowC -= 1;
						_playerControlSC.setScorePlayerBound(PlayerControl.enemyCrushType.blade_Die);
					}else _playerControlSC.setScorePlayerBound(PlayerControl.enemyCrushType.blade);
				}

				if(!_removeChack){
					_max2Count = _playerBullectList.Count;
					for(_indexB = 0; _indexB < _max2Count; _indexB++){
						_select2TF = _playerBullectList[_drowB++];
						_removeChack = true;
						switch(_select1TF.chackCollisionValue((BullectControl)_select2TF)){
						case 1: // enemy crush
							_drowC -= 1;
							_playerControlSC.setScorePlayerBound(PlayerControl.enemyCrushType.bullet_Die);
							break;
						case 2: // player bullet crush
							_drowB -= 1;
							_playerControlSC.setScorePlayerBound(PlayerControl.enemyCrushType.bullet);
							break;
						case 3: // duth
							_drowC -= 1;
							_drowB -= 1;
							_playerControlSC.setScorePlayerBound(PlayerControl.enemyCrushType.bullet_Die);
							break;
						default: // miss
							_removeChack = false;
							break;
						}
						
						if(_drowB < 0 || _playerBullectList.Count <= _drowB) break;
					}
				}

			}

			if(!_removeChack && !_allstopBulletCreate){
				_removeChack = true;
				switch( _select1TF.chackCrushPlayerValue(_copyVector3)){
				case 1: // crush player
					_playerControlSC.playerCollisionChack(_select1TF.VPosition);
					break;
				case 2: // crush of player and bullect
					_drowC -= 1;
					_playerControlSC.playerCollisionChack(_select1TF.VPosition);
					break;
				default:
					_removeChack = false;
					break;
				}
			}

			if(_drowC < 0 || _allEnemyList.Count <= _drowC) return;
		}
	}
}
