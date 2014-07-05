using UnityEngine;
using System.Collections;

public class Player_Hp : MonoBehaviour {

	private Tk2dFadeInOutControl[] _drowHPSpriteList;
	void Awake(){
		_drowHPSpriteList = transform.GetComponentsInChildren<Tk2dFadeInOutControl>();
		//foreach(tk2dSprite selectSprite in _drowHPSpriteList)
		//	selectSprite.color = Color.clear;
	}

	private int _maxDieCount = 0;
	private float _minusBase = 0;
	private float _currentCount = 0;
	public void ViewPlayerHpObject(int maxDieCount){
		_maxDieCount = maxDieCount;
		StartCoroutine("allViewPlayerHpOption");
	}

	IEnumerator allViewPlayerHpOption(){
		int maxCount = _drowHPSpriteList.Length;
		for(int i = 0; i < maxCount; i++){
			_drowHPSpriteList[i].startFadeIn_OR_Out(Tk2dFadeInOutControl.fadeControlAni.hide, Tk2dFadeInOutControl.fadeControlAni.view, 0.2f);
			yield return new WaitForSeconds(0.2f);
		}

		_minusBase = (float)maxCount/(float)_maxDieCount;
		_currentCount = maxCount;
		_drowHPSpriteList[maxCount-1].startFadeInAndOut(0.3f, Tk2dFadeInOutControl.fadeControlAni.view, Tk2dFadeInOutControl.fadeControlAni.halfView, 0, -1);
	}

	void minusPlayerHpValue(int maxCount){
		if(maxCount >= _drowHPSpriteList.Length) maxCount = _drowHPSpriteList.Length-1;

		_drowHPSpriteList[maxCount-1].startFadeInAndOut(0.3f, Tk2dFadeInOutControl.fadeControlAni.view, Tk2dFadeInOutControl.fadeControlAni.halfView, 0, -1);
		_drowHPSpriteList[maxCount].startFadeIn_OR_Out(Tk2dFadeInOutControl.fadeControlAni.view, Tk2dFadeInOutControl.fadeControlAni.hide, 0.3f);
	}

	public void minusHpValueChack(){
		int oldCount = (int)_currentCount;
		_currentCount -= _minusBase;
		int chackCount = (int)_currentCount;
		if(chackCount > 0){
			if(oldCount > chackCount) minusPlayerHpValue(chackCount);
		}else _drowHPSpriteList[0].startFadeIn_OR_Out(Tk2dFadeInOutControl.fadeControlAni.view, Tk2dFadeInOutControl.fadeControlAni.hide, 0.3f);
	}

}
