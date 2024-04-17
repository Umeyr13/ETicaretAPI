using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.Authentications
{
    public interface IInternalAuthentication
    {
        Task<DTOs.Token> LoginAsync(string usernameOrEmail, string passwordi, int accessTokeLifeTime);
        Task<DTOs.Token> RefreshTokenLoginAsync(string refreshToken);
    }
}
