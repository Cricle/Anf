using Anf.ChannelModel.Results;
using Anf.Web.Models;
using Anf.Web.Services;
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

        [HttpGet("[action]")]
        public async Task<IActionResult> FlushKey()
        {
            var key = await userService.FlushRSAKey(HttpContext.Session.Id);
            var res = new EntityResult<string> { Data = key };
            return Ok(res);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromForm]string userName, [FromForm] string passwordHash)
        {
            var succeed = await userService.LoginAsync(HttpContext.Session.Id, userName, passwordHash);
            var res = new EntityResult<bool> { Data = succeed };
            return Ok(res);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Registe([FromForm] string userName, [FromForm] string passwordHash)
        {
            var succeed = await userService.RegisteAsync(HttpContext.Session.Id, userName, passwordHash);
            var res = new EntityResult<bool> { Data = succeed };
            return Ok(res);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> ResetPwd(string userName, string tk, string pwd)
        {
            var resetRes = await userService.RestPasswordAsync(HttpContext.Session.Id, userName, tk, pwd);
            var res = new EntityResult<bool> { Data = resetRes };
            return Ok(res);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> ResetPwdWithOld(string userName, string old, string pwd)
        {
            var resetRes = await userService.RestPasswordWithOldAsync(HttpContext.Session.Id, userName, old, pwd);
            var res = new EntityResult<bool> { Data = resetRes };
            return Ok(res);
        }
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
