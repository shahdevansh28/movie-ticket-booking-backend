using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using movie_ticket_booking.Models;
using movie_ticket_booking.Models.DTO;
using movie_ticket_booking.Models.EmailService;
using movie_ticket_booking.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace movie_ticket_booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly IMailService mailService;

        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailService mailService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.mailService = mailService;
        }

        
        [HttpGet]
        [Route("getUser")]
        public async Task<ActionResult<User>> GetUser(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var userExist = await userManager.FindByNameAsync(model.Username);
            if (userExist != null)
                return StatusCode(StatusCodes.Status409Conflict, new Models.DTO.Response() { Status = "Error", Message = "User Already Exist" });

            User user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await userManager.CreateAsync(user, model.Password);
            
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Models.DTO.Response() { Status = "Error", Message = "User creatiion Failed" });
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.User))
                await userManager.AddToRoleAsync(user,UserRoles.User);


            //generate verification code
            Random verificationCode = new Random();
            int code = verificationCode.Next(10000000, 99999999);

            //send an email with verification code
            MailRequest mailRequest = new MailRequest();
            mailRequest.ToEmail = user.Email;
            mailRequest.Subject = "Verification Code";
            mailRequest.Body = "Your verification code is " + user.SecurityStamp[..8];

            await mailService.SendEmailAsync(mailRequest);

            return Ok(new Models.DTO.RegisterResponseDTO() { Email = user.Email, Status = "200" });
        }

        [HttpPost]
        [Route("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody]VerificationCodeDTO verificationCodeDTO)
        {
            //Application password for gmail :- imnd bbkj jpjp zgqs
            try { 
            var userExist = await userManager.FindByEmailAsync(verificationCodeDTO.mail);
            
            if (userExist != null)
            {
                //if verification code matches
                if (verificationCodeDTO.codeByUser.Equals(userExist.SecurityStamp[..8]))
                {
                    return Ok( new Models.DTO.Response() { Status = "Succes", Message = "User created Succesfully" });

                }
                else
                {
                    await userManager.DeleteAsync(userExist);
                    return StatusCode(StatusCodes.Status409Conflict, new Models.DTO.Response() { Status = "Failed", Message = "Verification code is not matched" });
                }

                //than send success code
                //else delete user 
                //send rejection code
            }
            else
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }
            return Ok("Some Problem");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO model)
        {
            var userExist = await userManager.FindByNameAsync(model.Username);
            if (userExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Models.DTO.Response() { Status = "Error", Message = "User Already Exist" });

            User user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Models.DTO.Response() { Status = "Error", Message = "User creatiion Failed" });
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
                await userManager.AddToRoleAsync(user, UserRoles.Admin);

            return Ok(new Models.DTO.Response() { Status = "Succes", Message = "User created Succesfully" });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecurityKey"]));
                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(5),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user
                });
            }
            return Unauthorized();
        }
    }
}
