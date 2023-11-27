namespace CellularAutomata
{
    using UnityEngine;
    
    [System.Flags]
    public enum InitializationMethod
    {
        /// <summary>
        /// Only includes the values in the center of the automaton.
        /// </summary>
        [InspectorName("Center cells")] CENTER_CELLS = 1 << 0,
        
        /// <summary>
        /// Randomizes the initial state of each cell.
        /// </summary>
        [InspectorName("Random state")] RANDOM_STATE = 1 << 1,
    }
}