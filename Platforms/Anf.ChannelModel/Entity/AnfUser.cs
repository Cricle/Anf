using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Anf.ChannelModel.Entity
{
    public class AnfUser : IdentityUser<long>
    {
        public AnfUser()
        {
        }

        public AnfUser(string userName) : base(userName)
        {
        }

        public virtual ICollection<AnfBookshelfItem> BookshelfItems { get; set; }

        public virtual ICollection<AnfBookshelf> Bookshelves { get; set; }
    }
}
