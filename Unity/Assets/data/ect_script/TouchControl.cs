using UnityEngine;
using System.Collections;

public class TouchControl : MonoBehaviour {

	[SerializeField] bool _multiTouchControl = false;
	protected float _scaleControl;
	private Collider _selfCollider;
	private bool _objectTouch = false;
	private Camera _mainCamera = null;
	protected Camera_Main _CameraScript;
	protected float _controlScale = 1.0f;
	
	protected bool _moveChack = true;
	private float _touchTimeChack = 0;
	private float _maxTouchTime = 0.1f;
	private Vector3 _endMoveChack = Vector3.zero;

	void Start(){
		SetTouch();
	}
	
	protected void SetTouch () {
		_selfCollider = collider;
		_mainCamera = getCamera(transform);
		_CameraScript = _mainCamera.transform.GetComponent<Camera_Main>() as Camera_Main;
		_scaleControl = (_mainCamera.GetComponent<Camera_Main>() as Camera_Main)._mScaleControl;
	}
	
	public static Camera getCamera(Transform ChackT)
    {
        Camera CmeraChack = ChackT.camera;
        if (CmeraChack == null)
        {
            if (ChackT.parent != null) return getCamera(ChackT.parent);
            else return null;
        }

        return CmeraChack;
    }

	protected virtual bool childBeganTouch() { return false; }
	bool chackTargetObject(Vector2 point)
    {
		if (_mainCamera == null || _selfCollider == null || childBeganTouch()) return false;
		
        Ray ray = _mainCamera.ScreenPointToRay(point);
        RaycastHit hitInfo;
        if (_selfCollider.Raycast(ray, out hitInfo, 1.0e8f))
        {
            if (!Physics.Raycast(ray, hitInfo.distance - 0.01f))
                return true;
        }
        return false;
    }
	
	Vector3 getMousePosition(Vector3 position)
    {
        return position * _scaleControl / _controlScale;
    }

	protected void closeTouchObject(){
		if(_selfCollider == null) return;

		this.enabled = false;
		_selfCollider = null;
	}

	protected void OpenTouchObject(){
		if(_selfCollider != null) return;
		_selfCollider = collider;
	}

	protected void removeTouchObject(){
		if(_mainCamera == null) return;
		this.enabled = false;
		_mainCamera = null;
		Destroy(_selfCollider);
		_selfCollider = null;
	}
	
	protected virtual bool getMoveChack(Vector3 currentMove){
		return _moveChack && (_maxTouchTime < (_touchTimeChack += Time.deltaTime) || (_endMoveChack - currentMove).magnitude > 0.05f);
	}
	
	void Update(){
		TouchChackControl();
	}

#if UNITY_EDITOR
	void debugOption(){
		Debug.Log(_multiTouchControl);
	}
#endif
	
	protected void TouchChackControl(){
#if UNITY_EDITOR
		Vector3 mousePosition = getMousePosition(Input.mousePosition);
		if (_objectTouch)
		{
			if (Input.GetMouseButtonUp(0)) {
				//if (chackTargetObject(Input.mousePosition)) 
				BaseTouchEnd(mousePosition);
				//else BaseTouchCancel(mousePosition);
			}
			else if(getMoveChack(mousePosition)) {
				if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
				{
					//if (chackTargetObject(Input.mousePosition)) 
					BaseTouchMove(mousePosition);
					//else BaseTouchMoveOut(mousePosition);
				}else {
					_touchTimeChack = 0;
					_endMoveChack = mousePosition;
					BaseTouchMoveStop(mousePosition);
				}
			}
			else BaseTouchMoveStop(mousePosition);
		}
		else if (Input.GetMouseButtonDown(0))
		{
			if (chackTargetObject(Input.mousePosition)) BaseTouchBegan(mousePosition);
			//else BaseTouchCancel(mousePosition);
		}
#elif UNITY_IPHONE  || UNITY_ANDROID
		for(int i = 0; i < Input.touchCount; i++){
			touchOptionControl(Input.GetTouch(i));
			if(!_multiTouchControl) return;
		}
#endif

	}

#if UNITY_IPHONE  || UNITY_ANDROID
	void touchOptionControl(Touch oneTouch){
		Vector3 mousePosition = getMousePosition(oneTouch.position);
		if (_objectTouch)
		{
			if (oneTouch.phase == TouchPhase.Ended) {
				//if(chackTargetObject(oneTouch.position)) 
				BaseTouchEnd(mousePosition);
				//else touchCancel(mousePosition);
			}
			else if(oneTouch.phase == TouchPhase.Canceled) BaseTouchCancel(mousePosition);
			else if(getMoveChack(mousePosition)){
				if (oneTouch.phase == TouchPhase.Moved) {
					//if(chackTargetObject(oneTouch.position)) 
					BaseTouchMove(mousePosition);
					//else BaseTouchMoveOut(mousePosition);
				}else {
					_touchTimeChack = 0;
					_endMoveChack = mousePosition;
					BaseTouchMoveStop(mousePosition);
				}
			}
			else BaseTouchMoveStop(mousePosition);
		}
		else if (oneTouch.phase == TouchPhase.Began)
		{
			if (chackTargetObject(oneTouch.position)) BaseTouchBegan(mousePosition);
			//else BaseTouchCancel(mousePosition);
		}
	}
#endif
	
	void interceptTouchObject(GameObject touchObject){
#if UNITY_EDITOR 
		Vector3 GetPoint = Input.mousePosition;
#elif UNITY_IPHONE  || UNITY_ANDROID
		Vector3 GetPoint = Input.GetTouch(0).position;
#endif
		
		(touchObject.GetComponent("TouchControl") as TouchControl).touchStop();
		BaseTouchBegan(getMousePosition(GetPoint));
	}
	
	void BaseTouchBegan(Vector3 moveP){
		touchBegan(moveP);
		_endMoveChack = moveP;
		_objectTouch = true;
		_touchTimeChack = 0;
	}

    void BaseTouchMove(Vector3 moveP)
    {
		touchMove(moveP);
    }

    void BaseTouchMoveOut(Vector3 moveP)
    {
		touchMoveOut(moveP);
    }
	
	void BaseTouchMoveStop(Vector3 moveP)
    {
		touchMoveStop(moveP);
    }

    void BaseTouchEnd(Vector3 moveP)
    {
		_objectTouch = false;
		touchEnd(moveP);
    }
	
	void BaseTouchCancel(Vector3 moveP)
    {
		_objectTouch = false;
		touchCancel(moveP);
    }
	
	public virtual void touchStop(){
		_objectTouch = false; 
	}
	public void touchMoveStop(){ _moveChack = false; }
	
	protected virtual void touchBegan(Vector3 moveP){}
    protected virtual void touchMove(Vector3 moveP){}
    protected virtual void touchMoveOut(Vector3 moveP){}
	protected virtual void touchMoveStop(Vector3 moveP){}
    protected virtual void touchEnd(Vector3 moveP){}
	protected virtual void touchCancel(Vector3 moveP){}
}
