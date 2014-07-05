using UnityEngine;
using System.Collections;

public class TraceBackGroundXAni : MonoBehaviour
{
    public float scrollSpeed = 1f;
	public Transform _traceTFValue;
	private float _traceYPosition = 0;

    private Material _material;
    private Vector2 _texOffset;
	
	void Start(){
		_material = gameObject.renderer.material;
		_texOffset = _material.GetTextureOffset("_MainTex");
	}
	
    // Update is called once per frame
    void Update()
    {
		float xPosition = _traceTFValue.localPosition.x;
		if(_traceYPosition == xPosition) return;
		_traceYPosition = xPosition;
		_texOffset.x = (scrollSpeed * xPosition);
		_material.SetTextureOffset("_MainTex", _texOffset);
    }
}
