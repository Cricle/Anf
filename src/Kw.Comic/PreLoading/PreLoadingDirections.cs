namespace Kw.Comic.PreLoading
{
    public enum PreLoadingDirections
    {
        None = 0,
        Left = 1,
        Right = Left << 1,
        Both = Left | Right
    }
}
