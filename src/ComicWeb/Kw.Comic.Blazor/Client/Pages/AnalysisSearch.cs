using Kw.Comic.Blazor.Server.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kw.Comic.Blazor.Client.Pages
{
    public partial class AnalysisSearch
    {

        public AnalysisSearch()
        {
        }
        public string Address { get; set; }

        public bool HasFind { get; set; }

        internal AnalysisResult Result { get; set; }

        public AddParseStatus? AddParseStatus { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public ILogger<AnalysisSearch> Logger { get; set; }


        public async void Add(MouseEventArgs e)
        {
            await FindAsync();
        }

        public async Task FindAsync()
        {
            //TODO:check
            var url = "/api/v1/Analysisi/Get?address=" + Address;
            Result = null;
            var ok = false;
            try
            {
                using var val = await HttpClient.GetAsync(url);
                var str = await val.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    var result = JsonSerializer.Deserialize<AnalysisResult>(str);
                    Result = result;
                    ok = true;
                }
            }
            catch (Exception e) 
            {
                Logger.LogError(e.ToString());
            }
            if (!ok)
            {
                url = "/api/v1/Analysisi/Add?address=" + Address;
                using var val = await HttpClient.GetAsync(url);
                var context =await val.Content.ReadAsStringAsync();
                if (Enum.TryParse<AddParseStatus>(context, out var status))
                {
                    AddParseStatus = status;
                }
                else
                {
                    status = Comic.AddParseStatus.Unknow;
                }
            }
            HasFind = true;
        }
    }
}
