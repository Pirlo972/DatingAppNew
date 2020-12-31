using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public async Task<string> CreateToken(AppUser user)
        {
            // Ajouter des revendications
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            //Récupère le role de l'utilisateur
            var roles = await _userManager.GetRolesAsync(user);

            //Ajout du role au token
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Créer des informations d'identification
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Décrire notre token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            // Ajouter un gestionnaire de token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Création du jeton
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Renvoyer le jeton
            return tokenHandler.WriteToken(token);
        }
    }
}