namespace CellularAutomata
{
    using UnityEngine;
    
    public enum ColorMode
    {
        [InspectorName("None")]            NONE,
        [InspectorName("State")]           STATE,
        [InspectorName("Position")]        POSITION,
        [InspectorName("Center distance")] CENTER_DISTANCE,
    }
}