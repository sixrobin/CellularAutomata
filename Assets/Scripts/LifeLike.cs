namespace CellularAutomata
{
    using UnityEngine;

    public class LifeLike : CellularAutomaton2D
    {
        [SerializeField, Range(0f, 1f)]
        private float _randomStep = 0.5f;

        protected override void Init()
        {
            base.Init();

            this.InitRules();

            this._computeShader.SetFloat(INIT_RANDOM_STEP_ID, this._randomStep);
            
            this._computeShader.SetTexture(this._initKernelIndex, RESULT_ID, this._gridBuffer);
            this._computeShader.Dispatch(this._initKernelIndex, this._threadGroups, this._threadGroups, 1);

            this._computeShader.SetTexture(this._applyBufferKernelIndex, RESULT_ID, this._result);
            this._computeShader.SetTexture(this._applyBufferKernelIndex, GRID_BUFFER_ID, this._gridBuffer);
            this._computeShader.Dispatch(this._applyBufferKernelIndex, this._threadGroups, this._threadGroups, 1);
        }

        protected override void Next()
        {
            base.Next();

            this._computeShader.SetTexture(this._nextKernelIndex, RESULT_ID, this._result);
            this._computeShader.SetTexture(this._nextKernelIndex, GRID_BUFFER_ID, this._gridBuffer);
            this._computeShader.Dispatch(this._nextKernelIndex, this._threadGroups, this._threadGroups, 1);

            this.ApplyTextureBuffer();
        }
    }
}