namespace CellularAutomata
{
    using UnityEngine;

    public abstract class CellularAutomaton2D : CellularAutomaton
    {
        protected const string RESULT_ID = "_Result";
        protected const string GRID_BUFFER_ID = "_GridBuffer";
        private const string DECAY_STEP_ID = "_DecayStep";
        private static readonly int MAIN_TEX_SHADER_ID = Shader.PropertyToID("_MainTex");
        private static readonly int RAMP_SHADER_ID = Shader.PropertyToID("_Ramp");
        
        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private TextureWrapMode _wrapMode = TextureWrapMode.Repeat;

        [SerializeField, Range(0.001f, 0.1f)]
        private float _decayStep = 0.1f;

        [SerializeField]
        private Gradient _gradient;

        protected RenderTexture _result;
        protected RenderTexture _gridBuffer;
        private Texture2D _ramp;

        private RenderTexture CreateTexture()
        {
            RenderTexture texture = new RenderTexture(this.Resolution, this.Resolution, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Point,
                wrapMode = this._wrapMode,
            };

            texture.Create();
            return texture;
        }
        
        private Texture2D CreateRampTexture()
        {
            this._ramp = new Texture2D(32, 1, TextureFormat.RGBAFloat, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
            };

            for (int x = 0; x < this._ramp.width; ++x)
                this._ramp.SetPixel(x, 0, this._gradient.Evaluate(x / (float)this._ramp.width));

            this._ramp.Apply();
            return this._ramp;
        }

        protected override void Init()
        {
            this._result = this.CreateTexture();
            this._gridBuffer = this.CreateTexture();

            this._computeShader = Instantiate(this._computeShader); // Create a compute shader copy so that every instance can have its own parameters.
            this._computeShader.SetFloat(RESOLUTION_ID, this.Resolution);
            this._computeShader.SetFloat(DECAY_STEP_ID, this._decayStep);

            this._renderer.material.SetTexture(RAMP_SHADER_ID, this.CreateRampTexture());
            this._renderer.material.SetTexture(MAIN_TEX_SHADER_ID, this._result);
        }

        protected override void Next() { }

        protected void ApplyTextureBuffer()
        {
            int kernelIndex = this._computeShader.FindKernel("ApplyBuffer");
            
            this._computeShader.SetTexture(kernelIndex, RESULT_ID, this._gridBuffer);
            this._computeShader.SetTexture(kernelIndex, GRID_BUFFER_ID, this._result);
            this._computeShader.Dispatch(kernelIndex, this.Resolution / 8, this.Resolution / 8, 1);
        }

        #region UNITY METHODS
        private void Reset()
        {
            this._renderer = this.GetComponent<Renderer>();
        }
        #endregion // UNITY METHODS
    }
}