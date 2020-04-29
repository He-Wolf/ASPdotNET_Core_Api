using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;
using AutoMapper;

namespace WebApiJwt.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<WebApiUser> _signInManager;
        private readonly UserManager<WebApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<WebApiUser> userManager,
            SignInManager<WebApiUser> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Logs in registered user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: POST
        ///     route:  /Account/Login
        ///     body:   {
        ///             "Email": "john.smith@email.com",
        ///             "Password": "SomeSecurePassword123!"
        ///             }
        ///     additional header: none
        /// </remarks>
        /// <param name="model"> User email and password.</param>
        /// <returns>A JWT for authorization.</returns>
        /// <response code="200">Returns the newly created JSON web token.</response>
        /// <response code="400">Message if login failed.</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status400BadRequest)]
        [Route("Login", Name="Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            
            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                var token = GenerateJwtToken(appUser);

                return Ok(new TokenViewModel(token, "Successful login", DateTime.Now));
            }
            
            return BadRequest(new MessageViewModel("Login failed", DateTime.Now));
        }

        /// <summary>
        /// Logs out registered, logged in user. Here we can wait for the token to expire or
        /// delete the token from the browser on the client side.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: POST
        ///     route:  /Account/Logout
        ///     body:   none
        ///     additional header:
        ///             key: authorization,
        ///             value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ZmI4N2Q0Zi0wNGZkLTQwNGUtOTUzZC1hODJmYjU4NjNmMGQiLCJqdGkiOiIxOTViNGM4Ni02M2JmLTQ4YzgtYTkyOC01ZjRjNjRjZmQ4Y2EiLCJleHAiOjE1ODc0ODg1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VnZcQD04z0nRlHOwtCtiBXheQGcO80BLYtKOYewZ4Mo
        /// </remarks>
        /// <param></param>
        /// <returns>Message if logout was succesful.</returns>
        /// <response code="200">Returns the message about logout.</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status200OK)]
        [Route("Logout", Name="Logout")]
        public async Task<IActionResult> Logout()
        {
            // Well, What do you want to do here?
            // Wait for token to get expired OR
            // Maintain token cache and invalidate the tokens after logout method is called

            await _signInManager.SignOutAsync(); //works only if cookie based authentication is used

            return Ok(new MessageViewModel("Successful logout.", DateTime.Now));
        }
        
        /// <summary>
        /// Registers a new user with the given user data. After successful registration, JWT is returned.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: POST
        ///     route:  /Account/Register
        ///     body:   {
	    ///             "FirstName": "John",
	    ///             "LastName": "Smith",
	    ///             "Email": "john.smith@email.com",
        ///             "Password": "SomeSecurePassword123!",
        ///             "ConfirmPassword": "SomeSecurePassword123!"
        ///             }
        ///     additional header: none
        ///
        /// Password needs upper and lower case letters from English alphabet, at least a digit and a special character (-._@+) as well.
        /// Length must be min. 6 character long.
        /// </remarks>
        /// <param name="model"> User firstname, lastname, email and password (with confirmation).</param>
        /// <returns>JSON web token if registration was successful.</returns>
        /// <response code="201">Returns JWT after registration.</response>
        /// <response code="400">Returns a message if registration data do not fit the requirements.</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status400BadRequest)]
        [Route("Register", Name="Register")]
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
                    var token = GenerateJwtToken(user);
                    
                    return CreatedAtRoute("Display", null, new TokenViewModel(token, "Successful registration", DateTime.Now));
                }
            }
            
            return BadRequest(new MessageViewModel("Registration failed.", DateTime.Now));
        }

        /// <summary>
        /// You can edit the user data of registered, logged in user account. JWT needed in the request header.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: PUT
        ///     route:  /Account/Edit
        ///     body:   {
	    ///             "FirstName": "Jonathan",
	    ///             "LastName": "Smith",
	    ///             "Email": "jonathan.smith@email.com",
        ///             "Password": "SomeSecurePassword123.",
        ///             "ConfirmPassword": "SomeSecurePassword123."
        ///             }
        ///     additional header:
        ///             key: authorization,
        ///             value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ZmI4N2Q0Zi0wNGZkLTQwNGUtOTUzZC1hODJmYjU4NjNmMGQiLCJqdGkiOiIxOTViNGM4Ni02M2JmLTQ4YzgtYTkyOC01ZjRjNjRjZmQ4Y2EiLCJleHAiOjE1ODc0ODg1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VnZcQD04z0nRlHOwtCtiBXheQGcO80BLYtKOYewZ4Mo
        /// </remarks>
        /// <param name="model"> User firstname, lastname, email and password (with confirmation).</param>
        /// <returns>New, modified user data of the logged-in user.</returns>
        /// <response code="200">User data after successful modification.</response>
        /// <response code="400">Returns a message if user data modification was not successful.</response>
        [Authorize]
        [HttpPut]
        [ProducesResponseType(typeof(DisplayViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status400BadRequest)]
        [Route("Edit", Name="Edit")]
        public async Task<IActionResult> Edit([FromBody] RegisterViewModel model)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrentUser = await _userManager.FindByIdAsync(CurrentUserId);

            if(ModelState.IsValid)
            {
                CurrentUser.FirstName = model.FirstName;
                CurrentUser.LastName = model.LastName;
                CurrentUser.Email = model.Email;
                CurrentUser.UserName = model.Email;
                CurrentUser.PasswordHash = _userManager.PasswordHasher.HashPassword(CurrentUser, model.Password);

                await _userManager.UpdateAsync(CurrentUser);

                return Ok(_mapper.Map<DisplayViewModel>(CurrentUser));
            }
            return BadRequest(new MessageViewModel("Edit failed.", DateTime.Now));
        }

        /// <summary>
        /// You can display the logged in user data. JWT needed in the request header.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: GET
        ///     route:  /Account/Display
        ///     body:   none
        ///     additional header:
        ///             key: authorization,
        ///             value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ZmI4N2Q0Zi0wNGZkLTQwNGUtOTUzZC1hODJmYjU4NjNmMGQiLCJqdGkiOiIxOTViNGM4Ni02M2JmLTQ4YzgtYTkyOC01ZjRjNjRjZmQ4Y2EiLCJleHAiOjE1ODc0ODg1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VnZcQD04z0nRlHOwtCtiBXheQGcO80BLYtKOYewZ4Mo
        /// </remarks>
        /// <param></param>
        /// <returns>Logged in user data.</returns>
        /// <response code="200">Logged-in user data.</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(DisplayViewModel), StatusCodes.Status200OK)]
        [Route("Display", Name="Display")]
        public async Task<IActionResult> Display()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrentUser = await _userManager.FindByIdAsync(CurrentUserId);

            return Ok(_mapper.Map<DisplayViewModel>(CurrentUser));
        }

        /// <summary>
        /// You can delete your user account. All the ToDo items of the user will be deleted as well. JWT needed in the request header.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: DELETE
        ///     route:  /Account/Delete
        ///     body:   none
        ///     additional header:
        ///             key: authorization,
        ///             value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ZmI4N2Q0Zi0wNGZkLTQwNGUtOTUzZC1hODJmYjU4NjNmMGQiLCJqdGkiOiIxOTViNGM4Ni02M2JmLTQ4YzgtYTkyOC01ZjRjNjRjZmQ4Y2EiLCJleHAiOjE1ODc0ODg1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VnZcQD04z0nRlHOwtCtiBXheQGcO80BLYtKOYewZ4Mo
        /// </remarks>
        /// <param></param>
        /// <returns>Message after deletion.</returns>
        /// <response code="200">Successful deletion with confirmation message.</response>
        [Authorize]
        [HttpDelete]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status200OK)]
        [Route("Delete", Name="Delete")]
        public async Task<IActionResult> Delete()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrentUser = await _userManager.FindByIdAsync(CurrentUserId);

            await _userManager.DeleteAsync(CurrentUser);

            return Ok(new MessageViewModel("Your account deleted successfully.", DateTime.Now));
        }
        
        private string GenerateJwtToken(WebApiUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            //_logger.LogInformation("user.id: {user.id}", user.Id);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:Expiration"]));

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