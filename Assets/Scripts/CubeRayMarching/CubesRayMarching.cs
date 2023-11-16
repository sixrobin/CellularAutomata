namespace CellularAutomata.CubeRayMarching
{
    using UnityEngine;

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    // [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
    public class CubesRayMarching : MonoBehaviour
    {
        private struct CubeInfo
        {
            public const int BUFFER_SIZE = sizeof(float) * 9;

            public Vector3 Position;
            public Vector3 Scale;
            public Vector3 Color;
        }
        
        [SerializeField]
        private ComputeShader _computeShader;
        
        private Cube[] _cubes;
        private Camera _camera;
        private RenderTexture _renderTexture;

        private void Init()
        {
            this._camera = Camera.main;
            this._cubes = FindObjectsByType<Cube>(FindObjectsSortMode.None);
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

        private CubeInfo[] ComputeCubesInfo()
        {
            // TODO: Sort by depth.
            CubeInfo[] cubesInfo = new CubeInfo[this._cubes.Length];
            
            for (int i = 0; i < _cubes.Length; ++i)
            {
                Cube cube = _cubes[i];
                cubesInfo[i] = new CubeInfo
                {
                    Position = cube.Position,
                    Scale = cube.Scale,
                    Color = cube.ColorToVector3(),
                };
            }

            return cubesInfo;
        }
        
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            this.Init();
            this.InitRenderTexture();

            // Cubes info.
            CubeInfo[] cubesInfo = this.ComputeCubesInfo();
            ComputeBuffer cubesBuffer = new(cubesInfo.Length, CubeInfo.BUFFER_SIZE);
            cubesBuffer.SetData(cubesInfo);
            this._computeShader.SetBuffer(0, "Cubes", cubesBuffer);
            this._computeShader.SetInt("CubesLength", cubesInfo.Length);
            
            // Rendering parameters.
            this._computeShader.SetMatrix("_CameraToWorld", this._camera.cameraToWorldMatrix);
            this._computeShader.SetMatrix("_CameraInverseProjection", this._camera.projectionMatrix.inverse);
            
            // Set source/destination textures.
            this._computeShader.SetTexture(0, "Source", source);
            this._computeShader.SetTexture(0, "Destination", this._renderTexture);
            
            // Dispatch.
            int threadGroupsX = Mathf.CeilToInt(this._camera.pixelWidth / 8f);
            int threadGroupsY = Mathf.CeilToInt(this._camera.pixelHeight / 8f);
            this._computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

            Graphics.Blit(this._renderTexture, destination);
            
            // TODO: Release buffers.
        }
    }
}