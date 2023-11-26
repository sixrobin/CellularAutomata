namespace CellularAutomata
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public abstract class CellularAutomaton : MonoBehaviour
    {
        private const string KERNEL_NAME_INIT = "Init";
        private const string KERNEL_NAME_NEXT = "Next";
        private const string KERNEL_NAME_APPLY_BUFFER = "ApplyBuffer";
        
        private const string RESOLUTION_ID = "_Resolution";
        protected const string RULES_ID = "_Rules";

        protected const string INIT_RANDOM_STEP_ID = "_InitRandomStep";
        protected const string INIT_CENTER_WIDTH_ID = "_InitCenterWidth";

        protected static readonly int RESOLUTION_SHADER_ID = Shader.PropertyToID(RESOLUTION_ID);
        private static readonly int RAMP_SHADER_ID = Shader.PropertyToID("_Ramp");
        
        [SerializeField]
        protected ComputeShader _computeShader;
        [SerializeField]
        protected Resolution _resolution = CellularAutomata.Resolution._32;
        [SerializeField]
        protected string _rules = "1/1/2/M";
        [SerializeField, Min(0f)]
        private float _iterationDelay = 0.1f;
        [SerializeField]
        private Gradient _gradient;

        protected int _initKernelIndex;
        protected int _nextKernelIndex;
        protected int _applyBufferKernelIndex;

        private float _iterationTimer;
        protected int _threadGroups;
        private Texture2D _ramp;

        protected int Resolution => (int)this._resolution;

        protected virtual void Init()
        {
            this._threadGroups = Resolution / 8;
            this._computeShader.SetFloat(RESOLUTION_ID, this.Resolution);

            this._initKernelIndex = this._computeShader.FindKernel(KERNEL_NAME_INIT);
            this._nextKernelIndex = this._computeShader.FindKernel(KERNEL_NAME_NEXT);
            this._applyBufferKernelIndex = this._computeShader.FindKernel(KERNEL_NAME_APPLY_BUFFER);
        }
        
        protected abstract void Next();

        protected virtual void Draw()
        {
        }

        protected void InitRules()
        {
            (int surviveRule, int birthRule, int cellStatesRule, int neighbourhoodRule) = RulesParser.ParseRuleset(this._rules);
            this._computeShader.SetInts(RULES_ID, surviveRule, birthRule, cellStatesRule, neighbourhoodRule);
        }

        protected void InitRampTexture(Material targetMaterial)
        {
            this._ramp = new Texture2D(32, 1, TextureFormat.RGBAFloat, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
            };

            for (int x = 0; x < this._ramp.width; ++x)
                this._ramp.SetPixel(x, 0, this._gradient.Evaluate(x / (float)this._ramp.width));

            this._ramp.Apply();
            targetMaterial.SetTexture(RAMP_SHADER_ID, this._ramp);
        }

        #region UNITY METHODS
        protected virtual void Start()
        {
            this.Init();
        }
        
        protected virtual void Update()
        {
            this._iterationTimer += Time.deltaTime;
            if (this._iterationTimer > this._iterationDelay)
            {
                this.Next();
                this._iterationTimer = 0f;
            }
            
            this.Draw();
        }
        #endregion // UNITY METHODS
    }
}