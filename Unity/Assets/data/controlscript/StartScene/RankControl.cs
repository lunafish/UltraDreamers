using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankControl : MonoBehaviour {

	[System.Serializable]
	public class scoreOption{
		public string _playerName = "UTD";
		public int _playerScore = 0;
	}

#if UNITY_IPHONE || UNITY_ANDROID
	private static TouchScreenKeyboard _KeyBoard;
#endif

	[SerializeField] StartScene _controlStartScene = null;
	[SerializeField] GameObject _pressButton = null;
	private List<GameObject> _RankObjectList = new List<GameObject>();
	private List<Vector3> _centerPosition = new List<Vector3>();
	private Vector3 _addVector = new Vector3(2, 0, 0);
	
	private List<NumberControl> _numberControl = new List<NumberControl>();
	static private List<scoreOption> _scoreList = new List<scoreOption>();
	private UILabel _selectLabe = null;
	private int _indexOf = -1;
	private bool _awakeCheck = false;
	void Awake(){
		if(_awakeCheck) return;
		_awakeCheck = true;

		if(_scoreList.Count == 0){
			for(int i = 0; i < 8; i++){
				scoreOption scOp = new scoreOption();
				_scoreList.Add(scOp);
			}
		}


		if(PlayerControl._currentCoinValue > 0){
			scoreOption scOp = new scoreOption();
			scOp._playerName = "";
			scOp._playerScore = PlayerControl._currentCoinValue;
			_scoreList.Add(scOp);

			_scoreList.Sort(compare);
			_scoreList.Reverse();

			_scoreList.RemoveAt(_scoreList.Count - 1);
			_indexOf = _scoreList.IndexOf(scOp);
			PlayerControl._currentCoinValue = 0;
		}



		Transform selfTF = transform;
		int maxCount =  selfTF.childCount;
		Transform selectTf = null;
		_selectLabe = null;
		for(int i = 0; i < maxCount; i++){
			selectTf = selfTF.FindChild("RankObject_" + (i+1));
			_centerPosition.Add(selectTf.position);
			_RankObjectList.Add(selectTf.gameObject);
			(selectTf.FindChild("Label").GetComponent("UILabel") as UILabel).text = _scoreList[i]._playerName;
			_numberControl.Add(selectTf.FindChild("NumberObject").GetComponent("NumberControl") as NumberControl);

			selectTf.position += _addVector;
			if(_indexOf == i) _selectLabe = selectTf.FindChild("Label").GetComponent("UILabel") as UILabel;
		}

		this.enabled = true;
	}

	void Start(){
		if(StartScene._viewRankObject){
			controlViewRankObject(true);
		}

		this.enabled = false;
		StartScene._viewRankObject = false;
	}

	public int compare(RankControl.scoreOption x, RankControl.scoreOption y)
	{
		int xN = x._playerScore;
		int yN = y._playerScore;
		return xN.CompareTo(yN);
	}

	private int _viewNumverControl = 0;
	void endPosition(){
		_numberControl[_viewNumverControl].ViewNumberCount(_scoreList[_viewNumverControl]._playerScore, 0.3f);
		_viewNumverControl++;
#if UNITY_IPHONE || UNITY_ANDROID
		if(_viewNumverControl >= 8 && _selectLabe != null){
			this.enabled = true;
			_pressButton.SetActive(true);
			_KeyBoard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);
			//TouchScreenKeyboard.hideInput = true;
		}
#else
		if(_viewNumverControl >= 8) _pressButton.SetActive(true);
#endif
	}

#if UNITY_IPHONE || UNITY_ANDROID
	void Update(){
		if(_KeyBoard.done){
			this.enabled = false;
			_scoreList[_indexOf]._playerName = _selectLabe.text = _KeyBoard.text;
			_selectLabe = null;
		}
	}
#endif
	
	void hideRankObject(GameObject pressButton){
		_pressButton = pressButton;
		_pressButton.SetActive(false);
		controlViewRankObject(false);
	}

	public void controlViewRankObject(bool viewOrHide){
		_viewNumverControl = 0;
		Awake();

		int maxCount = _RankObjectList.Count;
		Vector3 startPosition = Vector3.zero;
		for(int i = 0; i < maxCount; i++){

			if(viewOrHide)
			{
				startPosition = _RankObjectList[i].transform.position = _centerPosition[i] + _addVector;
				ObjectMoveControl.moveToPosition(_RankObjectList[i], 0.3f, startPosition, _centerPosition[i], gameObject, "endPosition", delayValue: 0.15f * i, easetype: iTween.EaseType.easeOutQuart);
			}
			else{
				startPosition = _RankObjectList[i].transform.position = _centerPosition[i];
				startPosition -= _addVector;
				ObjectMoveControl.moveToPosition(_RankObjectList[i], 0.1f, _centerPosition[i] , startPosition, gameObject, "hideScene", delayValue: 0.05f * i);
			}
		}
	}

	void hideScene(){
		_viewNumverControl++;
		if(_viewNumverControl >= 8){
			_controlStartScene.hideRankingView();
		}
	}
}
