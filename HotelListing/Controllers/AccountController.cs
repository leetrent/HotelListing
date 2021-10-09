using AutoMapper;
using HotelListing.Data;
using HotelListing.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApiUser> userManager, IMapper mapper, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registration attempt for {userDTO.Email} ");

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            try
            {
                ApiUser userEntity = _mapper.Map<ApiUser>(userDTO);
                userEntity.UserName = userEntity.Email;
                IdentityResult identityResult = await _userManager.CreateAsync(userEntity, userDTO.Password);

                if (identityResult.Succeeded == false)
                {
                    _logger.LogError($"UserManager.CreateAsync failed for '{userEntity.Email}'");
                    _logger.LogError("Error Details:");
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        _logger.LogError($"{identityError.Code} - {identityError.Description}");
                        ModelState.AddModelError(identityError.Code, identityError.Description);
                    }

                    return BadRequest(ModelState);
                }
                return Accepted();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Something Went Wrong in the {nameof(Register)}");
                _logger.LogError(exc.Message);
                _logger.LogError(exc.StackTrace);

                return Problem($"Exception encountered in {nameof(Register)}", statusCode: StatusCodes.Status500InternalServerError);
            }

        }

        //[HttpPost]
        //[Route("login")]
        //public async Task<IActionResult> Login([FromBody] LoginUserDTO loginDTO)
        //{
        //    _logger.LogInformation($"Login attempt for {loginDTO.Email} ");

        //    if (ModelState.IsValid == false)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        SignInResult signInResult = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false);

        //        if (signInResult.Succeeded == false)
        //        {
        //            _logger.LogError($"SignInManager.PasswordSignInAsync failed for '{loginDTO.Email}'");
        //            return Unauthorized(loginDTO);
        //        }
        //        return Accepted();
        //    }
        //    catch (Exception exc)
        //    {
        //        _logger.LogError(exc, $"Exception encountered in {nameof(Login)}");
        //        _logger.LogError(exc.Message);
        //        _logger.LogError(exc.StackTrace);

        //        return Problem($"Exception encountered in {nameof(Login)}", statusCode: StatusCodes.Status500InternalServerError);
        //    }
        //}
    }
}
