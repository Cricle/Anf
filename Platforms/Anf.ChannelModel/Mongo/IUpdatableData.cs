namespace Anf.ChannelModel.Mongo
{
    public interface IUpdatableData
    {
        long CreateTime { get; set; }

        long UpdateTime { get; set; }
    }
}
