using UnityEngine;
using System.Collections;

public class Tk2dFadeInOutControl : MonoBehaviour {
	/*
	[SerializeField] bool _autoFadeINOUT = false;
	[SerializeField] float _autoFadeSpeed = 0.3f;
	[SerializeField] float _autoFadeDelaySpeed = 0.1f;*/

	public enum fadeControlAni{
		view,
		halfView,
		hide
	}

	[SerializeField] fadeControlAni _startFade = fadeControlAni.view;

    private tk2dSprite _CImgTk2d;
    private bool _FadeInOutAni = false;
    private float _startFTime;
    private float _drowFSpeed = 1.0f;
    private Color _drowColor;
    private Color _endColor;

	private float _delayedTime = 0.0f;
	private Color _oroginalColor = Color.white;
	private int _inOutCount = 0;

	// lunafish for differant
	private float _startSpeed = 0.0f;
	private float _endspeed = 0.0f;

    void Awake()
    {
		_CImgTk2d = GetComponent("tk2dSprite") as tk2dSprite;
		_oroginalColor = getColor();

		sColor = _oroginalColor;
		switch(_startFade){
		case fadeControlAni.view:
			sColor.a = 1;
			break;
		case fadeControlAni.halfView:
			sColor.a = 0.5f;
			break;
		case fadeControlAni.hide:
			sColor.a = 0;
			break;
		}
		overWriteColor(sColor);
		this.enabled = false;
    }

	/*
	public void StartSetFadeValue(bool autoFD = false){
		
		_autoFadeINOUT = autoFD;
		_FadeInOutAni = false;
		_oroginalColor = getColor();
		if(_autoFadeINOUT) startFadeInAndOut(_autoFadeSpeed, _autoFadeDelaySpeed);
		else stopFadeAni(true);
	}*/

	private bool startFadeIn_OR_Out(float speed, Color startColor, Color endColor)
    {
        _FadeInOutAni = false;
        if (speed > 0)
        {
            overWriteColor(startColor);
            _endColor = endColor;
            _drowColor = (endColor - startColor) / speed;

            _FadeInOutAni = true;
            _startFTime = 0;
            _drowFSpeed = speed;
			this.enabled = true;

            return true;
        }

        overWriteColor(endColor);
        return false;
    }

	private Color sColor = Color.white;
	private Color eColor = Color.white;
	public bool startFadeIn_OR_Out(fadeControlAni startFade, fadeControlAni endFade, float speed)
    {
        sColor = _oroginalColor;
        eColor = _oroginalColor;

		switch(startFade){
		case fadeControlAni.view:
			sColor.a = 1;
			break;
		case fadeControlAni.halfView:
			sColor.a = 0.5f;
			break;
		case fadeControlAni.hide:
			sColor.a = 0;
			break;
		}

		switch(endFade){
		case fadeControlAni.view:
			eColor.a = 1;
			break;
		case fadeControlAni.halfView:
			eColor.a = 0.5f;
			break;
		case fadeControlAni.hide:
			eColor.a = 0;
			break;
		}

		_delayedTime = _inOutCount = 0;
        return startFadeIn_OR_Out(speed, sColor, eColor);
    }

	public void startFadeInAndOut(float speed, fadeControlAni startFade, fadeControlAni endFade, float delayedTime = 0, int drowCount = 1){
		startFadeIn_OR_Out(startFade, endFade, speed/2);
		_delayedTime = delayedTime;
		_inOutCount = drowCount;
		_delayStart = false;
		_swapSpeedControl = false;

		_startSpeed = _endspeed = speed/2;
	}

	private bool _swapSpeedControl = false;
	// differant start speed and end speed
	public void startFadeInAndOut(float startSpeed, float endSpeed, fadeControlAni startFade, fadeControlAni endFade, float delayedTime = 0, int drowCount = 1){
		startFadeIn_OR_Out(startFade, endFade, startSpeed);
		_delayedTime = delayedTime;
		_inOutCount = drowCount;
		_delayStart = false;
		_swapSpeedControl = false;

		_startSpeed = startSpeed;
		_endspeed = endSpeed;
	}

	/*
	public void stopFadeAni(bool endColor){
		if(nullChackControl()) return;
		
		_delayedTime = 0;
		_FadeInOutAni = false;
		InOutStart = false;
		InOutPosition = false;
		overWriteColor(endColor ? _oroginalColor : Color.clear);
	}*/

	/*
    public bool animationChack()
    {
        return _FadeInOutAni;
    }
	
	protected virtual bool nullChackControl(){
		return (_CImgTk2d == null);
	}*/
	
	protected virtual Color getColor(){
		return _CImgTk2d.color;
	}
	
	void addColorControl(Color addColor){
		_CImgTk2d.color += addColor;
	}
	
	void overWriteColor(Color overColor){
		_CImgTk2d.color = overColor;
	}

	private bool _delayStart = false;
	void Update () {
        if (_FadeInOutAni)
        {
			if(_delayStart){
				if((_startFTime += Time.deltaTime) >= _delayedTime){
					_startFTime = 0;
					_delayStart = false;
				} else return;
			}

			if ((_startFTime += Time.deltaTime) >= _drowFSpeed)
            {
				if(!_swapSpeedControl) _drowFSpeed = _endspeed; // setting end speed
				else _drowFSpeed = _startSpeed;
				_swapSpeedControl = !_swapSpeedControl;

				overWriteColor(_endColor);
				this.enabled = _FadeInOutAni = false;	
				if(_inOutCount > 0 || _inOutCount < 0){
					_inOutCount--;
					_startFTime = 0;
					if(_delayedTime > 0) _delayStart = true;

					Color copyColor = sColor;
					sColor = eColor;
					eColor = copyColor;
					startFadeIn_OR_Out(_drowFSpeed, sColor, eColor);
				}else _inOutCount = 0;
            }
            else addColorControl(_drowColor * Time.deltaTime);
        }
	}
}
