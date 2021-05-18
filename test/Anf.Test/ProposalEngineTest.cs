using Anf.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test
{
    [TestClass]
    public class ProposalEngineTest
    {
        [TestMethod]
        public void InitProposalEngineUse_NullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ProposalEngine(null));
        }
        [TestMethod]
        public void ActiveType_TypeMustActived()
        {
            var prov = new NullIProposalProvider();
            var f = new ValueServiceScopeFactory
            {
                Factory = new Dictionary<Type, Func<object>>
                {
                    [typeof(NullIProposalProvider)] = ()=> prov
                }
            };
            var eng = new ProposalEngine(f);
            Assert.AreEqual(f, eng.ServiceScopeFactory);
            var obj = eng.Active(typeof(NullIProposalProvider));
            Assert.IsNotNull(obj.Provider);
            Assert.IsNotNull(obj.Scope);
            Assert.AreEqual(prov, obj.Provider);
            obj.Dispose();
        }
        [TestMethod]
        public void ActiveNoImplement_IProposalProvider_Type_MustThrowException()
        {
            var prov = new object();
            var f = new ValueServiceScopeFactory
            {
                Factory = new Dictionary<Type, Func<object>>
                {
                    [typeof(object)] = () => prov
                }
            };
            var eng = new ProposalEngine(f);
            Assert.ThrowsException<InvalidCastException>(() => eng.Active(typeof(object)));
        }
        [TestMethod]
        public void GivenAnyCondition_ActiveByIndex_MustBeActived()
        {
            var prov = new NullIProposalProvider();
            var f = new ValueServiceScopeFactory
            {
                Factory = new Dictionary<Type, Func<object>>
                {
                    [typeof(NullIProposalProvider)] = () => prov
                }
            };
            var eng = new ProposalEngine(f);
            eng.Add(new NullProposalDescription
            {
                DescritionUri = new Uri("http://localhost:4200"),
                Name = "any",
                ProviderType = typeof(NullIProposalProvider)
            });
            Assert.AreEqual(1, eng.Count);
            var obj = eng.Active(0);
            Assert.IsNotNull(obj.Provider);
            Assert.AreEqual(prov, obj.Provider);
            obj.Dispose();
        }
        [TestMethod]
        [DataRow(0,-1)]
        [DataRow(0, 0)]
        [DataRow(0, 1)]
        [DataRow(1, -1)]
        [DataRow(1, 1)]
        [DataRow(1, 2)]
        [DataRow(5, -5)]
        [DataRow(5, 5)]
        public void GivenAnyCondition_ActiveOutOfRang_MustThrowException(int count,int index)
        {
            var prov = new NullIProposalProvider();
            var f = new ValueServiceScopeFactory
            {
                Factory = new Dictionary<Type, Func<object>>
                {
                    [typeof(NullIProposalProvider)] = () => prov
                }
            };
            var eng = new ProposalEngine(f);
            for (int i = 0; i < count; i++)
            {
                eng.Add(new NullProposalDescription
                {
                    DescritionUri = new Uri("http://localhost:420"+i),
                    Name = "any" + i,
                    ProviderType = typeof(NullIProposalProvider)
                });
            }
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => eng.Active(index));
        }
    }
}
