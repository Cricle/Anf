using Anf.ChannelModel;
using Anf.ChannelModel.Entity;
using Microsoft.AspNetCore.Identity;
using SecurityLogin;
using SecurityLogin.Mode.RSA;
using System.Threading.Tasks;

namespace Anf.WebService
{
    public class UserService : RSALoginService
    {
        private readonly UserIdentityService userIdentityService;
        private readonly UserManager<AnfUser> userManager;

        public UserService(ILockerFactory lockerFactory,
            ICacheVisitor cacheVisitor,
            UserIdentityService userIdentityService,
            UserManager<AnfUser> userManager)
            : base(lockerFactory, cacheVisitor)
        {
            this.userIdentityService = userIdentityService;
            this.userManager = userManager;
        }
        protected override string GetSharedIdentityKey()
        {
            return "Anf.WebService.UserService.IdentityKey";
        }
        protected override string GetSharedLockKey()
        {
            return "Anf.WebService.UserService.SharedLockKey";
        }
        protected override string GetHeader()
        {
            return "Anf.WebService.UserService.Header";
        }
        public Task<string> GenerateResetTokenAsync(string userName)
        {
            var user = new AnfUser { UserName = userName };
            return userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<bool> RestPasswordAsync(string connectId, string userName, string resetToken, string @new)
        {
            var pwdNew = await DecryptAsync(connectId, @new);
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
            var pwdOld = await DecryptAsync(connectId, old);
            if (string.IsNullOrEmpty(pwdOld))
            {
                return false;
            }
            var pwdNew = await DecryptAsync(connectId, @new);
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
            var pwd = await DecryptAsync(connectId, passwordHash);
            if (string.IsNullOrEmpty(pwd))
            {
                return false;
            }
            var user = new AnfUser { UserName = userName };
            var identity = await userManager.CreateAsync(user, pwd);
            return identity.Succeeded;
        }
        public async Task<string> LoginAsync(string connectId, string userName, string passwordHash)
        {
            var val = await DecryptAsync(connectId, passwordHash);
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }
            var u = await userManager.FindByNameAsync(userName);
            var ok = await userManager.CheckPasswordAsync(u, val);
            if (ok)
            {
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
