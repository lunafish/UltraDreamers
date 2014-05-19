using UnityEngine;
using System.Collections;

public class ObjectRotateControl : MonoBehaviour
{
	public bool _reverseChack = false;
	public float _accuracy = 20.0f;
	
    private bool _moveControl;
	private Vector3 _drowPoint = Vector3.zero;
    private Vector3 _endPoint = Vector3.zero;
	private Vector3 _startPoint = Vector3.zero;
	private float _EndMagitude;
	private Transform _selfTF;

	private GameObject _MainTarget;
	private string _MainFunctionName;

	private float _reverseControl = 1;
	
	void Awake(){
		_selfTF = transform;
		//this.enabled = false;
	}

	public void rotateToPosition(Vector3 startPoint, Vector3 endPoint, float drowTime, GameObject endObject = null, string EndFunction = null)
    {
        _selfTF.localEulerAngles = startPoint;
		rotateToPosition(endPoint, drowTime, endObject, EndFunction);
    }

	public void rotateToPosition(Vector3 endPoint, float drowTime, GameObject endObject = null, string EndFunction = null)
    {
		if(_selfTF == null) _selfTF = transform;

		_endPoint = endPoint;
		_MainTarget = endObject;
		_MainFunctionName = EndFunction;

		if(drowTime > 0){
			_moveControl = true;
			this.enabled = true;

			_reverseControl = _reverseChack ? -1 : 1;
			
			_startPoint = _selfTF.localEulerAngles;
			if(_startPoint.z > 180f) _startPoint.z -= 360f;
			
			_drowPoint = (_endPoint - _startPoint);

			_EndMagitude = _drowPoint.magnitude;
			_EndMagitude -= _EndMagitude/_accuracy;
			_drowPoint = _drowPoint / drowTime;
		}else{
			this.enabled = false;
			sendEndControl();
			_selfTF.localEulerAngles = endPoint;
		}
        
    }

	void sendEndControl(){
		this.enabled = false;
		if (string.IsNullOrEmpty(_MainFunctionName) || _MainTarget == null) return;
		_MainTarget.SendMessage(_MainFunctionName, gameObject, SendMessageOptions.DontRequireReceiver);
	}

    public bool animationChack()
    {
        return _moveControl;
    }

	public void PauseToMoveAni(){
		_moveControl = false;
		this.enabled = false;
	}

	public void rePlayToMoveAni(){
		_moveControl = true;
		this.enabled = true;
	}

	public void stopToMoveAni()
	{
		if(_selfTF == null) _selfTF = transform;

		_moveControl = false;
		_selfTF.eulerAngles = _endPoint;
		this.enabled = false;
	}

    void Update()
    {
        if (_moveControl)
        {
            Vector3 drowP = (_drowPoint*Time.deltaTime);
            Vector3 intervalV = _selfTF.localEulerAngles;
			
			if(intervalV.z > 180f) {
				intervalV.z -= 360f;
				intervalV = _startPoint - intervalV;
			}
			else intervalV = _startPoint - intervalV;
			
            float mag = intervalV.magnitude;
			
			if( mag >= _EndMagitude)
            {
                _moveControl = false;
				this.enabled = false;
                _selfTF.localEulerAngles = _endPoint;
				sendEndControl();
			}
			else _selfTF.localEulerAngles += (drowP * _reverseControl);
        }
    }

	public static void rotateToObject(GameObject moveObject, float drowTime, Vector3 eulerAngles, GameObject endObject = null, string EndFunction = null, float delayValue = 0.0f, string easetype = "linear"){
		Hashtable optionsHash = new Hashtable();
		optionsHash.Add("time", drowTime);
		optionsHash.Add("easetype", easetype);
		optionsHash.Add("islocal", true);
		optionsHash.Add("rotation", eulerAngles);
		if (!string.IsNullOrEmpty(EndFunction) && endObject != null){
			optionsHash.Add("oncomplete", EndFunction);
			optionsHash.Add("oncompletetarget", endObject);
		}
		if(delayValue > 0) optionsHash.Add("delay", delayValue);
		iTween.RotateTo(moveObject, optionsHash);
	}
}
