namespace CellularAutomata
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public abstract class CellularAutomaton : MonoBehaviour
    {
        protected const string RESOLUTION_ID = "_Resolution";
        protected static readonly int RESOLUTION_SHADER_ID = Shader.PropertyToID(RESOLUTION_ID);
        
        protected const string RULES_ID = "_Rules";
        protected static readonly int RULES_SHADER_ID = Shader.PropertyToID(RULES_ID);

        protected const string INIT_RANDOM_STEP_ID = "_InitRandomStep";

        [SerializeField]
        protected ComputeShader _computeShader;
        [SerializeField]
        protected Resolution _resolution = CellularAutomata.Resolution._32;
        [SerializeField]
        protected string _rules = "1/1/2/M";
        [SerializeField, Min(0f)]
        private float _iterationDelay = 0.1f;

        private float _iterationTimer;
        
        protected int Resolution => (int)this._resolution;

        protected abstract void Init();
        protected abstract void Next();

        protected void InitRules()
        {
            (int surviveRule, int birthRule, int cellStatesRule, int neighbourhoodRule) = CellularAutomata.RulesParser.ParseRuleset(this._rules, log: true);
            this._computeShader.SetInts(RULES_ID, surviveRule, birthRule, cellStatesRule, neighbourhoodRule);
        }

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
        }
    }
}