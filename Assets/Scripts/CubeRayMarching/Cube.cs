namespace CellularAutomata.CubeRayMarching
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class Cube : MonoBehaviour
    {
        public Vector3 Position => this.transform.position;
        public Vector3 Scale => this.transform.localScale;
    }
}
