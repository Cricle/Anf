using Anf.ChannelModel;
using Anf.ChannelModel.Entity;
using Anf.ChannelModel.Helpers;
using Anf.ChannelModel.KeyGenerator;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Anf.WebService
{
    public class UserService
    {
        private static readonly string RSAKey = "Anf.Web.Services.UserService.RSAKey";
        private static readonly TimeSpan RSAKeyCacheTime = TimeSpan.FromMinutes(3);

        private readonly IDatabase database;
        private readonly UserIdentityService userIdentityService;
        private readonly UserManager<AnfUser> userManager;

        public UserService(IDatabase database,
            UserIdentityService userIdentityService,
            UserManager<AnfUser> userManager)
        {
            this.userIdentityService = userIdentityService;
            this.database = database;
            this.userManager = userManager;
        }

        public async Task<RSAKeyIdentity> FlushRSAKey()
        {
            var rsaKey = RSAHelper.GenerateRSASecretKey();
            var identity = Guid.NewGuid().ToString();
            var key = RedisKeyGenerator.Concat(RSAKey,identity);
            await database.StringSetAsync(key, rsaKey.PrivateKey, RSAKeyCacheTime);
            return new RSAKeyIdentity
            {
                Key = rsaKey.PublicKey,
                Identity = identity
            };
        }
        public Task<string> GenerateResetTokenAsync(string userName)
        {
            var user = new AnfUser { UserName = userName };
            return userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<bool> RestPasswordAsync(string connectId, string userName, string resetToken, string @new)
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
            var identity = await userManager.CreateAsync(user, pwd);
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
        public async Task<string> LoginAsync(string connectId, string userName, string passwordHash)
        {
            var privateKey = await GetPrivateKeyAsync(connectId);
            if (privateKey is null)
            {
                return null;
            }
            var val = RSAHelper.RSADecrypt(privateKey, passwordHash);
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }
            var u = await userManager.FindByNameAsync(userName);
            var ok = await userManager.CheckPasswordAsync(u, val);
            if (ok)
            {
                var key = RedisKeyGenerator.Concat(RSAKey, connectId);
                await database.KeyDeleteAsync(key);
                var identity = new UserSnapshot
                {
                    Email = u.Email,
                    Id = u.Id,
                    Name = u.NormalizedUserName
                };
                var tk = await userIdentityService.SetIdentityAsync(identity);
                return tk;
            }
            return null;
        }
    }
}
