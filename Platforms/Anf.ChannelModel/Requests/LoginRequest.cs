using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.ChannelModel.Requests
{
    public class LoginRequest
    {
        public string UserName { get; set; }

        public string PasswordHash { get; set; }
    }
}
