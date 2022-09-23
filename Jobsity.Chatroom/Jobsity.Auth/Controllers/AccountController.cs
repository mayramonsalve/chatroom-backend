using FluentValidation.Results;
using Jobsity.Auth.DTOs;
using Jobsity.Auth.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jobsity.Auth.Controllers
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
                                IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Register.
        /// </summary>
        /// <param name="registerDTO">Account information</param>
        /// <returns></returns>
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var decodedPassword = PasswordManagement.DecodePassword(registerDTO.Password);
            var decodedConfirmPassword = PasswordManagement.DecodePassword(registerDTO.ConfirmPassword);
            ValidationResult validationResult = PasswordManagement.FluentValidatePassword(decodedPassword, decodedConfirmPassword);
            if (validationResult.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(registerDTO.Username);
                if (existingUser == null)
                {
                    var user = new IdentityUser { UserName = registerDTO.Username, Email = registerDTO.Email };
                    var result = await _userManager.CreateAsync(user, decodedPassword);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        var response = new ApiResponse<bool>(true, "Account created successfully.");
                        return Ok(response);
                    }
                    else
                    {
                        var response = new ApiResponse<bool>(false, "Account could not be created.");
                        return BadRequest(response);
                    }
                }
                else
                {
                    var response = new ApiResponse<bool>(false, "Username already exists.");
                    return BadRequest(response);
                }
            }
            else
            {
                Dictionary<string, List<string>> errors = PasswordManagement.GetErrorObjectFromValidationResult(validationResult);
                var response = new ApiResponse<Dictionary<string, List<string>>>(errors, "Errors found.");
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Sign in.
        /// </summary>
        /// <param name="signInDTO">Sign in information</param>
        /// <returns></returns>
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDTO signInDTO)
        {
            var user = await _userManager.FindByNameAsync(signInDTO.Username);
            var decodedPassword = PasswordManagement.DecodePassword(signInDTO.Password);
            if (user != null && await _userManager.CheckPasswordAsync(user, decodedPassword))
            {
                var result = await _signInManager.PasswordSignInAsync(signInDTO.Username, signInDTO.Password, false, false);

                    var authClaims = new List<Claim>
                    {
                        new Claim("Username", user.UserName),
                        new Claim("Id",user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddDays(30),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                    string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                    var response = new ApiResponse<string>(jwtToken, "Signed in successfully.");
                    return Ok(response);
            }
            else
            {
                var response = new ApiResponse<bool>(false, "Email / Password don't match.");
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Sign out.
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            var response = new ApiResponse<bool>(true, "Signed out successfully.");
            return Ok(response);
        }
    }
}
