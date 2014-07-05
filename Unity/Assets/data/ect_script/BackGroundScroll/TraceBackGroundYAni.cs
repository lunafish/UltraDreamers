using UnityEngine;
using System.Collections;

public class TraceBackGroundYAni : MonoBehaviour
{
	public float texOffsetY = 0;
    public float scrollSpeed = 1f;
	public Transform _traceTFValue;
	private float _traceYPosition = 0;

    private Material _material;
    private Vector2 _texOffset;

    void Awake()
    {
        _material = gameObject.renderer.material;
        _texOffset = _material.GetTextureOffset("_MainTex");
    }
	
    // Update is called once per frame
    void Update()
    {
		float yPosition = _traceTFValue.localPosition.y;
		if(_traceYPosition == yPosition) return;
		_traceYPosition = yPosition;
		texOffsetY = _texOffset.y = (scrollSpeed * yPosition);
		_material.SetTextureOffset("_MainTex", _texOffset);
    }
}
