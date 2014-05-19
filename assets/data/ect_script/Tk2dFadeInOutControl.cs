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

	private bool InOutStart = false;
	private int _inOutCount = 0;

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

	private bool startFadeIn_OR_Out(float speed)
	{
		Color copyColor = sColor;
		sColor = eColor;
		eColor = copyColor;
		
		return startFadeIn_OR_Out(speed, sColor, eColor);
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
		InOutStart = false;
        return startFadeIn_OR_Out(speed, sColor, eColor);
    }

	public void startFadeInAndOut(float speed, fadeControlAni startFade, fadeControlAni endFade, float delayedTime = 0, int drowCount = 1){
		InOutStart = true;

		startFadeIn_OR_Out(startFade, endFade, speed/2);
		_delayedTime = delayedTime + _drowFSpeed;
		_inOutCount = drowCount;
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
	
	void Update () {
        if (_FadeInOutAni)
        {
            if ((_startFTime += Time.deltaTime) >= _drowFSpeed)
            {
				overWriteColor(_endColor);
                if(!InOutStart) {
					this.enabled = _FadeInOutAni = false;	
					if(_inOutCount > 0 || _inOutCount < 0){
						_inOutCount--;
						startFadeIn_OR_Out(_drowFSpeed);
					}else _inOutCount = 0;
				}
                else if(_startFTime > _delayedTime){
					startFadeIn_OR_Out(_drowFSpeed);
					InOutStart = false;
				}
            }
            else addColorControl(_drowColor * Time.deltaTime);
        }
	}
}
