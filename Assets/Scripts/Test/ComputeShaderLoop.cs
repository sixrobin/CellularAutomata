using UnityEngine;

public class ComputeShaderLoop : MonoBehaviour
{
    public struct Cube
    {
        public const int BUFFER_SIZE = sizeof(float) * 6;
        
        public Vector3 Position;
        public Vector3 Color;
    }
    
    [SerializeField]
    private ComputeShader _computeShader;
    [SerializeField]
    private TestCube[] _testCubes;

    [SerializeField]
    private Material _material;
    [SerializeField]
    private Mesh _mesh;
    
    private ComputeBuffer _cubesBuffer;
    
    private void OnEnable()
    {
        Cube[] cubes = new Cube[this._testCubes.Length];
        for (int i = 0; i < cubes.Length; ++i)
        {
            cubes[i] = new Cube
            {
                Position = this._testCubes[i].transform.position,
                Color = new Vector3(Color.white.r, Color.white.g, Color.white.b),
            };
        }
        
        _cubesBuffer = new ComputeBuffer(this._testCubes.Length, Cube.BUFFER_SIZE);
    }

    private void OnDisable()
    {
        this._cubesBuffer.Release();
        this._cubesBuffer = null;
    }

    private void Update()
    {
        this._computeShader.SetBuffer(0, "_Cubes", this._cubesBuffer);
        this._computeShader.SetFloat("_Resolution", _testCubes.Length);
        this._computeShader.Dispatch(0, 1, 1, 1);
        
        this._material.SetBuffer("_Cubes", this._cubesBuffer);
        this._material.SetFloat("_Step", 1f);
        
        Bounds bounds = new(Vector3.zero, Vector3.one * 10f);
        Graphics.DrawMeshInstancedProcedural(this._mesh, 0, this._material, bounds, this._cubesBuffer.count);
    }
}
