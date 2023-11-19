namespace CellularAutomata
{
    using CubeRayMarching;
    using UnityEngine;
    using CubeInfo = CubeRayMarching.CubesRayMarching.CubeInfo;

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class CellularAutomaton3DShader : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader _computeShader;
        [SerializeField]
        private int _resolution = 8;
        
        private CubeInfo[] _cubes;
        private Camera _camera;
        private RenderTexture _renderTexture;
        private ComputeBuffer _cubesBuffer;
        
        private void Awake()
        {
            this.Init();
        }

        private void Init()
        {
            this._camera = Camera.main;

            this._cubes = new CubeInfo[this._resolution * this._resolution * this._resolution];
            
            for (int x = 0; x < this._resolution; ++x)
                for (int y = 0; y < this._resolution; ++y)
                    for (int z = 0; z < this._resolution; ++z)
                        this._cubes[x + this._resolution * y + this._resolution * this._resolution * z] = new CubeInfo
                        {
                            Position = new Vector3(x, y, z),
                            Scale = Vector3.one,
                            Color = new Vector3(x / (float)(this._resolution - 1), y / (float)(this._resolution - 1), z / (float)(this._resolution - 1))
                        };
            
            this._cubesBuffer = new ComputeBuffer(this._cubes.Length, CubeInfo.BUFFER_SIZE);
            
            this.InitRenderTexture();
        }

        private void InitRenderTexture()
        {
            if (this._renderTexture == null
                || this._renderTexture.width != this._camera.pixelWidth
                || this._renderTexture.height != this._camera.pixelHeight)
            {
                if (this._renderTexture != null)
                    this._renderTexture.Release();

                this._renderTexture = new RenderTexture(this._camera.pixelWidth,
                                                        this._camera.pixelHeight, 
                                                        0,
                                                        RenderTextureFormat.ARGBFloat,
                                                        RenderTextureReadWrite.Linear)
                {
                    enableRandomWrite = true
                };

                this._renderTexture.Create();
            }
        }
        
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Cubes info.
            this._cubesBuffer.SetData(this._cubes);
            this._computeShader.SetBuffer(0, "Cubes", this._cubesBuffer);
            this._computeShader.SetInt("CubesLength", this._cubes.Length);
            
            // Rendering parameters.
            this._computeShader.SetMatrix("_CameraToWorld", this._camera.cameraToWorldMatrix);
            this._computeShader.SetMatrix("_CameraInverseProjection", this._camera.projectionMatrix.inverse);
            
            // Set destination texture.
            this._computeShader.SetTexture(0, "Result", this._renderTexture);
            
            // Dispatch.
            int threadGroupsX = Mathf.CeilToInt(this._camera.pixelWidth / 8f);
            int threadGroupsY = Mathf.CeilToInt(this._camera.pixelHeight / 8f);
            this._computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
            
            Graphics.Blit(this._renderTexture, destination);
            
            // TODO: Release buffers.
        }
    }
}