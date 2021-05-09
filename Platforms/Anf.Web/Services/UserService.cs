using Anf.ChannelModel.Helpers;
using Anf.ChannelModel.KeyGenerator;
using Anf.Web.Models;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Services
{
    public class UserService
    {
        private static readonly string RSAKey = "Anf.Web.Services.UserService.RSAKey";
        private static readonly TimeSpan RSAKeyCacheTime = TimeSpan.FromMinutes(5);

        private readonly IDatabase database;
        private readonly UserManager<AnfUser> userManager;

        public UserService(IDatabase database, UserManager<AnfUser> userManager)
        {
            this.database = database;
            this.userManager = userManager;
        }

        public async Task<string> FlushRSAKey(string connectId)
        {
            var rsaKey = RSAHelper.GenerateRSASecretKey(128);
            var key = RedisKeyGenerator.Concat(RSAKey, connectId);
            await database.StringSetAsync(key, rsaKey.PrivateKey, RSAKeyCacheTime);
            return rsaKey.PrivateKey;
        }
        public Task<string> GenerateResetTokenAsync(string userName)
        {
            var user = new AnfUser { UserName = userName };
            return userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<bool> RestPasswordAsync(string connectId,string userName,string resetToken,string @new)
        {
            var privateKey = await GetPrivateKeyAsync(connectId);
            if (privateKey is null)
            {
                return false;
            }
            var pwdNew = RSAHelper.RSADecrypt(privateKey, @new);
            if (string.IsNullOrEmpty(pwdNew))
            {
                return false;
            }
            var user = new AnfUser { UserName = userName };
            var ok = await userManager.ResetPasswordAsync(user, resetToken, pwdNew);
            return ok.Succeeded;
        }
        public async Task<bool> RestPasswordWithOldAsync(string connectId, string userName, string old, string @new)
        {
            var privateKey = await GetPrivateKeyAsync(connectId);
            if (privateKey is null)
            {
                return false;
            }
            var pwdOld = RSAHelper.RSADecrypt(privateKey, old);
            if (string.IsNullOrEmpty(pwdOld))
            {
                return false;
            }
            var pwdNew = RSAHelper.RSADecrypt(privateKey, @new);
            if (string.IsNullOrEmpty(pwdNew))
            {
                return false;
            }
            var user = new AnfUser { UserName = userName };
            var ok = await userManager.ChangePasswordAsync(user, pwdOld, pwdNew);
            return ok.Succeeded;
        }
        public async Task<bool> RegisteAsync(string connectId, string userName, string passwordHash)
        {
            var privateKey = await GetPrivateKeyAsync(connectId);
            if (privateKey is null)
            {
                return false;
            }
            var pwd = RSAHelper.RSADecrypt(privateKey, passwordHash);
            if (string.IsNullOrEmpty(pwd))
            {
                return false;
            }
            var user = new AnfUser { UserName = userName };
            var identity=await userManager.CreateAsync(user, pwd);
            return identity.Succeeded;
        }
        private async Task<string> GetPrivateKeyAsync(string connectId)
        {
            var key = RedisKeyGenerator.Concat(RSAKey, connectId);
            var privateKeyValue = await database.StringGetAsync(key);
            if (!privateKeyValue.HasValue)
            {
                return null;
            }
            return privateKeyValue.ToString();
        }
        public async Task<bool> LoginAsync(string connectId,string userName,string passwordHash)
        {
            var privateKey = await GetPrivateKeyAsync(connectId);
            if (privateKey is null)
            {
                return false;
            }
            var val = RSAHelper.RSADecrypt(privateKey, passwordHash);
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            var user = new AnfUser
            {
                UserName = userName
            };
            var ok=await userManager.CheckPasswordAsync(user,val);
            return ok;
        }
    }
}
