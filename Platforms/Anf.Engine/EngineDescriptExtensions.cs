namespace Anf
{
    public static class EngineDescriptExtensions
    {
        public static void SetName(this EngineDescript desc,string name)
        {
            desc[EngineDescriptConst.Name] = name;
        }
        public static void SetUrl(this EngineDescript desc, string url)
        {
            desc[EngineDescriptConst.Url] = url;
        }
        public static void SetDescript(this EngineDescript desc, string descript)
        {
            desc[EngineDescriptConst.Descript] = descript;
        }
    }
}
