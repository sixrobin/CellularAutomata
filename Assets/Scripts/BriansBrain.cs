namespace CellularAutomata
{
    public class BriansBrain : CellularAutomaton2D
    {
        protected override void Init()
        {
            base.Init();
            
            this._computeShader.SetTexture(0, RESULT_ID, this._gridBuffer);
            this._computeShader.Dispatch(0, this.Resolution / 8, this.Resolution / 8, 1);

            this._computeShader.SetTexture(2, RESULT_ID, this._result);
            this._computeShader.SetTexture(2, GRID_BUFFER_ID, this._gridBuffer);
            this._computeShader.Dispatch(2, this.Resolution / 8, this.Resolution / 8, 1);
        }

        protected override void Next()
        {
            base.Next();

            this._computeShader.SetTexture(1, RESULT_ID, this._result);
            this._computeShader.SetTexture(1, GRID_BUFFER_ID, this._gridBuffer);
            this._computeShader.Dispatch(1, this.Resolution / 8, this.Resolution / 8, 1);
            
            this.ApplyTextureBuffer();
        }
    }
}
