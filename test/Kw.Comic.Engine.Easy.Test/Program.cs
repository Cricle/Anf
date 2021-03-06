using System;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            EasyComicBuilder.Download("http://www.dm5.com/manhua-monvzhilv/",
                new PhysicalFileSaver(AppDomain.CurrentDomain.BaseDirectory));
        }
    }
}
