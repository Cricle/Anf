﻿using BetterStreams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ValueBuffer;

namespace Anf.Easy
{
    internal class ComicDownloader : IComicDownloader
    {
        public Func<Task>[] EmitTasks(ComicDownloadRequest request, CancellationToken token = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (request.Provider is null || request.Saver is null || request.DownloadRequests is null)
            {
                throw new ArgumentNullException("Any null for " + nameof(request));
            }
            var tasks = new List<Func<Task>>();
            foreach (var item in request.DownloadRequests)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                tasks.Add(() => DownloadPageAsync(request, item.Chapter, item.Page, token));
            }
            return tasks.ToArray();
        }
        private async Task DownloadPageAsync(ComicDownloadRequest request, ComicChapter chapter, ComicPage page, CancellationToken token)
        {
            var listener = request.Listener;
            DownloadListenerContext listenerContext = null;
            if (listener != null)
            {
                listenerContext = new DownloadListenerContext(request, chapter, page, token);
                await listener.ReadyFetchAsync(listenerContext);
            }
            if (listener != null && token.IsCancellationRequested)
            {
                await listener.CanceledAsync(listenerContext);
                return;
            }
            var ctx = new ComicDownloadContext(request.Entity, chapter, page, null, token);
            if (!request.Saver.NeedToSave(ctx))
            {
                if (listener != null)
                {
                    await listener.NotNeedToSaveAsync(listenerContext);
                }
                return;
            }
            try
            {
                if (listener != null)
                {
                    await listener.BeginFetchPageAsync(listenerContext);
                }
                using (var stream = await request.Provider.GetImageStreamAsync(page.TargetUrl))
                using (var destStream = new PooledMemoryStream())
                {
                    DownloadSaveListenerContext saveCtx = null;
                    if (listener != null)
                    {
                        saveCtx = new DownloadSaveListenerContext(request, chapter, page, token, destStream);
                        await listener.ReadySaveAsync(saveCtx);
                    }
                    await stream.CopyToAsync(destStream);
                    destStream.Seek(0, SeekOrigin.Begin);
                    ctx = new ComicDownloadContext(request.Entity, chapter, page, destStream, token);
                    await request.Saver.SaveAsync(ctx);
                    if (listener != null)
                    {
                        await listener.EndFetchPageAsync(saveCtx);
                    }
                }
            }
            catch (Exception ex)
            {
                if (listener != null)
                {
                    var errCtx = new DownloadExceptionListenerContext(request, chapter, page, token, ex);
                    await listener.FetchPageExceptionAsync(errCtx);
                }
            }
            if (listener != null)
            {
                await listener.ComplatedSaveAsync(listenerContext);
            }
        }
    }
}
