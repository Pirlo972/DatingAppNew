using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            // Ajouter des revendications
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // Créer des informations d'identification
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Décrire notre token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(30),
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