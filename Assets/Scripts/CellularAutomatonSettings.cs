namespace CellularAutomata
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewCellularAutomatonSettings", menuName = "Cellular Automaton Settings")]
    public class CellularAutomatonSettings : ScriptableObject
    {
        [SerializeField]
        private string _displayName;
        [SerializeField]
        private Resolution _resolution = CellularAutomata.Resolution._64;
        [SerializeField]
        private string _rules = "1/1/2/M";
        [SerializeField]
        private InitializationMethod _initializationMethod = InitializationMethod.RANDOM_STEP;
        [SerializeField, Range(0f, 1f)]
        private float _initRandomStep = 0.5f;
        [SerializeField, Min(0)]
        private int _initCenterWidth = 16;
        [SerializeField, Min(0f)]
        private float _iterationDelay = 0.1f;
        [SerializeField]
        private Gradient _gradient;
        [SerializeField]
        private bool _useWorldSpaceColor;
        [SerializeField]
        private bool _useCenterDistanceColor;

        public string DisplayName => string.IsNullOrEmpty(this._displayName) ? this.name : this._displayName;
        public Resolution Resolution => this._resolution;
        public string Rules => this._rules;
        public InitializationMethod InitializationMethod => this._initializationMethod;
        public float InitRandomStep => this._initRandomStep;
        public int InitCenterWidth => this._initCenterWidth;
        public float IterationDelay => this._iterationDelay;
        public Gradient Gradient => this._gradient;
        public bool UseWorldSpaceColor => this._useWorldSpaceColor;
        public bool UseCenterDistanceColor => this._useCenterDistanceColor;

        // public void SetSettings(Resolution res, string rules, InitializationMethod initMethod, float randomStep, int width, float delay, Gradient gradient)
        // {
        //     this._resolution = res;
        //     this._rules = rules;
        //     this._initializationMethod = initMethod;
        //     this._initRandomStep = randomStep;
        //     this._initCenterWidth = width;
        //     this._iterationDelay = delay;
        //     this._gradient = gradient;
        //     
        //     UnityEditor.AssetDatabase.SaveAssets();
        // }
    }
}