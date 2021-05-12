using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.ChannelModel.Mongo
{
    public class AnfComicEntity : AnfComicEntityInfoOnly
    {
        public AnfComicEntity()
        {
            WithPageChapters = Array.Empty<WithPageChapter>();
        }

        [BsonId]
        public BsonObjectId Id { get; set; }

        public WithPageChapter[] WithPageChapters { get; set; }
    }
}
