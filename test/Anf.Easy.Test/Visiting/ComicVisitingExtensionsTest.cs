using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class ComicVisitingExtensionsTest
    {
        class NullComicVisiting<T> : IComicVisiting<T>
        {
            public IComicVisitingInterceptor<T> VisitingInterceptor {get;set;}
            public IResourceFactoryCreator<T> ResourceFactoryCreator { get; set; }

            public IResourceFactory<T> ResourceFactory => null;

            public IComicSourceProvider SourceProvider => null;

            public string Address { get; set; }

            public IServiceProvider Host => null;

            public ComicEntity Entity { get; set; }

            public event Action<ComicVisiting<T>, string> Loading;
            public event Action<ComicVisiting<T>, ComicEntity> Loaded;
            public event Action<ComicVisiting<T>, int> LoadingChapter;
            public event Action<ComicVisiting<T>, ChapterWithPage> LoadedChapter;

            public void Dispose()
            {
            }

            public void EraseChapter(int index)
            {
            }

            public Task<IComicChapterManager<T>> GetChapterManagerAsync(int index)
            {
                return Task.FromResult<IComicChapterManager<T>>(null);
            }

            public Task<bool> LoadAsync(string address)
            {
                return Task.FromResult(true);
            }

#pragma warning disable CS1998
            public async Task LoadChapterAsync(int index)
            {
            }
#pragma warning restore CS1998
        }
        [TestMethod]
        public async Task GivenNullOrNotInRangeCall_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ComicVisitingExtensions.IsLoad<object>(null));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => ComicVisitingExtensions.GoToPageAsync<object>(null, default));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => ComicVisitingExtensions.DownloadChapterAsync<object>(null, 0));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => ComicVisitingExtensions.DownloadChapterAsync(new ComicVisiting<Stream>(new NullServiceProvider(), StreamResourceFactoryCreator.Default), -1));
        }

        private NullComicVisiting<T> MakeVisiting<T>(int chapterCount)
        {
            var visit = new NullComicVisiting<T>();
            visit.Entity = new ComicEntity
            {
                Chapters = Enumerable.Range(0, chapterCount).Select(x => new ComicChapter
                {
                    TargetUrl = "http://localhost:5000/" + x,
                    Title = x.ToString()
                }).ToArray()
            };
            return visit;
        }
        [TestMethod]
        public void WhenAddresNullOrNot_LoadResultIsDependencyIt()
        {
            var visit = MakeVisiting<object>(0);
            Assert.IsFalse(ComicVisitingExtensions.IsLoad(visit));
        }
    }
}
