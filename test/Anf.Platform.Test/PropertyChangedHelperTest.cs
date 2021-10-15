using GalaSoft.MvvmLight;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Test
{
    [TestClass]
    public class PropertyChangedHelperTest
    {
        class NullNotifyObject : ObservableObject
        {
            private string name;

            public string Name
            {
                get => name;
                set => Set(ref name, value);
            }
        }
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            var obj = new NullNotifyObject();
            Assert.ThrowsException<ArgumentNullException>(() => PropertyChangedHelper.Subscribe<NullNotifyObject>(obj, null, x => { }));
            Assert.ThrowsException<ArgumentNullException>(() => PropertyChangedHelper.Subscribe<NullNotifyObject>(obj, x => x, (Action)null));
            Assert.ThrowsException<ArgumentNullException>(() => PropertyChangedHelper.Subscribe<NullNotifyObject>(obj, x => x, (Action<object>)null));
            Assert.ThrowsException<ArgumentNullException>(() => PropertyChangedHelper.Subscribe<NullNotifyObject>(obj, x => x, (PropertyChangedEventHandler)null));
        }
        [TestMethod]
        public void Subscribed_PropertyChanged_MustRaised()
        {
            var obj = new NullNotifyObject();
            object sub = null;
            string name = null;
            var s=PropertyChangedHelper.Subscribe(obj, x => x.Name, (o,e) =>
            {
                sub = o;
                name = e.PropertyName;
            });
            obj.Name = "hello";
            Assert.AreEqual(obj, sub);
            Assert.AreEqual(nameof(NullNotifyObject.Name), name);
            s.Dispose();

            sub = null;
            name = null;
            obj.Name = "well";
            Assert.IsNull(name);
            Assert.IsNull(sub);
        }
    }
}
