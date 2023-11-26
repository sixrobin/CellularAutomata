namespace CellularAutomata.Editor
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Class used to save a mesh from a MeshFilter as a mesh asset.
    /// Can be used for procedurally generated meshes.
    /// </summary>
    public static class MeshFilterToAsset
    {
        [MenuItem("CONTEXT/MeshFilter/Save")]
        public static void SaveMesh(MenuCommand menuCommand) => SaveMesh(menuCommand.context as MeshFilter, false);

        [MenuItem("CONTEXT/MeshFilter/Save (new mesh instance)")]
        public static void SaveMeshAsNewInstance(MenuCommand menuCommand) => SaveMesh(menuCommand.context as MeshFilter, true);

        private static void SaveMesh(MeshFilter meshFilter, bool makeNewInstance)
        {
            if (meshFilter == null)
            {
                Debug.Log("No selected MeshFilter to save mesh from.");
                return;
            }
            
            if (meshFilter.sharedMesh == null)
            {
                Debug.Log("MeshFilter's sharedMesh is null.");
                return;
            }
            
            string path = EditorUtility.SaveFilePanel("Save Mesh as asset", "Assets/", meshFilter.sharedMesh.name, "asset");
            if (string.IsNullOrEmpty(path))
                return;

            path = FileUtil.GetProjectRelativePath(path);

            Mesh mesh = meshFilter.sharedMesh;
            Mesh meshToSave = makeNewInstance ? Object.Instantiate(mesh) : mesh;
            MeshUtility.Optimize(meshToSave);
            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();
        }
    }
}