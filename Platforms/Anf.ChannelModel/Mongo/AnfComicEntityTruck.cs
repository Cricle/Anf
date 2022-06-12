namespace Anf.ChannelModel.Mongo
{
    public class AnfComicEntityTruck : AnfComicEntityInfoOnly
    {
        public ComicChapter[] Chapters { get; set; }
    }
    public class AnfComicEntityScoreTruck
    {
        public AnfComicEntityTruck Truck { get; set; }

        public double Score { get; set; }
    }
}
