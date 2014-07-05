using UnityEngine;
using System.Collections;

public class BackGroundAni : MonoBehaviour
{

    public float scrollSpeed = 0.1f;
    private Material _material;
    private Vector2 _texOffset;

    void Start()
    {
        _material = gameObject.renderer.material;
        _texOffset = _material.GetTextureOffset("_MainTex");
    }
	
    // Update is called once per frame
    void Update()
    {
		controlLRAni();
    }

	void controlLRAni(){
		_texOffset.y += (Time.deltaTime * scrollSpeed);
		_material.SetTextureOffset("_MainTex", _texOffset);
	}
}
