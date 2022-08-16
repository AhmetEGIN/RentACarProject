using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        ITokenHelper _tokenHelper;
        IUserService _userService;
        public AuthManager(ITokenHelper tokenHelper, IUserService userService)
        {
            _tokenHelper = tokenHelper;
            _userService = userService;
        }

        public IDataResult<User> CheckUser(string mail)
        {
            var result = _userService.GetByMail(mail);
            if (!result.Success)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }
            return new SuccessDataResult<User>(result.Data);
        }

        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            var claims = _userService.GetClaims(user).Data;
            var token = _tokenHelper.CreateToken(user, claims);
            return new SuccessDataResult<AccessToken>(token);
        }

        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            var user = CheckUser(userForLoginDto.EMail);
            if (!user.Success)
            {
                return new ErrorDataResult<User>(user.Message);
            }
            var result = HashingHelper.VerifyPassword(userForLoginDto.Password, user.Data.PasswordHash, user.Data.PasswordSalt);
            if (result==false)
            {
                return new ErrorDataResult<User>(Messages.UserPasswordError);
            }
            return new SuccessDataResult<User>(user.Data);
        }

        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto)
        {
            var result = CheckUser(userForRegisterDto.EMail);
            if (!result.Success)
            {
                return new ErrorDataResult<User>(result.Message);
            }
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreateHash(userForRegisterDto.Password, out passwordHash, out passwordSalt);
            var user = new User
            {
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                EMail = userForRegisterDto.EMail,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            };
            _userService.Add(user);
            return new SuccessDataResult<User>(user);
            
            
        }


    }
}
