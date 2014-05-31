using UnityEngine;
using System.Collections;

public class StartScene : MonoBehaviour {

	public static bool _viewRankObject = false;
	[SerializeField] GameObject _startScene = null;
	[SerializeField] GameObject _rankScene = null;
	[SerializeField] RankControl _viewRank;
	[SerializeField] GameObject _viewScene;

	void Awake(){
		if(_viewRankObject) _rankScene.SetActive(true);
		else _startScene.SetActive(true);
	}

	void viewRanking (GameObject buttonObject) {
		_startScene.SetActive(false);
		_rankScene.SetActive(true);
		_viewRank.controlViewRankObject(true);
		_viewRankObject = false;
	}

	public void hideRankingView(){
		_viewRankObject = false;
		_startScene.SetActive(true);
		_rankScene.SetActive(false);
	}

	void gameStart (GameObject buttonObject) {
		_viewRankObject = false;
		buttonObject.SetActive(false);
		_viewScene.SetActive(false);
		//Application.LoadLevelAsync("BattleScene");// pro verstion
		Application.LoadLevel("BattleScene");
	}
}
