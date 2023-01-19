using System;
using System.ComponentModel;

namespace Anf.ChannelModel.Entity
{
    [Flags]
    public enum WordType
    {
        /// <summary>
        /// 动画
        /// </summary>
        [Description("动画")]
        Animation='a',
        /// <summary>
        /// 漫画
        /// </summary>
        [Description("漫画")]
        Cartoon = 'b',
        /// <summary>
        /// 游戏
        /// </summary>
        [Description("游戏")]
        Game = 'c',
        /// <summary>
        /// 文学
        /// </summary>
        [Description("文学")]
        Literature = 'd',
        /// <summary>
        /// 原创
        /// </summary>
        [Description("原创")]
        Original = 'e',
        /// <summary>
        /// 来自网络
        /// </summary>
        [Description("来自网络")]
        Web = 'f',
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 'g',
        /// <summary>
        /// 影视
        /// </summary>
        [Description("影视")]
        Video = 'h',
        /// <summary>
        /// 诗词
        /// </summary>
        [Description("诗词")]
        Poetry = 'i',
        /// <summary>
        /// 网易云
        /// </summary>
        [Description("网易云")]
        NeteaseCloud = 'j',
        /// <summary>
        /// 哲学
        /// </summary>
        [Description("哲学")]
        Philosophy = 'k',
        /// <summary>
        /// 抖机灵
        /// </summary>
        [Description("抖机灵")]
        BeClever = 'l',
    }
}
