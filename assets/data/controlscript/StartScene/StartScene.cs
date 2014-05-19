using UnityEngine;
using System.Collections;

public class StartScene : MonoBehaviour {
	
	void gameStart (GameObject buttonObject) {
		buttonObject.SetActive(false);
		//Application.LoadLevelAsync("BattleScene");// pro verstion
		Application.LoadLevel("BattleScene");
	}
}
