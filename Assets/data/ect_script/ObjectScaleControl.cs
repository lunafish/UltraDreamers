using UnityEngine;
using System.Collections;

public class ObjectScaleControl : MonoBehaviour
{
	private bool _moveControl;
	private Transform _selfTF;
	private GameObject _selfObject;
	
	private GameObject _MainTarget;
    private string _MainFunctionName;
	private Hashtable _optionsHash = new Hashtable();

	void Awake(){
		_selfTF = transform;
		_selfObject = gameObject;
	}

    public void drowToScale(Vector3 startScale, Vector3 endScale, float drowTime = 0, GameObject endObject = null, string EndFunction = null)
    {
        _selfTF.localScale = startScale;
        drowToScale(endScale, drowTime, endObject, EndFunction);
    }

    public void drowToScale(Vector3 endScale, float drowTime = 0, GameObject endObject = null, string EndFunction = null)
    {
		_MainTarget = endObject;
		_MainFunctionName = EndFunction;
		
		if(drowTime != 0){
			_moveControl = true;
	        
			_optionsHash.Clear();
			_optionsHash.Add("time", drowTime);
			_optionsHash.Add("islocal", true);
			_optionsHash.Add("easetype", "linear");
			_optionsHash.Add("scale", endScale);
			_optionsHash.Add("oncomplete", "sendEndControl");
			_optionsHash.Add("oncompletetarget", _selfObject);
			iTween.ScaleTo(_selfObject, _optionsHash);
		}
        else {
			sendEndControl();
			_moveControl = false;
			_selfTF.localScale = endScale;
		}
    }

    public bool animationChack()
    {
        return _moveControl;
    }

    float getValue(float _value)
    {
        return (_value < 0 ? (-1 * _value) : _value);
    }

	public void PauseToMoveAni(){
		_moveControl = false;
		iTween.Pause(_selfObject);
	}
	
	public void rePlayToMoveAni(){
		_moveControl = true;
		iTween.Resume(_selfObject);
	}
	
	public void stopToScaleAni()
	{
		_moveControl = false;
		iTween.Stop(_selfObject);
	}
	
	void sendEndControl()
	{
		_moveControl = false;
		if (string.IsNullOrEmpty(_MainFunctionName) || _MainTarget == null) return;	
       	_MainTarget.SendMessage(_MainFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
	}

	public static void scaleToObject(GameObject scaleObject, float drowTime, Vector3 endScale, 
	                                 GameObject endObject = null, string EndFunction = null, float delayValue = 0.0f, string easetype = "linear")
	{
		Hashtable optionsHash = new Hashtable();

		optionsHash.Add("time", drowTime);
		optionsHash.Add("islocal", true);
		optionsHash.Add("easetype", easetype);
		optionsHash.Add("scale", endScale);
		if (!string.IsNullOrEmpty(EndFunction) && endObject != null){
			optionsHash.Add("oncomplete", EndFunction);
			optionsHash.Add("oncompletetarget", endObject);
		}
		if(delayValue > 0) optionsHash.Add("delay", delayValue);
		iTween.ScaleTo(scaleObject, optionsHash);
	}

	public static void scaleToShakeObject(GameObject moveObject, float drowTime, Vector3 distance, 
	                                       GameObject endObject = null, string EndFunction = null, float delayValue = 0.0f){
		Hashtable optionsHash = new Hashtable();
		
		optionsHash.Add("time", drowTime);
		optionsHash.Add("amount",distance);
		if (!string.IsNullOrEmpty(EndFunction) && endObject != null){
			optionsHash.Add("oncomplete", EndFunction);
			optionsHash.Add("oncompletetarget", endObject);
		}
		if(delayValue > 0) optionsHash.Add("delay", delayValue);
		iTween.ShakeScale(moveObject, optionsHash);
	}
}
