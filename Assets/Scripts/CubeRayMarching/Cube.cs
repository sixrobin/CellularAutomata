namespace CellularAutomata.CubeRayMarching
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class Cube : MonoBehaviour
    {
        [field: SerializeField]
        public Color Color { get; private set; }

        public Vector3 ColorToVector3() => new Vector3(this.Color.r, this.Color.g, this.Color.b);
        public Vector3 Position => this.transform.position;
        public Vector3 Scale => this.transform.localScale;
    }
}
