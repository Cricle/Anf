using Anf.Platform.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Phone.Models
{
    public class PhoneStorableComicSourceInfo : StorableComicSourceInfo<PhoneComicStoreBox>
    {
        public PhoneStorableComicSourceInfo(ComicSnapshot snapshot, ComicSource source, IComicSourceCondition condition, PhoneComicStoreBox storeBox) : base(snapshot, source, condition, storeBox)
        {
        }

        protected override PhoneComicStoreBox CreateBox(FileInfo file)
        {
            return new PhoneComicStoreBox(file);
        }
    }
}
