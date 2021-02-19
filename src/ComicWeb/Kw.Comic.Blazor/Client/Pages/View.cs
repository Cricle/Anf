using Kw.Comic.Blazor.Server.Models;
using Kw.Comic.Engine;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kw.Comic.Blazor.Client.Pages
{
    public partial class View
    {
        private AnalysisResult result;

        public bool LoadDone { get; set; } = true;

        public bool CanNext { get; set; }

        public int CurrentIndex { get; set; }

        internal ChapterWithPage CurrentChapter { get; set; }

        [Parameter]
        public string Site { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        public void Next()
        {
            ToPage(CurrentIndex + 1);
        }
        public void Prev()
        {
            ToPage(CurrentIndex - 1);
        }

        public void ToPage(int i)
        {
            if (i >= 0 && result.ChapterWithPages.Length > i)
            {
                CurrentIndex = i;
                CurrentChapter = result.ChapterWithPages[i];
                StateHasChanged();
            }
        }

        protected async override Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(Site))
            {
                var url = "/api/v1/Analysisi/Get?address=" + Site;
                try
                {
                    using var val = await HttpClient.GetAsync(url);
                    var str = await val.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(str))
                    {
                        result = JsonSerializer.Deserialize<AnalysisResult>(str);
                        ToPage(0);
                    }
                }
                catch (Exception e)
                {
                }
            }
            LoadDone = false;
        }
    }
}
