using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Security.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; set; }
        private TokenOptions tokenOptions;
        DateTime _accessTokenExpiration;
        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            _accessTokenExpiration = DateTime.Now.AddMinutes(tokenOptions.AccessTokenExpiration);
            
        }

        public AccessToken CreateToken(User user, List<OperationClaim> operationClaims)
        {
            var securityKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey);
            var signingCredential = SigningCredentialHelper.CreateSigningCredential(securityKey);
            var jwt = CreateJwtSecurityToken(user, signingCredential, operationClaims, tokenOptions);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token= tokenHandler.WriteToken(jwt);
            return new AccessToken
            {
                Expiration = _accessTokenExpiration,
                Token = token
            };

        }

        public JwtSecurityToken CreateJwtSecurityToken(User user, SigningCredentials signingCredentials, List<OperationClaim> operationClaims, TokenOptions tokenOptions)
        {
            var jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                claims: SetClaims(user, operationClaims),
                signingCredentials: signingCredentials,
                notBefore: DateTime.Now,
                expires: _accessTokenExpiration
                ) ;
            return jwt;
        }

        public IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims)
        {
            var claims = new List<Claim>();
            claims.AddEmail(user.EMail);
            claims.AddName($"{user.FirstName} {user.LastName}");
            claims.AddNameIdentifier(user.Id.ToString());
            claims.AddRoles(operationClaims.Select(o => o.Name).ToArray());
            return claims;
        }

    }
}
