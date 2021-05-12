using Anf.ChannelModel.Helpers;
using Anf.ChannelModel.Results;
using Anf.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> Go()
        {
            return Ok(11);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> FlushKey([FromQuery]string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }
            var key = await userService.FlushRSAKey(name);
            var res = new EntityResult<RSAKeyIdentity> { Data = key };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromForm]string userName, [FromForm] string passwordHash,[FromForm] string connectId)
        {
            var tk = await userService.LoginAsync(connectId, userName, passwordHash);
            var res = new EntityResult<string> { Data = tk };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Registe([FromForm] string userName, [FromForm] string passwordHash, [FromForm] string connectId)
        {
            var succeed = await userService.RegisteAsync(connectId, userName, passwordHash);
            var res = new EntityResult<bool> { Data = succeed };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> ResetPwd(string userName, string tk, string pwd)
        {
            var resetRes = await userService.RestPasswordAsync(HttpContext.Session.Id, userName, tk, pwd);
            var res = new EntityResult<bool> { Data = resetRes };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> ResetPwdWithOld(string userName, string old, string pwd)
        {
            var resetRes = await userService.RestPasswordWithOldAsync(HttpContext.Session.Id, userName, old, pwd);
            var res = new EntityResult<bool> { Data = resetRes };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GenerateResetToken(string userName)
        {
            //TODO:发邮件
            var tk = await userService.GenerateResetTokenAsync(userName);
            var res = new EntityResult<string> { Data = tk };
            return Ok(res);
        }
    }
}
