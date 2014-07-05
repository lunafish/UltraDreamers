using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumberControl : MonoBehaviour {
	
	public enum Alignment
	{
		left,
		center,
		right
	}
	
	[SerializeField] Alignment vAlign = Alignment.right;
	
	protected Transform _selfTF;
	protected int _countNumber = 0;
	//protected AllFadeInOutControl _fadeInControl = null;
	
	private int _indexCount = 1;
	private float _intervalX = 0.08f;
	private tk2dSprite _originalSprite;
	private List<tk2dSprite> _numberSprite = new List<tk2dSprite>();
	private List<Transform> _numberTransform = new List<Transform>();
	
	void Awake(){
		resetNumberValue();
	}

	public void resetNumberValue(){
		if(_selfTF != null) return;
		
		settingValue();
		removeNumber();
		this.enabled = false;
		ViewNumberCount(0, true);
	}
	
	void settingValue(){
		_selfTF = transform;

		Transform numberTF = transform.FindChild("Number");
		_originalSprite = numberTF.GetComponent("tk2dSprite") as tk2dSprite;
		switch (vAlign)
		{
		case Alignment.left:
			numberTF.localPosition = new Vector3(_intervalX,0,0);
			break;
		case Alignment.center:
			numberTF.localPosition = new Vector3(_intervalX/2.0f,0,0);
			break;
		}
		
		//_fadeInControl = transform.GetComponent("AllFadeInOutControl") as AllFadeInOutControl;
		_numberSprite.Add(_originalSprite);
		_numberTransform.Add(numberTF);
	}

	void removeNumber(){
		_countNumber = 0;
		StopAllCoroutines();
		int maxCount = _numberSprite.Count;
		for(int i = 0; i < maxCount; i++) _numberSprite[i].SetSprite("Img_blank");
	}
	/*
	public void viewnumberFade(bool viewChack, float drowTime = 0){
		StopAllCoroutines();
		//if(_fadeInControl == null) return;
		//_fadeInControl.viewFadeInOutControl(viewChack ? 1 : 0, drowTime);
	}

	public void setColorData(Color selectColor){
		selectColor.a = _fadeInControl == null ? 1 : _fadeInControl.currentAlpha;
		int maxCount = _numberSprite.Count;
		for(int i = 0; i < maxCount; i++)
			_numberSprite[i].color = selectColor;
	}

	public int countNumber { get { return _countNumber; } }*/
	public void ViewNumberCount(int numberValue, float drowTime, GameObject endObject = null, string endFunction = ""){
		if(numberValue == _countNumber) return;
		
		this.StopAllCoroutines();
		if(drowTime > 0){
			int reverseChack = 1;
			int chackNumber = numberValue - _drowNumber;
			if(chackNumber < 0) {
				reverseChack = -1;
				chackNumber *= -1;
			}
			
			float cdrowTime = drowTime/chackNumber;
			if(cdrowTime < 0.05f){
				cdrowTime = 0.05f;
				
				float saveNumber = drowTime/cdrowTime;
				reverseChack *= (int)(chackNumber/saveNumber);
			}

			_countNumber = numberValue;
			_Update_endObject = endObject;
			_Update_endFunction = endFunction;
			_Update_drowTime = cdrowTime;
			_Update_addCount = reverseChack;
			this.enabled = true;
			//StartCoroutine(AddViewNumberCount(cdrowTime, reverseChack, endObject, endFunction));

		}else ViewNumberCount(numberValue);
	}

	private float _Update_drowTime;
	private int _Update_addCount;
	private GameObject _Update_endObject;
	private string _Update_endFunction;
	private int _drowNumber = 0;
	private float _currentTime = 0;

	void Update(){
		_currentTime+= Time.deltaTime;
		if(_currentTime < _Update_drowTime) return;
		_currentTime -= _Update_drowTime;

		if(((_Update_addCount > 0) && (_drowNumber < _countNumber)) ||
		   ((_Update_addCount < 0) && (_drowNumber > _countNumber))){

			_drowNumber += _Update_addCount;
			ViewNumberCount(_drowNumber, true);
			return;
		}


		if(_drowNumber != _countNumber) ViewNumberCount(_countNumber);
		if (!string.IsNullOrEmpty(_Update_endFunction) && _Update_endObject != null) {
			GameObject endObject = _Update_endObject;
			string endFunction = _Update_endFunction;

			_Update_endFunction = "";
			_Update_endObject = null;
			endObject.SendMessage(endFunction, "", SendMessageOptions.DontRequireReceiver);
		}

		this.enabled = false;
		_currentTime = 0;
	}


	public int ViewNumberCount(int numberValue, bool active = false){
		if(!active) _drowNumber = _countNumber = numberValue;
		
		if(numberValue < 0) numberValue *= -1;
		int drowCount = 0;
		int indexCount = 0;
		int maxCount = _numberSprite.Count;
		int oldMaxCount = _indexCount;
		
		while(numberValue > 0){
			
			drowCount = numberValue%10;
			numberValue = (numberValue - drowCount)/10;
			
			if(maxCount <= indexCount){
				maxCount++;
				tk2dSprite copySprite = Instantiate(_originalSprite) as tk2dSprite;
				Transform copyTF = copySprite.transform;
				copyTF.parent = _selfTF;
				copyTF.localScale = Vector3.one;
				copyTF.localEulerAngles = Vector3.zero;
				switch (vAlign)
				{
				case Alignment.left:
					copyTF.localPosition = new Vector3(_intervalX * (indexCount+1),0,0);
					break;
				case Alignment.right:
					copyTF.localPosition = new Vector3(_intervalX * indexCount * -1,0,0);
					break;
				}
				_numberSprite.Add(copySprite);
				_numberTransform.Add(copyTF);
				//if(_fadeInControl != null) _fadeInControl.addNewTransform(copyTF);
			}
			_numberSprite[indexCount++].SetSprite("combo_"+drowCount);
		}
		_indexCount = indexCount;
		switch (vAlign)
		{
		case Alignment.center:
			if(_indexCount != oldMaxCount){
				float startX = (_intervalX/2.0f)*indexCount;
				for(int i = 0; i < indexCount; i++)
					_numberTransform[i].localPosition = new Vector3(startX - _intervalX*i,0,0);
			}
			break;
		}
		
		for(int i = indexCount; i < maxCount; i++)
			_numberSprite[i].SetSprite("Img_blank");

		if(indexCount == 0)_numberSprite[0].SetSprite("combo_0");
		
		return indexCount;
	}
}
