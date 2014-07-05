using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class Camera_Main : MonoBehaviour
{
	public static bool iphone4_5 = true;

	public static float _WidthSize = 2.0f;
	public static float _heightSize = 3.0f;
	
	public float _mScaleControl;
	
	public enum viewPortControl
    {
        portrait,
		landscape
    }
	public viewPortControl _viewPoint = viewPortControl.portrait;
	
    void Awake()
    {
		camera.useOcclusionCulling = false;
		switch(_viewPoint){
		case viewPortControl.portrait:
			portraitCamera();
			break;
		case viewPortControl.landscape:
			landscapeCamera();
			break;
		}
    }
	
	void landscapeCamera(){
		
		Camera camera = transform.camera;
		float _widthValue = 3.0f;
        float _heightValue = 2.0f;
		_WidthSize = 3.0f;
		_heightSize = 2.0f;	
		
		camera.orthographicSize = 1;
		iphone4_5 = false;
		
		float windowaspect = (float)Screen.height/(float)Screen.width;
       	if (windowaspect > 1.5f)
        {
			_WidthSize = 3.55f;
			iphone4_5 = true;
            _widthValue = 16.0f;
            _heightValue = 9.0f;
        }
		
		float targetaspect = _heightValue/_widthValue;
        float scalewidth = windowaspect / targetaspect;

        if (scalewidth < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
			_mScaleControl = _heightSize/(float)Screen.height;
        }
        else // add pillarbox
        {
            float scaleheight = 1.0f / scalewidth;

            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
			_mScaleControl = _WidthSize/(float)Screen.width;
        }
		
	}
	
	void portraitCamera(){
		Camera camera = transform.camera;
		float _widthValue = 2.0f;
        float _heightValue = 3.0f;
		_WidthSize = 2.0f;
		_heightSize = 3.0f;	
		
		camera.orthographicSize = 1.5f;

		float windowaspect = (float)Screen.width / (float)Screen.height;
/*#if UNITY_IPHONE
		iphone4_5 = false;
		if (windowaspect < 0.6f || 0.75f <= windowaspect)
#endif*/
        {
			_heightSize = 3.55f;
			iphone4_5 = true;
            _widthValue = 9.0f;
            _heightValue = 16.0f;
        }

        camera.orthographicSize = _heightValue / _widthValue;
		float scaleheight = windowaspect / (_widthValue / _heightValue);

        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;
            _mScaleControl = _WidthSize / (float)Screen.width;
            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
            _mScaleControl = _heightSize / (float)Screen.height;
        }

		camera.fieldOfView = 39.1f;
	}
}