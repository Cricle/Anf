namespace KwC.Hubs
{
    public enum NotifyTypes
    {
        BeginFetchPage = 0,
        Canceled = 1,
        Complated = 2,
        EndFetchPage = 3,
        FetchPageException = 4,
        NotNeedToSave = 5,
        ReadyFetch = 6,
        ReadySave = 7,
    }
}
