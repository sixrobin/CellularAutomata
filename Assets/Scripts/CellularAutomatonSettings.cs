namespace CellularAutomata
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewCellularAutomatonSettings", menuName = "Cellular Automaton Settings")]
    public class CellularAutomatonSettings : ScriptableObject
    {
        #region SERIALIZED FIELDS
        [SerializeField]
        [Tooltip("Display name, that's only used for UI or debugging.")]
        private string _displayName;
        
        [SerializeField]
        [Tooltip("Automaton resolution (width, height, and depth for 3D automata).")]
        private int _resolution = 64;
        
        [SerializeField]
        [Tooltip("Automaton ruleset (in A/B/C/D format, where A is the survive rule, B the birth rule, C the number of states a cell can have, and D the neighbourhood index).")]
        private string _rules = "1/1/2/M";
        
        [Header("INITIALIZATION")]

        [SerializeField]
        [Tooltip("How is the automaton initial state computed"
                 + "Center cells: filters the cells that are not in a given distance from center."
                 + "Random state: each cell is initialized as alive only based on a random chance.")]
        private InitializationMethod _initializationMethod = InitializationMethod.RANDOM_STATE;
        
        [SerializeField, Range(0f, 1f)]
        [Tooltip("Chances of a cell to start the simulation alive. Only used if initialization method includes random state.")]
        private float _initRandomStep = 0.5f;
        
        [SerializeField, Min(0)]
        [Tooltip("Maximum distance to automaton center for a cell to start the simulation alive. Only used if initialization method includes center cells.")]
        private int _initCenterWidth = 16;
        
        [SerializeField, Min(0f)]
        [Tooltip("Time waited in seconds between two iterations of the automaton simulation.")]
        private float _iterationDelay = 0.1f;

        [Header("COLOR")]
        
        [SerializeField]
        [Tooltip("Color mode."
                 + "None: white cells."
                 + "State: use the current cell state to pick into the color gradient."
                 + "Position: use the normalized cell position into the automaton."
                 + "Center distance: use the normalized distance to automaton center to pick into the color gradient.")]
        private ColorMode _colorMode = ColorMode.POSITION;
        
        [SerializeField]
        [Tooltip("Color gradient. Only used for color modes State and Center distance.")]
        private Gradient _gradient;
        #endregion // SERIALIZED FIELDS

        #region PROPERTIES
        public string DisplayName => string.IsNullOrEmpty(this._displayName) ? this.name : this._displayName;
        public int Resolution => this._resolution;
        public string Rules => this._rules;
        public InitializationMethod InitializationMethod => this._initializationMethod;
        public float InitRandomStep => this._initRandomStep;
        public int InitCenterWidth => this._initCenterWidth;
        public float IterationDelay => this._iterationDelay;
        public ColorMode ColorMode => this._colorMode;
        public Gradient Gradient => this._gradient;
        #endregion // PROPERTIES
    }
}