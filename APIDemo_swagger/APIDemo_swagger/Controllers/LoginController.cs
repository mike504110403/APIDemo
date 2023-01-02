using APIDemo_swagger.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace APIDemo_swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // 允許匿名
    public class LoginController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        private readonly IConfiguration _configuration; // JWT注入
        public LoginController(TodoContext todoContext, IConfiguration configuration)
        {
            _todoContext = todoContext;
            _configuration = configuration;
        }

        // 登入驗證api cookie
        [HttpPost]
        public string Login(LoginPost value)
        {
            var user = (from a in _todoContext.Employees
                        where a.Account == value.Account
                        && a.Password == value.Password
                        select a).SingleOrDefault();

            // 登入驗證
            if (user == null)
            {
                return "帳號或密碼錯誤";
            }
            else
            {
                var claims = new List<Claim> // 登入時宣告使用者資訊
                {
                    new Claim(ClaimTypes.Name, user.Account), // 驗證設定
                    new Claim("FullName", user.Name), // 其餘驗證屬性 - 使用者資訊
                    new Claim("EmployeeId", user.EmployeeId.ToString()),
                    new Claim(ClaimTypes.Role, "select") // 功能權限 selct為其中一個自定義的功能 // 可創一個資料表放使用者可使用的roles -> 還未寫
                };

                //var role = from a in _todoContext.Roles
                //           where a.EmployeeId == user.Name
                //           select a;

                //foreach (var temp in role)
                //{
                //    claims.Add(new Claim(ClaimTypes.Role, temp.Name));
                //}

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)); // 登入狀態

                return "Ok";
            }
        }
        public class LoginPost
        {
            public string Password { get; set; }
            public string Account { get; set; }
        }

        [HttpDelete]
        public void Logout()  // 登出狀態驗證
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
        }

        // 登入驗證api jwt
        [HttpPost("jwtLogin")]
        public string jwtLogin(LoginPost value)
        {
            var user = (from a in _todoContext.Employees
                        where a.Account == value.Account
                        && a.Password == value.Password
                        select a).SingleOrDefault();

            // 登入驗證
            if (user == null)
            {
                return "帳號或密碼錯誤";
            }
            else
            {

                var claims = new List<Claim> // 登入時宣告使用者資訊
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.Account), // 驗證設定
                    new Claim("FullName", user.Name), // 其餘驗證屬性 - 使用者資訊 要取得就要設定type
                    new Claim(JwtRegisteredClaimNames.NameId, user.EmployeeId.ToString()),
                    new Claim("EmployeeId", user.EmployeeId.ToString()),
                    new Claim(ClaimTypes.Role, "select") // 功能權限 selct為其中一個自定義的功能 // 可創一個資料表放使用者可使用的roles -> 還未寫
                };

                //var role = from a in _todoContext.Roles
                //           where a.EmployeeId == user.Name
                //           select a;

                //foreach (var temp in role)
                //{
                //    claims.Add(new Claim(ClaimTypes.Role, temp.Name));
                //}

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"])); //金鑰處理

                var jwt = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"], // 發行者
                    audience: _configuration["JWT:Audience"], // 給誰使用
                    claims: claims, // 使用者資訊
                    expires: DateTime.Now.AddMinutes(30), // 期限
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jwt); //寫token

                return token;
            }
        }  // 跨伺服器發行比cookie好用 但無法登出

        [HttpGet("NoLogin")]
        public string noLogout()
        {
            return "未登入";
        }

        [HttpGet("NoAcess")]
        public string noAcess()
        {
            return "沒有權限";
        }

    }
}
