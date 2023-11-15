namespace CellularAutomata
{
    using UnityEngine;

    public class CellularAutomaton3D : MonoBehaviour
    {
        [SerializeField]
        private int _resolution = 16;
        [SerializeField, Range(0f, 1f)]
        private float _initRandomStep = 0.5f;
        [SerializeField]
        private string _rules = "B/S";
        
        private int[,,] _grid;
        private int[,,] _gridBuffer;

        private void Init()
        {
            this._grid = new int[this._resolution, this._resolution, this._resolution];
            
            for (int x = 0; x < this._resolution; ++x)
                for (int y = 0; y < this._resolution; ++y)
                    for (int z = 0; z < this._resolution; ++z)
                        this._grid[x, y, z] = Random.value < this._initRandomStep ? 1 : 0;
        }

        private bool IsInRange(int x, int y, int z)
        {
            return x >= 0 && x < this._resolution && y >= 0 && y < this._resolution && z >= 0 && z < this._resolution;
        }
        
        private int ComputeNeighboursCount(int x, int y, int z)
        {
            int neighboursCount = 0;
            
            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    for (int dz = -1; dz <= 1; ++dz)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        int nz = z + dz;

                        if (IsInRange(nx, ny, nz))
                            neighboursCount += this._gridBuffer[nx, ny, nz];
                    }
                }   
            }

            return neighboursCount;
        }
        
        [ContextMenu("Iterate")]
        private void Iterate()
        {
            this._gridBuffer = this._grid;
            Vector2Int rules = RulesParser.Parse(this._rules);
            
            for (int x = 0; x < this._resolution; ++x)
            {
                for (int y = 0; y < this._resolution; ++y)
                {
                    for (int z = 0; z < this._resolution; ++z)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            continue;
                        
                        int neighboursCount = this.ComputeNeighboursCount(x, y, z);
                        int neighbours = (int)Mathf.Pow(2, neighboursCount);

                        if (this._gridBuffer[x, y, z] == 1)
                            this._grid[x, y, z] = (neighbours & rules.x) != 0 ? 1 : 0;
                        else
                            this._grid[x, y, z] = (neighbours & rules.y) != 0 ? 1 : 0;
                    }
                }
            }
        }

        private void Start()
        {
            this.Init();
        }

        private void Update()
        {
            this.Iterate();
        }

        private void OnDrawGizmosSelected()
        {
            if (this._grid == null)
                return;

            for (int x = 0; x < this._resolution; ++x)
            {
                for (int y = 0; y < this._resolution; ++y)
                {
                    for (int z = 0; z < this._resolution; ++z)
                    {
                        if (this._grid[x, y, z] == 0)
                            continue;

                        float r = x / (float)(this._resolution - 1);
                        float g = y / (float)(this._resolution - 1);
                        float b = z / (float)(this._resolution - 1);

                        Gizmos.color = new Color(r, g, b);
                        Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one);
                    }
                }
            }
        }
    }
}