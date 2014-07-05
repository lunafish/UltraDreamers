using UnityEngine;
using System.Collections;

public class buttonComponent : TouchControl 
{
    [SerializeField] GameObject _MainTarget;
	[SerializeField] string _MainFunctionName;
	[SerializeField] GameObject _CancelTarget;
	[SerializeField] string _CancelFunctionName;
	[SerializeField] GameObject _MoveTarget;
	[SerializeField] string _MoveFunctionName;
	[SerializeField] GameObject _HoldTarget;
	[SerializeField] string _HoldFunctionName;
	[SerializeField] GameObject _EndTarget;
	[SerializeField] string _EndFunctionName;

	[SerializeField] float _drowTime = 0.1f;
	[SerializeField] Vector3 _ScaleValue = new Vector3(0.9f, 0.9f, 1.0f);
	[SerializeField] Vector3 _endScaleValue = Vector3.zero;

	protected Vector3 _returnScale = Vector3.one;
	protected bool moveOut = false;
    protected ObjectScaleControl _ScaleControl;
	private bool _buttonEnabled = true;
	
	// Use this for initialization
    void Awake()
    {
		setButtonValue();
    }
	
	public void setButtonEnabled(bool control){
		_buttonEnabled = control;
	}
	
	protected void setButtonValue(){
		_returnScale = transform.localScale;
		if(_endScaleValue.magnitude == 0) _endScaleValue = _returnScale;
        _ScaleControl = GetComponent("ObjectScaleControl") as ObjectScaleControl;
	}

	void Start () {
        SetTouch ();
	}

	void Update ()
    {
		TouchChackControl();
	}
	
	public void returnScaleImg(){
		if(_ScaleControl == null) return;
		_ScaleControl.drowToScale(_returnScale, _drowTime);
	}
	
	public void drowEndScale(){
		if(_ScaleControl == null) return;
		_ScaleControl.drowToScale(_endScaleValue, _drowTime);
	}
	
	protected override void touchBegan(Vector3 moveP){
		
		if(!_buttonEnabled) {
			touchStop();
			return;
		}
		
		buttonBegan();
		sendControl();
	}

    protected override void touchMove(Vector3 moveP)
    {
		if(_ScaleControl != null) _ScaleControl.drowToScale(_ScaleValue, _drowTime);
		moveOut = false;
		Send(_MoveFunctionName ,_MoveTarget);
    }
	
	protected override void touchMoveStop(Vector3 moveP)
	{
		Send(_HoldFunctionName ,_HoldTarget);
	}

    protected override void touchMoveOut(Vector3 moveP)
    {
		moveOut = true;
        returnScaleImg();
    }

    protected override void touchEnd(Vector3 moveP)
    {
		buttonEnd();
    }
	
	protected override void touchCancel(Vector3 moveP)
    {
		buttonEnd();
        Send(_CancelFunctionName, _CancelTarget);
    }
	
	protected void buttonBegan(){
		resetButtonValue();
        if(_ScaleControl != null) _ScaleControl.drowToScale(_ScaleValue, _drowTime);
	}
	
	protected void resetButtonValue(){
		moveOut = false;
	}
	
	protected void buttonEnd(){
		moveOut = false;
		drowEndScale();
		Send(_EndFunctionName, _EndTarget);
	}
	
	protected void sendControl(){
		Send(_MainFunctionName, _MainTarget);
	}

    void Send(string functionName, GameObject target)
    {
		if(!_buttonEnabled) {
			touchStop();
			return;
		}
		
        if (string.IsNullOrEmpty(functionName) || target == null) return;

        target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
