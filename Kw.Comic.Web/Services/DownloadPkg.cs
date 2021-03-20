namespace KwC.Services
{
    public readonly struct DownloadPkg
    {
        public readonly string Address;
        public readonly string Path;

        public DownloadPkg(string address, string path)
        {
            Address = address;
            Path = path;
        }
    }
}
