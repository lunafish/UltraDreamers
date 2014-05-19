using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class CleanPlane : MonoBehaviour
{
	public enum vertical_Alignment
    {
        left,
        center,
        right,
		custom
    }
    public enum Horizontal_Alignment
    {
        top,
        center,
        bottom,
		custom
    }
	
	public enum shaderSelect{
		alpahBlended,
		transparendColored,
		softEdgeUnlit
	}

	public enum SelectCamera
	{
		Camera_Main,
		Camera_FullSize
	}

	public bool _customRotate = false;
	public bool _swapScale = false;
	public Vector2 _swapScaleChac = Vector3.zero;
	
	public vertical_Alignment vAlign = vertical_Alignment.center;
    public Horizontal_Alignment hAlign = Horizontal_Alignment.center;
	public shaderSelect shader = shaderSelect.transparendColored;
	public SelectCamera cameraV = SelectCamera.Camera_Main;
	
	public Vector2[] _scaleValue = { Vector2.one };
	public Texture _originalTexture = null;
	private Material _sharedMaterial = null;
	private float controlGuidSize = 640.0f;
	
    void Awake(){

		setShaderMT();
		switch(cameraV){
		case SelectCamera.Camera_Main:
			controlGuidSize = 640.0f;
			break;
		case SelectCamera.Camera_FullSize:
			controlGuidSize = Screen.height;
			break;
		}
		
		
		if(!_customRotate) transform.localEulerAngles = new Vector3(-90, 0,0);
        GetComponent<MeshFilter>().mesh = CreatePlaneMesh();
		setScaleValue(_originalTexture);
    }

	bool setShaderMT(){
		if(_sharedMaterial != null) return false;

		Shader saveShader = Shader.Find ("Unlit/Transparent Colored");
		switch (shader)
		{
		case shaderSelect.alpahBlended:
			saveShader = Shader.Find ("Particles/Alpha Blended");
			break;
		case shaderSelect.softEdgeUnlit:
			saveShader = Shader.Find("Transparent/Cutout/Soft Edge Unlit");
			break;
		}
		
		_sharedMaterial = renderer.material = new Material( saveShader );
		_sharedMaterial.mainTexture = _originalTexture;
		return true;
	}
	
	void setScaleValue(Texture chackTexture){
		int selectS = _scaleValue.Length > 1 ? (Camera_Main.iphone4_5 ? 1: 0) : 0;
		float xScale = 0;
		float zScale = 0;
		if(_swapScale){
			xScale = _swapScaleChac.x;
			zScale = _swapScaleChac.y;
		}else{
			xScale = chackTexture.width/controlGuidSize * _scaleValue[selectS].x;
			zScale = chackTexture.height/controlGuidSize * _scaleValue[selectS].y;
		}
		transform.localScale = new Vector3(xScale,1, zScale);
		
		Vector3 drowVector = transform.localPosition;
        switch (vAlign)
        {
            case vertical_Alignment.left:
                drowVector.x = xScale;
                break;
            case vertical_Alignment.center:
                drowVector.x = 0;
                break;
            case vertical_Alignment.right:
				drowVector.x = 0 - xScale;
                break;
        }

        switch (hAlign)
        {
            case Horizontal_Alignment.top:
                drowVector.y = 0 - zScale;
                break;
            case Horizontal_Alignment.center:
                drowVector.y = 0;
                break;
            case Horizontal_Alignment.bottom:
                drowVector.y = zScale;
                break;
        }
		
		transform.localPosition = drowVector;
	}
	
	/*
	void OnDestroy ()
	{
		DestroyImmediate(renderer.sharedMaterial);
	}*/
	
	public void chacngeMainTexture(Texture ChangeTexture){
		if(ChangeTexture == null) return;
		setShaderMT();
	
		_sharedMaterial.mainTexture = ChangeTexture;
		setScaleValue(ChangeTexture);
	}
	
	public void resetMainTexture(){
		if(!setShaderMT()) 
			_sharedMaterial.mainTexture = _originalTexture;
	}
	
	public void AlphaControl(float alpha){
		setShaderMT();
		_sharedMaterial.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, alpha *0.5f));
	}
	/*
	public void setGradientColor(){

		Mesh mesh  = GetComponent<MeshFilter>().mesh;
		Vector2[] uv = mesh.uv;
		Color[] colors = new Color[uv.Length];

		Color[] selectColor = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.gray, Color.black};
		for (var i = 0; i < uv.Length;i++)
			colors[i] = Color.Lerp(new Color(UnityEngine.Random.Range(0,1.0f),UnityEngine.Random.Range(0,1.0f),UnityEngine.Random.Range(0,1.0f)), 
			                       new Color(UnityEngine.Random.Range(0,1.0f),UnityEngine.Random.Range(0,1.0f),UnityEngine.Random.Range(0,1.0f)), 
			                       uv[i].x); 
			//colors[i] = Color.Lerp(selectColor[UnityEngine.Random.Range(0,8)], selectColor[UnityEngine.Random.Range(0,8)], uv[i].x); 
		
		mesh.colors = colors;
	}*/
	
    Mesh CreatePlaneMesh()
    {
        Mesh mesh = new Mesh();
 
        Vector3[] vertices = new Vector3[]
        {
            new Vector3( 1, 0,  1),
            new Vector3( 1, 0, -1),
            new Vector3(-1, 0,  1),
            new Vector3(-1, 0, -1),
        };
 
        Vector2[] uv = new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 0),
        };
 
        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3,
        };
 
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
 
        return mesh;
    }
}
