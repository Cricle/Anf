namespace Anf.ChannelModel.Mongo
{
    public class AnfComicEntityInfoOnly : ComicInfo, IUpdatableData, IRefableData
    {
        public long RefCount { get; set; }

        public long CreateTime { get; set; }

        public long UpdateTime { get; set; }
    }
}
