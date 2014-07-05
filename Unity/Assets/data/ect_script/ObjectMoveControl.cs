using UnityEngine;
using System.Collections;

public class ObjectMoveControl
{
   public static void moveToPosition(GameObject moveObject, float drowTime, Vector3 startVector , Vector3 endVector, 
	                                  GameObject endObject = null, string EndFunction = null, float delayValue = 0.0f, iTween.EaseType easetype = iTween.EaseType.linear, bool resettingChack = false, float speed = 0){
		if(resettingChack) moveObject.transform.localPosition = startVector;
		moveToPosition(moveObject, drowVList: new Vector3[] { startVector,endVector }, drowTime: drowTime, speed: speed, 
		endObject: endObject, EndFunction: EndFunction, delayValue: delayValue, easetype: easetype);
	}

	public static void moveToPosition(GameObject moveObject, Vector3[] drowVList = null, Transform[] drowTList = null,
	                                  float drowTime = 0, float speed = 0, GameObject endObject = null, string EndFunction = null, 
	                                  float delayValue = 0.0f, iTween.EaseType easetype = iTween.EaseType.linear, bool isLocal = false, iTween.LoopType loopControl = iTween.LoopType.none){

		Hashtable optionsHash = new Hashtable();
		if(drowTime != 0) optionsHash.Add("time", drowTime);
		if(speed != 0) optionsHash.Add("speed", speed);

		optionsHash.Add("looptype", loopControl);
		optionsHash.Add("islocal", isLocal);
		optionsHash.Add("easetype", easetype);
		if(drowVList != null) optionsHash.Add("path", drowVList);
		else if(drowTList != null) optionsHash.Add("path", drowTList);

		if (!string.IsNullOrEmpty(EndFunction) && endObject != null){
			optionsHash.Add("oncomplete", EndFunction);
			optionsHash.Add("oncompletetarget", endObject);
		}
		if(delayValue > 0) optionsHash.Add("delay", delayValue);
		iTween.MoveTo(moveObject, optionsHash);
	}

	public static void moveToShakePosition(GameObject moveObject, float drowTime, Vector3 distance, 
	                                       GameObject endObject = null, string EndFunction = null, float delayValue = 0.0f){
		Hashtable optionsHash = new Hashtable();

		optionsHash.Add("time", drowTime);
		optionsHash.Add("amount",distance);
		if (!string.IsNullOrEmpty(EndFunction) && endObject != null){
			optionsHash.Add("oncomplete", EndFunction);
			optionsHash.Add("oncompletetarget", endObject);
		}
		if(delayValue > 0) optionsHash.Add("delay", delayValue);
		iTween.ShakePosition(moveObject, optionsHash);
	}
}
