using Anf.ChannelModel.Response;
using Anf.ChannelModel.Results;
using Anf.WebService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<FlushKeyResponse>),200)]
        public async Task<IActionResult> FlushKey()
        {
            var key = await userService.FlushKeyAsync();
            var res = new EntityResult<FlushKeyResponse>
            {
                Data = new FlushKeyResponse { Identity = key.Identity, PublicKey = key.PublicKey }
            };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(EntityResult<string>), 200)]
        public async Task<IActionResult> Login([FromForm]string userName, [FromForm] string passwordHash,[FromForm] string connectId)
        {
            var tk = await userService.LoginAsync(connectId, userName, passwordHash);
            if (!string.IsNullOrEmpty(tk))
            {
                HttpContext.Response.Cookies.Append(AuthenticationConst.AuthHeader, tk, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    MaxAge = UserIdentityService.ExpireTime
                });
            }
            var res = new EntityResult<string> { Data = tk };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(EntityResult<bool>), 200)]
        public async Task<IActionResult> Registe([FromForm] string userName, [FromForm] string passwordHash, [FromForm] string connectId)
        {
            var succeed = await userService.RegisteAsync(connectId, userName, passwordHash);
            var res = new EntityResult<bool> { Data = succeed };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<bool>), 200)]
        public async Task<IActionResult> ResetPwd(string userName, string tk, string pwd)
        {
            var resetRes = await userService.RestPasswordAsync(HttpContext.Session.Id, userName, tk, pwd);
            var res = new EntityResult<bool> { Data = resetRes };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<bool>), 200)]
        public async Task<IActionResult> ResetPwdWithOld(string userName, string old, string pwd)
        {
            var resetRes = await userService.RestPasswordWithOldAsync(HttpContext.Session.Id, userName, old, pwd);
            var res = new EntityResult<bool> { Data = resetRes };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<string>), 200)]
        public async Task<IActionResult> GenerateResetToken(string userName)
        {
            //TODO:发邮件
            var tk = await userService.GenerateResetTokenAsync(userName);
            var res = new EntityResult<string> { Data = tk };
            return Ok(res);
        }
    }
}
