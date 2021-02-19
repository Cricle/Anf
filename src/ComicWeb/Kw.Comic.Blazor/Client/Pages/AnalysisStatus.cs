using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Kw.Comic.Blazor.Server.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Blazor.Client.Pages
{
    public class ComicInfoWraper
    {
        public ComicSnapsnot Info { get; set; }

        public bool Succeed { get; set; }
    }
    public partial class AnalysisStatus
    {

        public LinkedList<ComicInfoWraper> ComicInfos { get; set; } = new LinkedList<ComicInfoWraper>();

        public long Current { get; set; }

        public long Total { get; set; }

        public string ProcessWidth { get; set; }

        public string DetailProcessWidth { get; set; }

        public string ComicName { get; set; }

        public string ComicDescript { get; set; }

        public string ChapterName { get; set; }

        public long DetailCurrent { get; set; }

        public long DetailTotal { get; set; }

        public DateTime? EstimateAt { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public AnalysisingService.AnalysisingServiceClient AnalysisingServiceClient { get; set; }
        [Inject]
        public ILogger<AnalysisStatus> Logger { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override Task OnInitializedAsync()
        {
            Logger.LogInformation("Got!!");
            Does();
            return base.OnInitializedAsync();
        }
        public void NaviteTo(string uri)
        {
            NavigationManager.NavigateTo(uri);
        }
        private async void Does()
        {
            var url = "/api/v1/Analysisi/Gets?skip=0&take=20";
            using var rep = await HttpClient.GetAsync(url);
            try
            {
                var str = await rep.Content.ReadAsStringAsync();
                var val = JsonSerializer.Deserialize<AnalysisResult[]>(str);
                foreach (var item in val)
                {
                    var wrap = new ComicInfoWraper
                    {
                        Info = new ComicSnapsnot
                        {
                            Id = item.Id,
                            Descript = item?.Descript,
                            Name = item?.Name,
                            Url = item?.Url
                        },
                        Succeed = true
                    };
                    ComicInfos.AddFirst(wrap);
                }
            }
            catch (Exception ex) 
            {
                Logger.LogError(ex.ToString());
            }
            url = "/api/v1/Analysisi/CurrentStatus";
            try
            {
                var str = await rep.Content.ReadAsStringAsync();
                var val = JsonSerializer.Deserialize<UpdateProgressReponse>(str);
                if (val!=null)
                {
                    ReceivedStatus(val);
                }
            }
            catch (Exception) { }
            StateHasChanged();
            var reader = AnalysisingServiceClient.ReceivedUpdateProgress(new Empty());
            while (await reader.ResponseStream.MoveNext())
            {
                var inst = reader.ResponseStream.Current;
                ReceivedStatus(inst);
            }
        }
        private void ReceivedStatus(UpdateProgressReponse inst)
        {
            Current = inst.Current;
            Total = inst.Total;
            ComicName = inst.Info?.Name;
            ComicDescript = inst.Info?.Descript;
            ChapterName = inst.Info?.CurrentName;
            ProcessWidth = $"width:{(Total == 0 ? 0 : (Current / (float)Total) * 100):f2}%";
            DetailCurrent = inst.Info?.Current ?? 0;
            DetailTotal = inst.Info?.Total ?? 0;
            DetailProcessWidth = $"width:{(DetailTotal == 0 ? 0 : (DetailCurrent / (float)DetailTotal) * 100):f2}%";
            OnStatusChanged(inst);
            EstimateAt = DateTime.Now.AddMilliseconds(inst.Total - inst.Current);
            StateHasChanged();
        }
        private void OnStatusChanged(UpdateProgressReponse request)
        {
            if (request.Operator== UpdateOperators.Complated|| request.Operator == UpdateOperators.Fail)
            {
                if (ComicInfos.Count > 50)
                {
                    ComicInfos.RemoveLast();
                }
                ComicName = ComicDescript = ChapterName = null;
                ComicInfos.AddFirst(new ComicInfoWraper
                {
                    Info = request.Info,
                    Succeed = request.Operator == UpdateOperators.Complated
                });
            }
        }

    }
}
