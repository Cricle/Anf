using Newtonsoft.Json;
using System;
using System.IO;

namespace Anf.Easy.Test.Resources
{
    public static class ResourceHelper
    {
        public static readonly string ResourceFolder = "Resources";
        public static readonly string EntityFolder = "Entities";
        public static readonly string ChapterFolder = "Chapters";

        public static ComicEntity GetMonvzhilv()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourceFolder, EntityFolder, "monvzhilv.txt");
            var str = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<ComicEntity>(str);
        }
        public static ComicChapter GetMonvzhilvChatper(int index)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourceFolder, ChapterFolder, $"monvzhilv-{index}.txt");
            var str = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<ComicChapter>(str);
        }
        public static ComicChapter GetMonvzhilvChatper0()
        {
            return GetMonvzhilvChatper(0);
        }
        public static ComicChapter GetMonvzhilvChatper1()
        {
            return GetMonvzhilvChatper(1);
        }
    }
}
