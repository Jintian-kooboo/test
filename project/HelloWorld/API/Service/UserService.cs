using API.Interface;
using API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Service
{
    public class UserService : IUserService
    {
        public bool IsValid(LoginRequestDTO req)
        {
            return true;
        }
    }
}
