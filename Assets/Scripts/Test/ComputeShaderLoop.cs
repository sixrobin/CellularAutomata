using UnityEngine;

public class ComputeShaderLoop : MonoBehaviour
{
    public struct Cube
    {
        public const int BUFFER_SIZE = sizeof(float) * 7;
        
        public Vector3 Position;
        public Vector3 Color;
        public float State;
    }
    
    [SerializeField]
    private ComputeShader _computeShader;
    [SerializeField]
    private int _resolution = 8;

    [SerializeField]
    private Material _material;
    [SerializeField]
    private Mesh _mesh;
    
    private ComputeBuffer _cubesBuffer;
    
    private void OnEnable()
    {
        Cube[] cubes = new Cube[this._resolution * this._resolution * this._resolution];

        for (int x = 0; x < this._resolution; ++x)
            for (int y = 0; y < this._resolution; ++y)
                for (int z = 0; z < this._resolution; ++z)
                    cubes[x + y * this._resolution + z * this._resolution * this._resolution] = new Cube();

        this._cubesBuffer = new ComputeBuffer(cubes.Length, Cube.BUFFER_SIZE);
    }

    private void OnDisable()
    {
        this._cubesBuffer.Release();
        this._cubesBuffer = null;
    }

    private void Start()
    {
        this._computeShader.SetBuffer(0, "_Cubes", this._cubesBuffer);
        int groups = Mathf.CeilToInt(this._resolution / 8f);
        this._computeShader.Dispatch(0, groups, groups, groups);
    }

    private void Update()
    {
        this._computeShader.SetBuffer(1, "_Cubes", this._cubesBuffer);
        this._computeShader.SetFloat("_Resolution", this._resolution);
        int groups = Mathf.CeilToInt(this._resolution / 8f);
        
        this._computeShader.Dispatch(1, groups, groups, groups);

        this._material.SetBuffer("_Cubes", this._cubesBuffer);
        this._material.SetFloat("_Step", 1f);
        this._material.SetFloat("_Resolution", this._resolution);
        
        Bounds bounds = new(Vector3.zero, Vector3.one * this._resolution);
        Graphics.DrawMeshInstancedProcedural(this._mesh, 0, this._material, bounds, this._cubesBuffer.count);
    }
}
