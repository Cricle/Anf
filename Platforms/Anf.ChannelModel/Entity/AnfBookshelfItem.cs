﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Anf.ChannelModel.Entity
{
    public class AnfBookshelfItem
    {
        [Required]
        public long BookshelfId { get; set; }

        public string Address { get; set; }

        public int? ReadChatper { get; set; }

        public int? ReadPage { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public bool Like { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        [Required]
        public long UserId { get; set; }

        public virtual AnfBookshelf Bookshelf { get; set; }

        public virtual AnfUser User { get; set; }
    }
}
