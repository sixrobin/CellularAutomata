namespace CellularAutomata
{
    using UnityEngine;

    public class LifeLike : CellularAutomaton
    {
        [SerializeField]
        private string _rules;

        [SerializeField, Range(0f, 1f)]
        private float _randomStep = 0.5f;

        protected override void Init()
        {
            base.Init();

            Vector2Int rules = RulesParser.ParseRuleset(this._rules);
            
            this._computeShader.SetInts("Rules", rules.x, rules.y);

            this._computeShader.SetFloat("InitRandomStep", this._randomStep);
            this._computeShader.SetTexture(0, "Result", this._gridBuffer);
            this._computeShader.Dispatch(0, this.Resolution / 8, this.Resolution / 8, 1);

            this._computeShader.SetTexture(2, "Result", this._grid);
            this._computeShader.SetTexture(2, "GridBuffer", this._gridBuffer);
            this._computeShader.Dispatch(2, this.Resolution / 8, this.Resolution / 8, 1);
        }

        protected override void Next()
        {
            base.Next();

            this._computeShader.SetTexture(1, "Result", this._grid);
            this._computeShader.SetTexture(1, "GridBuffer", this._gridBuffer);
            this._computeShader.Dispatch(1, this.Resolution / 8, this.Resolution / 8, 1);

            this.ApplyTextureBuffer();
        }
    }
}