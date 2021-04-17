using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test
{
    internal class NullChapterAnalysisNotifyer : IChapterAnalysisNotifyer
    {
        public Task FetchedChapterAsync(ChapterAnalysedContext context)
        {
            Assert.IsNotNull(context);
#if NET461_OR_GREATER || NETSTANDARD2_0||NETCOREAPP3_0_OR_GREATER

            return Task.CompletedTask;
#else
            return Task.FromResult(0);
#endif
        }

        public Task FetchedComicAsync(ComicAnalysedContext context)
        {
            Assert.IsNotNull(context);
#if NET461_OR_GREATER || NETSTANDARD2_0||NETCOREAPP3_0_OR_GREATER

            return Task.CompletedTask;
#else
            return Task.FromResult(0);
#endif
        }

        public Task FetchingChapterAsync(ChapterAnalysingContext context)
        {
            Assert.IsNotNull(context);
#if NET461_OR_GREATER || NETSTANDARD2_0||NETCOREAPP3_0_OR_GREATER

            return Task.CompletedTask;
#else
            return Task.FromResult(0);
#endif
        }

        public Task FetchingComicAsync(ComicAnalysingContext context)
        {
            Assert.IsNotNull(context);
#if NET461_OR_GREATER || NETSTANDARD2_0||NETCOREAPP3_0_OR_GREATER

            return Task.CompletedTask;
#else
            return Task.FromResult(0);
#endif
        }
    }
}
