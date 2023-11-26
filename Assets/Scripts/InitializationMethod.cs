namespace CellularAutomata
{
    [System.Flags]
    public enum InitializationMethod
    {
        NONE = 0,
        CENTER_CELLS = 1 << 0,
        RANDOM_STEP = 1 << 1,
    }
}