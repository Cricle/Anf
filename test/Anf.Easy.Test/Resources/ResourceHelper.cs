using Anf.Engine;
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
            return JsonHelper.Deserialize<ComicEntity>(str);
        }
        public static ChapterWithPage GetMonvzhilvChatper(int index)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourceFolder, ChapterFolder, $"monvzhilv-{index}.txt");
            var str = File.ReadAllText(path);
            return JsonHelper.Deserialize<ChapterWithPage>(str);
        }
        public static ChapterWithPage GetMonvzhilvChatper0()
        {
            return GetMonvzhilvChatper(0);
        }
        public static ChapterWithPage GetMonvzhilvChatper1()
        {
            return GetMonvzhilvChatper(1);
        }
    }
}
