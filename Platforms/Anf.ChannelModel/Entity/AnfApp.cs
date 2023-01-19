using SecurityLogin.AppLogin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Anf.ChannelModel.Entity
{
    public class AnfApp : IdentityDbEntityBase,IAppInfo
    {
        [Required]
        [MaxLength(38)]
        public string AppKey { get; set; }

        [Required]
        [MaxLength(64)]
        public string AppSecret { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        public bool Enable { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public virtual AnfUser User { get; set; }

    }
}
