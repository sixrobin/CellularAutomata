namespace CellularAutomata
{
    using UnityEngine;

    public class CellularAutomaton3D : CellularAutomaton
    {
        private const string CUBES_ID = "_Cubes";
        private const string CUBES_BUFFER_ID = "_CubesBuffer";
        private static readonly int CUBES_SHADER_ID = Shader.PropertyToID("_Cubes");

        public struct Cube
        {
            public const int BUFFER_SIZE = sizeof(float) * 3 + sizeof(int) * 1;

            public Vector3 Position;
            public int State;
        }

        [SerializeField]
        private InitializationMethod _initializationMethod = InitializationMethod.RANDOM_STEP;

        [SerializeField, Min(0)]
        private int _initCenterWidth = 4;

        [SerializeField, Range(0f, 1f)]
        private float _initRandomStep = 0.5f;

        [SerializeField]
        private Material _material;

        [SerializeField]
        private Mesh _mesh;

        private ComputeBuffer _cubes;
        private ComputeBuffer _cubesBuffer;

        protected override void Init()
        {
            this._computeShader.SetFloat(INIT_RANDOM_STEP_ID, this._initializationMethod.HasFlag(InitializationMethod.RANDOM_STEP) ? this._initRandomStep : 1f);
            this._computeShader.SetInt("_InitCenterWidth", this._initializationMethod.HasFlag(InitializationMethod.CENTER_CELLS) ? this._initCenterWidth : this.Resolution);

            int groups = Mathf.CeilToInt(this.Resolution / 8f);
            this._computeShader.Dispatch(0, groups, groups, groups);

            this._material.SetBuffer(CUBES_SHADER_ID, this._cubes);
            this._material.SetFloat(RESOLUTION_SHADER_ID, this.Resolution);

            (int surviveRule, int birthRule, int cellStatesRule, int neighbourhoodRule) = CellularAutomata.RulesParser.ParseRuleset(this._rules, log: true);
            this._material.SetVector(RULES_ID, new Vector4(surviveRule, birthRule, cellStatesRule, neighbourhoodRule));
        }
        
        [ContextMenu("Next")]
        protected override void Next()
        {
            int groups = Mathf.CeilToInt(this.Resolution / 8f);

            this._computeShader.Dispatch(2, groups, groups, groups); // PrepareBuffer.
            this._computeShader.Dispatch(1, groups, groups, groups); // Next.
            this._computeShader.Dispatch(3, groups, groups, groups); // ApplyBuffer.
        }

        private void OnEnable()
        {
            Cube[] cubes = new Cube[this.Resolution * this.Resolution * this.Resolution];

            for (int x = 0; x < this.Resolution; ++x)
                for (int y = 0; y < this.Resolution; ++y)
                    for (int z = 0; z < this.Resolution; ++z)
                        cubes[x + y * this.Resolution + z * this.Resolution * this.Resolution] = new Cube();

            this._cubes = new ComputeBuffer(cubes.Length, Cube.BUFFER_SIZE);
            this._cubesBuffer = new ComputeBuffer(cubes.Length, Cube.BUFFER_SIZE);

            this._computeShader.SetFloat(RESOLUTION_ID, this.Resolution);
            this.InitRules();

            this._computeShader.SetBuffer(0, CUBES_ID, this._cubes);
            this._computeShader.SetBuffer(1, CUBES_ID, this._cubes);
            this._computeShader.SetBuffer(1, CUBES_BUFFER_ID, this._cubesBuffer);
            this._computeShader.SetBuffer(2, CUBES_ID, this._cubes);
            this._computeShader.SetBuffer(2, CUBES_BUFFER_ID, this._cubesBuffer);
            this._computeShader.SetBuffer(3, CUBES_ID, this._cubes);
            this._computeShader.SetBuffer(3, CUBES_BUFFER_ID, this._cubesBuffer);
        }

        private void OnDisable()
        {
            this._cubes.Release();
            this._cubes = null;

            this._cubesBuffer.Release();
            this._cubesBuffer = null;
        }
        
        protected override void Update()
        {
            base.Update();

            Bounds bounds = new(Vector3.zero, Vector3.one * this.Resolution);
            Graphics.DrawMeshInstancedProcedural(this._mesh, 0, this._material, bounds, this._cubes.count);
        }
    }
}