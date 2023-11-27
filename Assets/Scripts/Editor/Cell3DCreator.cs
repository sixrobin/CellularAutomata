namespace CellularAutomata.Editor
{
    using UnityEngine;

    /// <summary>
    /// Quick and dirty implementation to generate a cube with only half of its faces.
    /// Such a cube can be used for 3D cellular automata cells instancing, without the cost of 3 faces that are not facing the camera.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class Cell3DCreator : MonoBehaviour
    {
        [ContextMenu("Create 3 Faces Cube")]
        private void CreateThreeFacesCube()
        {
            Vector3[] vertices =
            {
                new(0f, 0f, 0f),
                new(1f, 0f, 0f),
                new(0f, 1f, 0f),
                new(1f, 1f, 0f),
                new(0f, 1f, 0f),
                new(1f, 1f, 0f),
                new(0f, 1f, 1f),
                new(1f, 1f, 1f),
                new(1f, 1f, 0f),
                new(1f, 0f, 0f),
                new(1f, 1f, 1f),
                new(1f, 0f, 1f),
            };

            int[] triangles =
            {
                0, 2, 1,
                2, 3, 1,
                4, 6, 5,
                6, 7, 5,
                8, 10, 9,
                10, 11, 9,
            };

            Vector3[] normals =
            {
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.right,
                Vector3.right,
                Vector3.right,
                Vector3.right,
            };

            MeshFilter meshFilter = this.GetComponent<MeshFilter>();
            if (meshFilter.mesh != null)
                meshFilter.mesh.Clear();
        
            meshFilter.mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                normals = normals,
            };
        }
    }   
}
