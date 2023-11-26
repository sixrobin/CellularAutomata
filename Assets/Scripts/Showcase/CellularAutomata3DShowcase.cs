namespace CellularAutomata.Showcase
{
    using UnityEngine;

    public class CellularAutomata3DShowcase : MonoBehaviour
    {
        [SerializeField]
        private CellularAutomatonSettings[] _settings;
        [SerializeField]
        private CellularAutomaton3D _automaton;
        [SerializeField]
        private CellularAutomata3DShowcaseUI _interface;

        private CellularAutomaton3D[] _automata;
        private int _currentAutomatonIndex = -1;

        private void Init()
        {
            _automata = new CellularAutomaton3D[this._settings.Length];
            for (int i = 0; i < this._settings.Length; ++i)
            {
                this._automata[i] = Instantiate(this._automaton);
                this._automata[i].SetSettings(this._settings[i]);
                this._automata[i].gameObject.SetActive(false);
            }
        }
        
        private void Next()
        {
            if (this._currentAutomatonIndex > -1)
                this._automata[this._currentAutomatonIndex].gameObject.SetActive(false);
            
            this._currentAutomatonIndex = ++_currentAutomatonIndex % this._settings.Length;
            this._automata[this._currentAutomatonIndex].gameObject.SetActive(true);
            this._interface.SetSettings(this._settings[this._currentAutomatonIndex]);
        }

        private void Start()
        {
            this.Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                this.Next();
        }
    }
}