using Kw.Comic.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kw.Comic.Models
{
    public class Bookshelf 
    {
        [Key]
        public string ComicUrl { get; set; }

        /// <summary>
        /// 当前阅读的章节
        /// </summary>
        public int ReadChapter { get; set; }
        /// <summary>
        /// 当前阅读的页数
        /// </summary>
        public int ReadPage { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        [NotMapped]
        public ComicEntity Entity { get; set; }
    }
}
