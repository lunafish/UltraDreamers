using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class BattleStartControl : MonoBehaviour {

	[SerializeField] Animation _startAnimation;
	[SerializeField] PlayerControl _playerSC;
	[SerializeField] List<GameObject> _startViewObject = new List<GameObject>();

	private Transform _selfTF = null;
	private List<GameObject> _honeycomOpbject = new List<GameObject>();
	void Awake(){
		_selfTF = transform;
		bool _leftRightCheck = false;
		float widthSize = 0.243f;
		Transform selectTF = null;
		int maxCount = 0;
		for(int i = 0; i < 7; i++){
			selectTF = _selfTF.FindChild("Honeycom_" + i);
			selectTF.localPosition = new Vector3(_leftRightCheck ? widthSize : 0, -0.423f * i,0);
			maxCount = selectTF.childCount;
			for(int j = 0; j < maxCount; j++){
				selectTF.GetChild(j).localPosition  = new Vector3( widthSize * 2 * j , 0, 0); 
				selectTF.GetChild(j).gameObject.SetActive(false);
			}
			_leftRightCheck = !_leftRightCheck;
		}

		List<int> _indexList = new List<int>();
		List<bool> _maxCountCheck = new List<bool>();
		int _maxAddObjectCount = 0;
		while(_maxAddObjectCount < 7){
			for(int i = 0; i < 7; i++){
				if(_indexList.Count <= i){
					_indexList.Add(0);
					_maxCountCheck.Add(false);
				}
				selectTF = _selfTF.FindChild("Honeycom_" + i);
				
				if(_indexList[i] == 0){
					_indexList[i]++;
					_honeycomOpbject.Add(selectTF.GetChild(0).gameObject);
					i = -1;
				}
				else{
					maxCount = selectTF.childCount;
					if(_indexList[i] + 1 <= maxCount) 
					{
						for(int j = _indexList[i]; j < _indexList[i] + 1; j++){
							_honeycomOpbject.Add(selectTF.GetChild(j).gameObject);
						}
						_indexList[i]++;
					}else if(!_maxCountCheck[i]){
						_maxCountCheck[i] = true;
						_maxAddObjectCount++;
					}
				}
			}
		}
	}

	void Start () {
		StartCoroutine("viewhoneycom");
	}

	private bool _viewHoneycomControl = true;
	IEnumerator viewhoneycom(){
		int maxCount = _honeycomOpbject.Count;

		for(int i = 0; i < maxCount; i++){
			_honeycomOpbject[i].SetActive(_viewHoneycomControl);
			yield return new WaitForSeconds(0.005f);
		}

		if(_viewHoneycomControl){
			_viewHoneycomControl = false;
			_startAnimation.Play();
		}
	}
	
	void star_HideHoneyComAni(){
		_honeycomOpbject.Reverse();
		StartCoroutine("viewhoneycom");
	}

	void endAnimationCheck(){
		int maxCount = _startViewObject.Count;
		for(int i = 0; i < maxCount; i++){
			_startViewObject[i].SetActive(true);
		}
		_playerSC.viewPlayerWingObject();
	}
}
