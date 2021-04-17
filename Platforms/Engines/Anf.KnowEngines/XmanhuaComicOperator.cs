using Anf.Networks;
using JavaScriptEngineSwitcher.Core;

namespace Anf.KnowEngines
{
    public class XmanhuaComicOperator : MangabzComicOperator
    {
        public XmanhuaComicOperator(INetworkAdapter networkAdapter, IJsEngine jsEngine) : base(networkAdapter, jsEngine)
        {
        }

        protected override string GetBaseAddress()
        {
            return "http://www.xmanhua.com";
        }
    }
}
