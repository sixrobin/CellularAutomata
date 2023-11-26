using UnityEngine;

public class ComputeShaderLoop : MonoBehaviour
{
    private static readonly int RESOLUTION_ID = Shader.PropertyToID("_Resolution");
    private static readonly int CUBES_ID = Shader.PropertyToID("_Cubes");
    private static readonly int RULES_ID = Shader.PropertyToID("_Rules");
    
    public struct Cube
    {
        public const int BUFFER_SIZE = sizeof(float) * 6
                                       + sizeof(int) * 1;
        
        public Vector3 Position;
        public Vector3 Color;
        public int State;
    }

    [System.Flags]
    public enum E_InitializationMethod
    {
        NONE = 0,
        CENTER_CELL = 1 << 0,
        RANDOM_STEP = 1 << 1,
    }
    
    [SerializeField]
    private ComputeShader _computeShader;
    [SerializeField]
    private int _resolution = 8;
    [SerializeField]
    private string _rules = "B/S";
    [SerializeField]
    private E_InitializationMethod _initializationMethod = E_InitializationMethod.RANDOM_STEP;
    [SerializeField, Min(0)]
    private int _initCenterWidth = 4;
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

        this._computeShader.Dispatch(3, groups, groups, groups);
        this._computeShader.Dispatch(2, groups, groups, groups);
        this._computeShader.Dispatch(4, groups, groups, groups);
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
        (int surviveRule, int birthRule, int cellStatesRule, int neighbourhoodRule) = CellularAutomata.RulesParser.ParseRuleset(this._rules, log: true);
        this._computeShader.SetInts("_Rules", surviveRule, birthRule, cellStatesRule, neighbourhoodRule);
        
        this._computeShader.SetBuffer(0, "_Cubes", this._cubes);
        
        this._computeShader.SetBuffer(2, "_Cubes", this._cubes);
        this._computeShader.SetBuffer(2, "_CubesBuffer", this._cubesBuffer);
        this._computeShader.SetBuffer(3, "_Cubes", this._cubes);
        this._computeShader.SetBuffer(3, "_CubesBuffer", this._cubesBuffer);
        this._computeShader.SetBuffer(4, "_Cubes", this._cubes);
        this._computeShader.SetBuffer(4, "_CubesBuffer", this._cubesBuffer);
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
        this._computeShader.SetFloat("_InitRandomStep", this._initializationMethod.HasFlag(E_InitializationMethod.RANDOM_STEP) ? this._initRandomStep : 1f);
        this._computeShader.SetInt("_InitCenterWidth", this._initializationMethod.HasFlag(E_InitializationMethod.CENTER_CELL) ? this._initCenterWidth : this._resolution);
        
        int groups = Mathf.CeilToInt(this._resolution / 8f);
        this._computeShader.Dispatch(0, groups, groups, groups);
        
        this._material.SetBuffer(CUBES_ID, this._cubes);
        this._material.SetFloat(RESOLUTION_ID, this._resolution);
        
        (int surviveRule, int birthRule, int cellStatesRule, int neighbourhoodRule) = CellularAutomata.RulesParser.ParseRuleset(this._rules, log: true);
        this._material.SetVector(RULES_ID, new Vector4(surviveRule, birthRule, cellStatesRule, neighbourhoodRule));
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
