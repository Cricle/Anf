using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Anf.Platform.Models
{
    /// <summary>
    /// 存储模型
    /// </summary>
    public class ComicStoreModel : ComicEntity,INotifyPropertyChanged
    {
        private int currentChapter;
        private int currentPage;
        private bool superFavorite;

        /// <summary>
        /// 当前章节
        /// </summary>
        public int CurrentChapter
        {
            get => currentChapter;
            set => RaisePropertyChanged(ref currentChapter, value);
        }
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get => currentPage;
            set => RaisePropertyChanged(ref currentPage, value);
        }
        /// <summary>
        /// 超级喜欢
        /// </summary>
        public bool SuperFavorite
        {
            get => superFavorite;
            set => RaisePropertyChanged(ref superFavorite, value);
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged<T>(ref T prop,T value,[CallerMemberName]string name=null)
        {
            if (!EqualityComparer<T>.Default.Equals(prop,value))
            {
                prop = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
