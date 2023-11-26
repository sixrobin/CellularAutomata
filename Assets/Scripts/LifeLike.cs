namespace CellularAutomata
{
    using UnityEngine;

    public class LifeLike : CellularAutomaton2D
    {
        [SerializeField]
        private string _rules;

        [SerializeField, Range(0f, 1f)]
        private float _randomStep = 0.5f;

        protected override void Init()
        {
            base.Init();

            int groups = this.Resolution / 8;

            (int surviveRule, int birthRule, _, _) = RulesParser.ParseRuleset(this._rules);
            this._computeShader.SetInts("_Rules", surviveRule, birthRule);

            this._computeShader.SetFloat("_InitRandomStep", this._randomStep);
            
            this._computeShader.SetTexture(0, "_Result", this._gridBuffer);
            this._computeShader.Dispatch(0, groups, groups, 1);

            this._computeShader.SetTexture(2, "_Result", this._grid);
            this._computeShader.SetTexture(2, "_GridBuffer", this._gridBuffer);
            this._computeShader.Dispatch(2, groups, groups, 1);
        }

        protected override void Next()
        {
            base.Next();

            this._computeShader.SetTexture(1, "_Result", this._grid);
            this._computeShader.SetTexture(1, "_GridBuffer", this._gridBuffer);

            int groups = this.Resolution / 8;
            this._computeShader.Dispatch(1, groups, groups, 1);

            this.ApplyTextureBuffer();
        }
    }
}