using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Blazor.Server.Rpc
{
    public class ComicAnalysisingService: AnalysisingService.AnalysisingServiceBase
    {
        public static readonly ConcurrentDictionary<Empty, IServerStreamWriter<UpdateProgressReponse>> StreamWriters =
            new ConcurrentDictionary<Empty, IServerStreamWriter<UpdateProgressReponse>>();

        public override async Task ReceivedUpdateProgress(Empty request, IServerStreamWriter<UpdateProgressReponse> responseStream, ServerCallContext context)
        {
            StreamWriters.TryAdd(request, responseStream);
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
            StreamWriters.TryRemove(request, out _);
        }
    }
}
