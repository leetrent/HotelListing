using AutoMapper;
using HotelListing.Data;
using HotelListing.DTO;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace HotelListing.Controllers
{
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/{v:apiversion}/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IAuthManager _authManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApiUser> userManager, IAuthManager authManager, IMapper mapper, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _authManager = authManager;
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

            await _userManager.AddToRolesAsync(userEntity, userDTO.Roles);
            return Accepted();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            _logger.LogInformation($"Login attempt for {userDTO.Email} ");

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            if (await _authManager.ValidateUser(userDTO) == false)
            {
                return Unauthorized();
            }

            return Accepted(new { Token = await _authManager.CreateToken() });
        }
    }
}
