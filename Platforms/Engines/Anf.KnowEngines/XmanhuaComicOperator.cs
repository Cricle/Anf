using Anf.Engine.Annotations;
using Anf.Networks;
using JavaScriptEngineSwitcher.Core;

namespace Anf.KnowEngines
{
    [ComicSourceProvider]
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
