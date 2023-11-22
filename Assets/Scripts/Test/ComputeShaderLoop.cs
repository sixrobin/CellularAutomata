using UnityEngine;

public class ComputeShaderLoop : MonoBehaviour
{
    private static readonly int RESOLUTION_ID = Shader.PropertyToID("_Resolution");
    private static readonly int STEP_ID = Shader.PropertyToID("_Step");
    private static readonly int CUBES_ID = Shader.PropertyToID("_Cubes");
    
    public struct Cube
    {
        public const int BUFFER_SIZE = sizeof(float) * 7;
        
        public Vector3 Position;
        public Vector3 Color;
        public float State;
    }

    public enum E_InitializationMethod
    {
        CENTER_CELL,
        RANDOM_STEP,
    }
    
    [SerializeField]
    private ComputeShader _computeShader;
    [SerializeField]
    private int _resolution = 8;
    [SerializeField]
    private string _rules = "B/S";
    [SerializeField]
    private E_InitializationMethod _initializationMethod = E_InitializationMethod.RANDOM_STEP;
    [SerializeField, Range(0f, 1f)]
    private float _initRandomStep = 0.5f;
    [SerializeField]
    private float _updateTimeInterval = 1f;

    [SerializeField]
    private Material _material;
    [SerializeField]
    private Mesh _mesh;
    
    private ComputeBuffer _cubes;
    private ComputeBuffer _cubesBuffer;
    private float _timer;

    [ContextMenu("Next")]
    private void Next()
    {
        int groups = Mathf.CeilToInt(this._resolution / 8f);

        this._computeShader.Dispatch(4, groups, groups, groups);
        this._computeShader.Dispatch(3, groups, groups, groups);
        this._computeShader.Dispatch(5, groups, groups, groups);
    }
    
    private void OnEnable()
    {
        Cube[] cubes = new Cube[this._resolution * this._resolution * this._resolution];

        for (int x = 0; x < this._resolution; ++x)
            for (int y = 0; y < this._resolution; ++y)
                for (int z = 0; z < this._resolution; ++z)
                    cubes[x + y * this._resolution + z * this._resolution * this._resolution] = new Cube();

        this._cubes = new ComputeBuffer(cubes.Length, Cube.BUFFER_SIZE);
        this._cubesBuffer = new ComputeBuffer(cubes.Length, Cube.BUFFER_SIZE);
        
        this._computeShader.SetFloat("_Resolution", this._resolution);
        this._computeShader.SetFloat("_InitRandomStep", this._initRandomStep);
        Vector2Int rules = CellularAutomata.RulesParser.ParseRuleset(this._rules);
        this._computeShader.SetInts("_Rules", rules.x, rules.y);
        
        this._computeShader.SetBuffer(0, "_Cubes", this._cubes);
        this._computeShader.SetBuffer(1, "_Cubes", this._cubes);
        
        this._computeShader.SetBuffer(3, "_Cubes", this._cubes);
        this._computeShader.SetBuffer(3, "_CubesBuffer", this._cubesBuffer);
        this._computeShader.SetBuffer(4, "_Cubes", this._cubes);
        this._computeShader.SetBuffer(4, "_CubesBuffer", this._cubesBuffer);
        this._computeShader.SetBuffer(5, "_Cubes", this._cubes);
        this._computeShader.SetBuffer(5, "_CubesBuffer", this._cubesBuffer);
    }

    private void OnDisable()
    {
        this._cubes.Release();
        this._cubes = null;
        
        this._cubesBuffer.Release();
        this._cubesBuffer = null;
    }

    private void Start()
    {
        int groups = Mathf.CeilToInt(this._resolution / 8f);

        switch (this._initializationMethod)
        {
            case E_InitializationMethod.CENTER_CELL:
                this._computeShader.Dispatch(1, groups, groups, groups);
                break;
            case E_InitializationMethod.RANDOM_STEP:
                this._computeShader.Dispatch(0, groups, groups, groups);
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
        
        this._material.SetBuffer(CUBES_ID, this._cubes);
        this._material.SetFloat(RESOLUTION_ID, this._resolution);
    }

    private void Update()
    {
        this._timer += Time.deltaTime;
        if (this._timer > this._updateTimeInterval)
        {
            this._timer = 0f;
            this.Next();
        }
        
        Bounds bounds = new(Vector3.zero, Vector3.one * this._resolution);
        Graphics.DrawMeshInstancedProcedural(this._mesh, 0, this._material, bounds, this._cubes.count);
    }
}
