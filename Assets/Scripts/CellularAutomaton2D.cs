namespace CellularAutomata
{
    using UnityEngine;

    public abstract class CellularAutomaton2D : CellularAutomaton
    {
        protected const string RESULT_ID = "_Result";
        protected const string GRID_BUFFER_ID = "_GridBuffer";
        private const string DECAY_STEP_ID = "_DecayStep";
        private static readonly int MAIN_TEX_SHADER_ID = Shader.PropertyToID("_MainTex");
        
        [SerializeField]
        private Renderer _renderer;
        [SerializeField]
        private TextureWrapMode _wrapMode = TextureWrapMode.Repeat;
        [SerializeField, Range(0.001f, 1f)]
        private float _decayStep = 0.1f;

        protected RenderTexture _result;
        protected RenderTexture _gridBuffer;

        private RenderTexture CreateTexture()
        {
            RenderTexture texture = new(this.Resolution, this.Resolution, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Point,
                wrapMode = this._wrapMode,
            };

            texture.Create();
            return texture;
        }

        protected override void Init()
        {
            this._computeShader = Instantiate(this._computeShader); // Create a compute shader copy so that every instance can have its own parameters.
            base.Init();
            this._computeShader.SetFloat(DECAY_STEP_ID, this._decayStep);

            this._result = this.CreateTexture();
            this._gridBuffer = this.CreateTexture();

            this.InitRampTexture(this._renderer.material);
            this._renderer.material.SetTexture(MAIN_TEX_SHADER_ID, this._result);
        }

        protected override void Next()
        {
        }

        protected void ApplyTextureBuffer()
        {
            this._computeShader.SetTexture(this._applyBufferKernelIndex, RESULT_ID, this._gridBuffer);
            this._computeShader.SetTexture(this._applyBufferKernelIndex, GRID_BUFFER_ID, this._result);
            this._computeShader.Dispatch(this._applyBufferKernelIndex, this._threadGroups, this._threadGroups, 1);
        }

        #region UNITY METHODS
        private void Reset()
        {
            this._renderer = this.GetComponent<Renderer>();
        }
        #endregion // UNITY METHODS
    }
}