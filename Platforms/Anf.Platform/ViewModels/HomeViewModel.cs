using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Anf;
using Anf.Models;
using Anf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Anf.Platform.Services;
using Anf.Engine;
using Anf.Platform.Models;
using System.Net.Http;
using Anf.Networks;

namespace Anf.ViewModels
{
    public abstract class HomeViewModel<TSourceInfo,TImage> : ViewModelBase,IDisposable
        where TSourceInfo:ComicSourceInfo
    {
        public const int PageSize = 50;


        public HomeViewModel()
        {
            ComicEngine = AppEngine.GetRequiredService<ComicEngine>();
            SearchEngine = AppEngine.GetRequiredService<SearchEngine>();
            ProposalEngine = AppEngine.GetRequiredService<ProposalEngine>();

            SearchCommand = new RelayCommand(() => _ = SearchAsync());
            GoSourceCommand = new RelayCommand(GoSource);
            SetAndSearchCommand = new RelayCommand<string>(x => _ = SetAndSearchAsync(x));
            ResetCommand = new RelayCommand(Reset);
            SetCurrentCommand = new RelayCommand<ComicSnapshotInfo<TSourceInfo>>(SetCurrent);
            CopyCommand = new RelayCommand<string>(Copy);
            scope = AppEngine.CreateScope();

            observableCollectionFactory = AppEngine.GetRequiredService<IObservableCollectionFactory>();
            SearchProviders = observableCollectionFactory.Create<ISearchProvider>();
            foreach (var item in SearchEngine)
            {
                var sp = (ISearchProvider)scope.ServiceProvider.GetRequiredService(item);
                SearchProviders.Add(sp);
            }
            CurrentSearchProvider = SearchProviders.FirstOrDefault();

            Snapshots = observableCollectionFactory.Create<ComicSnapshotInfo<TSourceInfo>>();
            ProposalSnapshots = observableCollectionFactory.Create<ComicSnapshotInfo<TSourceInfo>>();
            EngineIcons = observableCollectionFactory.Create<EngineInfo<TImage>>();
        }
        protected readonly IServiceScope scope;
        private string keyword;
        private bool emptySet=true;
        private bool searching;
        private int additionCount;
        private SearchComicResult searchResult;
        private ComicSnapshotInfo<TSourceInfo> currentComicSnapshot;
        private ISearchProvider currentSearchProvider;
        private int skip;
        private int take=PageSize;
        private IComicSourceCondition avaliableCondition;
        private bool hasAvaliableCondition;
        private bool proposalLoading;
        private IProposalDescription selectedProposal;

        public IProposalDescription SelectedProposal
        {
            get { return selectedProposal; }
            set
            {
                var old = selectedProposal;
                Set(ref selectedProposal, value);
                if (old!=value)
                {
                    OnSelectedProposalChanged(value);
                }
            }
        }
        
        public bool ProposalLoading
        {
            get { return proposalLoading; }
            private set => Set(ref proposalLoading, value);
        }

        public bool HasAvaliableCondition
        {
            get { return hasAvaliableCondition; }
            private set => Set(ref hasAvaliableCondition, value);
        }

        public IComicSourceCondition AvaliableCondition
        {
            get { return avaliableCondition; }
            private set
            {
                Set(ref avaliableCondition, value);
                HasAvaliableCondition = value != null;
            }
        }

        public int Take
        {
            get { return take; }
            set => Set(ref take, value);
        }

        public int Skip
        {
            get { return skip; }
            set => Set(ref skip, value);
        }

        public ISearchProvider CurrentSearchProvider
        {
            get { return currentSearchProvider; }
            set => Set(ref currentSearchProvider, value);
        }

        public ComicSnapshotInfo<TSourceInfo> CurrentComicSnapshot
        {
            get { return currentComicSnapshot; }
            set
            {
                Set(ref currentComicSnapshot, value);
                OnCurrentComicSnapshotChanged(value);
            }
        }

        /// <summary>
        /// 搜索的结果
        /// </summary>
        public SearchComicResult SearchResult
        {
            get { return searchResult; }
            private set => Set(ref searchResult, value);
        }

        /// <summary>
        /// 新添加的数量
        /// </summary>
        public int AdditionCount
        {
            get { return additionCount; }
            private set => Set(ref additionCount, value);
        }

        /// <summary>
        /// 是否正在搜索
        /// </summary>
        public bool Searching
        {
            get { return searching; }
            private set
            {
                Set(ref searching, value);
                OnSearchingChanged(value);
            }
        }
        /// <summary>
        /// 当前是否空集合
        /// </summary>
        public bool EmptySet
        {
            get { return emptySet; }
            private set => Set(ref emptySet, value);
        }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword
        {
            get { return keyword; }
            set 
            {
                Set(ref keyword, value);
                AvaliableCondition = null;
                if (UrlHelper.IsWebsite(value))
                {
                    try
                    {
                        var addr = value.GetUrl();
                        AvaliableCondition = ComicEngine.GetComicSourceProviderType(addr);
                    }
                    catch (Exception) { }
                }
            }
        }
        /// <summary>
        /// 漫画解析引擎
        /// </summary>
        public ComicEngine ComicEngine { get; }
        /// <summary>
        /// 搜索引擎
        /// </summary>
        public SearchEngine SearchEngine { get; }

        public IList<EngineInfo<TImage>> EngineIcons { get; }
        public HistoryService HistoryService { get; } = AppEngine.GetService<HistoryService>();
        /// <summary>
        /// 搜索命令
        /// </summary>
        public ICommand SearchCommand { get; }
        public ICommand GoSourceCommand { get; }
        public ICommand ResetCommand { get; }
        public RelayCommand<string> CopyCommand { get; }
        public RelayCommand<ComicSnapshotInfo<TSourceInfo>> SetCurrentCommand { get; }
        public RelayCommand<string> SetAndSearchCommand { get; }
        /// <summary>
        /// 漫画快照
        /// </summary>
        public IList<ComicSnapshotInfo<TSourceInfo>> Snapshots { get; }
        public IList<ComicSnapshotInfo<TSourceInfo>> ProposalSnapshots { get; }
        public IList<ISearchProvider> SearchProviders { get; }
        public ProposalEngine ProposalEngine { get; }
        private readonly IObservableCollectionFactory observableCollectionFactory;
        private IPlatformService PlatformService { get; } = AppEngine.GetRequiredService<IPlatformService>();
        public void Copy(string data)
        {
            PlatformService.Copy(data);
        }

        public Task SetAndSearchAsync(string keywork)
        {
            Keyword = keywork;
            return SearchAsync();
        }
        public void SetCurrent(ComicSnapshotInfo<TSourceInfo> info)
        {
            CurrentComicSnapshot = info;
        }
        public async Task UpdateProposalAsync(int count)
        {
            if (ProposalLoading || SelectedProposal is null) 
            {
                return;
            }
            ProposalLoading = true;
            try
            {
                var proposal = ProposalEngine.Active(SelectedProposal.ProviderType);
                var datas = await proposal.Provider.GetProposalAsync(count);
                observableCollectionFactory.AddRange(ProposalSnapshots, datas.Select(x => CreateSnapshotInfo(x)));
            }
            finally
            {
                ProposalLoading = false;
            }
        }
        protected virtual void OnSelectedProposalChanged(IProposalDescription description)
        {

        }
        public async Task LoadEngineIcons(Action<IComicSourceCondition,Exception> eceptionHandler=null)
        {
            DisposeLogo();
            EngineIcons.Clear();
            var httpClient = AppEngine.GetRequiredService<INetworkAdapter>();
            var streamTransfer = AppEngine.GetRequiredService<IStreamImageConverter<TImage>>();
            foreach (var item in ComicEngine)
            {
                var addr = item.FaviconAddress;
                if (addr is null)
                {
                    continue;
                }
                try
                {
                    var reqSetting = new RequestSettings
                    {
                        Address=addr.AbsoluteUri
                    };
                    using (var stream = await httpClient.GetStreamAsync(reqSetting))
                    {
                        var bitmap =await streamTransfer.ToImageAsync(stream);
                        EngineIcons.Add(new EngineInfo<TImage> { Bitmap = bitmap, Condition = item });
                    }
                }
                catch (Exception ex)
                {
                    var exSer=AppEngine.GetService<ExceptionService>();
                    if (exSer!=null)
                    {
                        exSer.Exception = ex;
                    }
                    eceptionHandler?.Invoke(item,ex);
                }
            }
        }
        private void DisposeLogo()
        {
            foreach (var item in EngineIcons)
            {
                if (item .Bitmap is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            EngineIcons.Clear();
        }
        /// <summary>
        /// 执行搜索
        /// </summary>
        /// <returns></returns>
        public async Task SearchAsync()
        {
            if (CurrentSearchProvider is null)
            {
                return;
            }
            Searching = true;
            try
            {
                DisposeSnapshot();
                OnBeginSearch();
                Snapshots.Clear();
                var keyword = Keyword;
                SearchResult = await CurrentSearchProvider.SearchAsync(keyword, Skip, Take);
                InsertDatas();
                OnEndSearch();
                if (HistoryService != null)
                {
                    HistoryService.Lines.Add(keyword);
                }
            }
            finally
            {
                Searching = false;
            }
        }
        public void Reset()
        {
            Snapshots.Clear();
            EmptySet = true;
        }
        public void GoSource()
        {
            if (HasAvaliableCondition)
            {
                var nav = AppEngine.GetRequiredService<IComicTurnPageService>();
                var addr = keyword.GetUrl();
                if (HistoryService != null)
                {
                    HistoryService.Lines.Add(addr);
                }
                nav.GoSource(addr);
            }
        }
        protected virtual void OnBeginSearch()
        {

        }
        protected virtual void OnEndSearch()
        {

        }
        private void InsertDatas()
        {
            var sn = SearchResult.Snapshots;
            if (sn != null)
            {
                observableCollectionFactory.AddRange(Snapshots,sn.Select(x => CreateSnapshotInfo(x)));
                AdditionCount = sn.Length;
            }
            else
            {
                SearchResult = null;
                AdditionCount = 0;
            }
            EmptySet = Snapshots.Count == 0;
        }
        protected abstract ComicSnapshotInfo<TSourceInfo> CreateSnapshotInfo(ComicSnapshot info);
        protected virtual void OnSearchingChanged(bool res)
        {

        }
        protected virtual void OnCurrentComicSnapshotChanged(ComicSnapshotInfo<TSourceInfo> info)
        {

        }
        protected void DisposeSnapshot()
        {
            foreach (var item in Snapshots)
            {
                if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public virtual void Dispose()
        {
            scope.Dispose();
            DisposeSnapshot();
            DisposeLogo();
        }
    }

}
