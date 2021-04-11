using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class ObserverObjectTest
    {
        internal class Human: ObserverObject
        {
            private string name;
            private int age;

            public int Age
            {
                get { return age; }
                set
                {
                    age = value;
                    RaisePropertyChanged();
                }
            }

            public string Name
            {
                get { return name; }
                set => RaisePropertyChanged(ref name, value);
            }

        }
        [TestMethod]
        public void ListenPropertyChangedEvent_ChangeProperty_EventMustFired()
        {
            var human = new Human();
            object sender=null;
            string name = null;
            human.PropertyChanged += (o, e) =>
            {
                sender = o;
                name = e.PropertyName;
            };
            human.Name = "a";
            Assert.AreEqual(human, sender);
            Assert.AreEqual(nameof(Human.Name), name);
            sender = null;
            name = null;
            human.Name = "a";
            Assert.IsNull(sender);
            Assert.IsNull(name);
            sender = null;
            name = null;
            human.Age = 123;
            Assert.AreEqual(human, sender);
            Assert.AreEqual(nameof(Human.Age), name);
        }
    }
}
