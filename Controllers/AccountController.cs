using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;

namespace WebApiJwt.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<WebApiUser> _signInManager;
        private readonly UserManager<WebApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<WebApiUser> userManager,
            SignInManager<WebApiUser> signInManager,
            IConfiguration configuration,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            
            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                var token = GenerateJwtToken(model.Email, appUser);
                return new OkObjectResult(new { Token = token, Message = "Successful login" });
            }
            
            return new BadRequestObjectResult(new { Message = "Login failed", currentDate = DateTime.Now });
        }
        
        [Authorize]
        [HttpPost]
        [Route("Logout")]
        public async Task<object> Logout()
        {
            // Well, What do you want to do here?
            // Wait for token to get expired OR
            // Maintain token cache and invalidate the tokens after logout method is called

            await _signInManager.SignOutAsync(); //works only if cookie based authentication is used

            var result = new OkObjectResult(new { message = "You have successfully logged out.", currentDate = DateTime.Now });
            return result;
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new WebApiUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    var token = GenerateJwtToken(model.Email, user);
                    return new OkObjectResult(new { Token = token, Message = "Successful registration" });
                }
            }

            return new BadRequestObjectResult(new { Message = "Registration failed", currentDate = DateTime.Now });
        }
        
        private object GenerateJwtToken(string email, WebApiUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            _logger.LogInformation("user.id: {user.id}", user.Id);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}