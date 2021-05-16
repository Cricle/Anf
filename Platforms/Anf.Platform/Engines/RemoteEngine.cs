using Anf.ChannelModel.Mongo;
using Anf.Networks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anf.Platform.Engines
{
    public class RemoteEngine
    {
        private const string Host = "https://anf.azureedge.net/";
        private const string EntityAddress = Host + "api/v1/reading/GetEntity?url=";
        private const string ChapterAddress = Host + "api/v1/reading/GetChapter";
        private readonly INetworkAdapter networkAdapter;

        public RemoteEngine(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        public async Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            try
            {
                var url = EntityAddress + targetUrl;
                var str = await networkAdapter.GetStringAsync(new RequestSettings
                {
                    Address = url
                });

                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }
                var truck = JsonSerializer.Deserialize<AnfComicEntityTruck>(str, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (truck is null)
                {
                    return null;
                }
                return new ComicEntity
                {
                    Chapters = truck.Chapters,
                    ComicUrl = truck.ComicUrl,
                    Descript = truck.Descript,
                    ImageUrl = truck.ImageUrl,
                    Name = truck.Name,
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl, string entityUrl)
        {
            try
            {


                var url = ChapterAddress + "?url=" + targetUrl + "&entityUrl=" + entityUrl;
                var str = await networkAdapter.GetStringAsync(new RequestSettings
                {
                    Address = url
                });
                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }
                var p = JsonSerializer.Deserialize<WithPageChapter>(str, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (p is null)
                {
                    return null;
                }
                return p.Pages;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
