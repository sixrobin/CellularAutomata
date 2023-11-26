namespace CellularAutomata
{
    using UnityEngine;

    public class CellularAutomaton3D : CellularAutomaton
    {
        private const string KERNEL_NAME_PREPARE_BUFFER = "PrepareBuffer";
        private const string CUBES_ID = "_Cubes";
        private const string CUBES_BUFFER_ID = "_CubesBuffer";
        private static readonly int CUBES_SHADER_ID = Shader.PropertyToID("_Cubes");

        private struct Cube
        {
            public const int BUFFER_SIZE = sizeof(float) * 3 + sizeof(int);

            public Vector3 Position;
            public int State;
        }
        
        [SerializeField]
        private Material _material;
        [SerializeField]
        private Mesh _mesh;

        private ComputeBuffer _cubes;
        private ComputeBuffer _cubesBuffer;
        private int _prepareBufferKernelIndex;

        private void CreateBuffers()
        {
            Cube[] cubes = new Cube[this.Resolution * this.Resolution * this.Resolution];

            for (int x = 0; x < this.Resolution; ++x)
                for (int y = 0; y < this.Resolution; ++y)
                    for (int z = 0; z < this.Resolution; ++z)
                        cubes[x + y * this.Resolution + z * this.Resolution * this.Resolution] = new Cube();

            this._cubes = new ComputeBuffer(cubes.Length, Cube.BUFFER_SIZE);
            this._cubesBuffer = new ComputeBuffer(cubes.Length, Cube.BUFFER_SIZE);
        }

        private void ReleaseBuffers()
        {
            this._cubes.Release();
            this._cubes = null;
            this._cubesBuffer.Release();
            this._cubesBuffer = null;
        }
        
        protected override void Init()
        {
            base.Init();
            
            this._prepareBufferKernelIndex = this._computeShader.FindKernel(KERNEL_NAME_PREPARE_BUFFER);

            this._computeShader.SetBuffer(this._initKernelIndex, CUBES_ID, this._cubes);
            this._computeShader.SetBuffer(this._nextKernelIndex, CUBES_ID, this._cubes);
            this._computeShader.SetBuffer(this._nextKernelIndex, CUBES_BUFFER_ID, this._cubesBuffer);
            this._computeShader.SetBuffer(this._prepareBufferKernelIndex, CUBES_ID, this._cubes);
            this._computeShader.SetBuffer(this._prepareBufferKernelIndex, CUBES_BUFFER_ID, this._cubesBuffer);
            this._computeShader.SetBuffer(this._applyBufferKernelIndex, CUBES_ID, this._cubes);
            this._computeShader.SetBuffer(this._applyBufferKernelIndex, CUBES_BUFFER_ID, this._cubesBuffer);
            
            this.InitRules();
            this.InitRampTexture(this._material);

            this._computeShader.Dispatch(this._initKernelIndex, this._threadGroups, this._threadGroups, this._threadGroups);

            this._material.SetBuffer(CUBES_SHADER_ID, this._cubes);
            this._material.SetFloat(RESOLUTION_SHADER_ID, this.Resolution);

            (int surviveRule, int birthRule, int cellStatesRule, int neighbourhoodRule) = RulesParser.ParseRuleset(this._settings.Rules);
            this._material.SetVector(RULES_ID, new Vector4(surviveRule, birthRule, cellStatesRule, neighbourhoodRule));
        }
        
        [ContextMenu("Next")]
        protected override void Next()
        {
            this._computeShader.Dispatch(this._prepareBufferKernelIndex, this._threadGroups, this._threadGroups, this._threadGroups); // PrepareBuffer.
            this._computeShader.Dispatch(this._nextKernelIndex, this._threadGroups, this._threadGroups, this._threadGroups);
            this._computeShader.Dispatch(this._applyBufferKernelIndex, this._threadGroups, this._threadGroups, this._threadGroups);
        }

        protected override void Draw()
        {
            base.Draw();

            Bounds bounds = new(Vector3.zero, Vector3.one * this.Resolution);
            Graphics.DrawMeshInstancedProcedural(this._mesh, 0, this._material, bounds, this._cubes.count);
        }

        #region UNITY METHODS
        private void OnEnable()
        {
            this.CreateBuffers();
        }

        private void OnDisable()
        {
            this.ReleaseBuffers();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
            
            Vector3 size = Vector3.one * this.Resolution;
            Gizmos.DrawWireCube(size * 0.5f, size);
        }
        #endregion // UNITY METHODS
    }
}