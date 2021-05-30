using Anf.Networks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.RemoteApi
{
    public class ReadingRemote
    {
        public RemoteSetting Setting { get; }


    }
    public class RemoteSetting
    {
        public INetworkAdapter NetworkAdapter { get; }

        public Uri Host { get; }

    }
}
