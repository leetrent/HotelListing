using AutoMapper;
using HotelListing.Data;
using HotelListing.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly SignInManager<ApiUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApiUser> userManager, SignInManager<ApiUser> signInManager, IMapper mapper, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registration Attempt for {userDTO.Email} ");

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            try
            {
                ApiUser userEntity = _mapper.Map<ApiUser>(userDTO);
                IdentityResult identityResult = await _userManager.CreateAsync(userEntity);

                if (identityResult.Succeeded == false)
                {
                    _logger.LogError($"UserManager.CreateAsync failed for '{userEntity.Email}'");
                    _logger.LogError("Error Details:");
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        _logger.LogError($"{identityError.Code} - {identityError.Description}");
                        ModelState.AddModelError(identityError.Code, identityError.Description);
                    }

                    return BadRequest($"User registration attempt failed.");
                    //return BadRequest(ModelState);
                }
                return Accepted();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Something Went Wrong in the {nameof(Register)}");
                _logger.LogError(exc.Message);
                _logger.LogError(exc.StackTrace);

                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: StatusCodes.Status500InternalServerError);
            }

        }
    }
}
