namespace CellularAutomata
{
    using UnityEngine;
    
    public enum ColorMode
    {
        /// <summary>
        /// White cells.
        /// </summary>
        [InspectorName("None")]            NONE,
        
        /// <summary>
        /// Use the cell current state to sample a color from a gradient.
        /// </summary>
        [InspectorName("State")]           STATE,
        
        /// <summary>
        /// Use the cell position XYZ as input for RGB channels.
        /// </summary>
        [InspectorName("Position")]        POSITION,
        
        /// <summary>
        /// Use the cell distance to automaton center to sample a color from a gradient.
        /// </summary>
        [InspectorName("Center distance")] CENTER_DISTANCE,
    }
}