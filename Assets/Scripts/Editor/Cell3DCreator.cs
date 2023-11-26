namespace CellularAutomata.Editor
{
    using UnityEngine;

    [RequireComponent(typeof(MeshFilter))]
    public class Cell3DCreator : MonoBehaviour
    {
        [ContextMenu("Create")]
        private void CreateCube()
        {
            Vector3[] vertices =
            {
                Vector3.zero,
                Vector3.right,
                Vector3.up,
                new(1f, 1f),
                Vector3.up,
                new(1f, 1f),
                new(0f, 1f, 1f),
                Vector3.one,
                new(1f, 1f),
                Vector3.right,
                Vector3.one,
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
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.right,
                Vector3.right,
                Vector3.right,
                Vector3.right,
            };

            Mesh mesh = new()
            {
                vertices = vertices,
                triangles = triangles,
                normals = normals,
            };

            MeshFilter meshFilter = this.GetComponent<MeshFilter>();
            
            if (meshFilter.mesh != null)
                meshFilter.mesh.Clear();
        
            meshFilter.mesh = mesh;
        }
    }   
}
