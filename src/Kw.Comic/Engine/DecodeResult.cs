using System.Text;

namespace Kw.Comic.Engine
{
    public class DecodeResult
    {
        public DecodeResult(bool succeed, string markMsg, StringBuilder value)
        {
            Succeed = succeed;
            MarkMsg = markMsg;
            Value = value;
        }

        public bool Succeed { get; }

        public string MarkMsg { get; }

        public StringBuilder Value { get; }

        public static DecodeResult MakeFail(string markMsg)
        {
            return new DecodeResult(false, markMsg, null);
        }
        public static DecodeResult MakeSucceed(StringBuilder builder)
        {
            return new DecodeResult(true, null, builder);
        }
        public static DecodeResult MakeSucceed(string value)
        {
            return MakeSucceed(new StringBuilder(value));
        }
    }
}
