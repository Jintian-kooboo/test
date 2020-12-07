using API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.MiddleWare
{
    //https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api
    //https://github.com/cornflourblue/aspnet-core-3-jwt-authentication-api
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtTokenDto _jwtTokenDto;

        // Dependency Injection
        public JwtAuthenticationMiddleware(RequestDelegate next, IOptions<JwtTokenDto> jwtTokenDto)
        {
            _next = next;
            _jwtTokenDto = jwtTokenDto.Value;
        }

        //public async Task Invoke(HttpContext context)
        //{
        //    //Reading the AuthHeader which is signed with JWT
        //    string authHeader = context.Request.Headers["Authorization"];

        //    if (authHeader != null)
        //    {
        //        //Reading the JWT middle part           
        //        int startPoint = authHeader.IndexOf(".") + 1;
        //        int endPoint = authHeader.LastIndexOf(".");

        //        var tokenString = authHeader
        //            .Substring(startPoint, endPoint - startPoint).Split(".");
        //        var token = tokenString[0].ToString() + "==";

        //        var credentialString = Encoding.UTF8
        //            .GetString(Convert.FromBase64String(token));

        //        // Splitting the data from Jwt
        //        var credentials = credentialString.Split(new char[] { ':', ',' });

        //        // Trim this Username and UserRole.
        //        var userRule = credentials[5].Replace("\"", "");
        //        var userName = credentials[3].Replace("\"", "");

        //        // Identity Principal
        //        var claims = new[]
        //        {
        //       new Claim("name", userName),
        //       new Claim(ClaimTypes.Role, userRule),
        //   };
        //        var identity = new ClaimsIdentity(claims, "basic");
        //        context.User = new ClaimsPrincipal(identity);
        //    }
        //    //Pass to the next middleware
        //    await _next(context);
        //}

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                attachUserToContext(context,  token);

            await _next(context);
        }

        private void attachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtTokenDto.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach user to context on successful jwt validation
                context.Items["User"] = "32";
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
