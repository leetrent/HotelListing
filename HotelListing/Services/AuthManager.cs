using HotelListing.Data;
using HotelListing.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _apiUser;

        public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public Task<string> CreateToken()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateUser(LoginUserDTO userDTO)
        {
            _apiUser = await _userManager.FindByNameAsync(userDTO.UserName);

            if (_apiUser == null) { return false; }

            return await _userManager.CheckPasswordAsync(_apiUser, userDTO.Password);
        }

        private SigningCredentials GetSigningCredentials()
        {
            string key = Environment.GetEnvironmentVariable("KEY");
            SymmetricSecurityKey secret = new (Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            if (_apiUser == null) { return new List<Claim>(); }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _apiUser.UserName)
            };

            IList<string> roles = await _userManager.GetRolesAsync(_apiUser);
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
