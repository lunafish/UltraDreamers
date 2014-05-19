#if UNITY_EDITOR
//#define repositionObject
#endif

using UnityEngine;
using System.Collections;

#if repositionObject
[ExecuteInEditMode]
#endif
public class MoveStageControl : MonoBehaviour {

	[SerializeField] bool _repositionChack = true;
	[SerializeField] float _moveTime = 5;
	[SerializeField] bool _loopChack = false;
	[SerializeField] float _gridYChack = 0.5f;
	[SerializeField] float _endZPosition = 0;
	//[SerializeField] bool _customZPosition = false;
	//[SerializeField] float _endZPosition = 0;
	private Vector3 _startPosition = Vector3.zero;

#if repositionObject
	[SerializeField] bool reposition = false;
#endif
	
	void Awake(){
		_startPosition = transform.localPosition;
		repositionChack();
		MoveToPosition();
	}

	void repositionChack(){
		//_endZPosition = 0;

		if(!_repositionChack) return;

		Transform selfTF = transform;
		int maxCount = selfTF.childCount;
		Transform getChild = null;
		for(int i = 0; i < maxCount; i++){
			getChild = selfTF.GetChild(i);
			getChild.localPosition = new Vector3(0,0,i*_gridYChack);
			//else _endZPosition = _endZPosition > getChild.localPosition.z ? _endZPosition : getChild.localPosition.z;
		}

		/*if(!_customZPosition)  if(_repositionChack) */ _endZPosition = (maxCount-2)*_gridYChack;
	}

	public void resetMovePosition(){
		iTween.Stop(gameObject);
		transform.localPosition = _startPosition;
		MoveToPosition();
	}

	void MoveToPosition(){
#if repositionObject
#else
		Vector3 startVector = transform.position;

		if(startVector.z == 0) 
			roopMoveControl();
		else{
			Vector3 endVector = startVector;
			endVector.z = 0;

			float moveChackTime = startVector.z;
			float copyEndPoint = _endZPosition;
			if(moveChackTime < 0) moveChackTime *= -1;
			if(copyEndPoint < 0) copyEndPoint *= -1;
			moveChackTime = (moveChackTime/copyEndPoint) * _moveTime;

			ObjectMoveControl.moveToPosition(gameObject, drowVList:  new Vector3[] { startVector, endVector }, 
			drowTime:moveChackTime, endObject:gameObject, EndFunction:"roopMoveControl");
		}
#endif
	}
	
	void roopMoveControl(){
		Vector3 startVector = transform.position;
		Vector3 endVector = startVector;
		endVector.z -= _endZPosition;

#if repositionObject
#else
		ObjectMoveControl.moveToPosition(gameObject, drowVList:  new Vector3[] { startVector, endVector }, 
											drowTime:_moveTime, loopControl: _loopChack ? iTween.LoopType.loop :iTween.LoopType.none);
#endif
	}

#if repositionObject
	void Update(){
		if(reposition){
			reposition = false;
			repositionChack();
		}
	}
#endif
}
