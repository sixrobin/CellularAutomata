namespace CellularAutomata
{
    public class BriansBrain : CellularAutomaton2D
    {
        protected override void Init()
        {
            base.Init();
            
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
