using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.PasswordReset
{
    public class PaswordResetCommanHandler : IRequestHandler<PaswordResetCommanRequest, PaswordResetCommanResponse>
    {
        readonly IAuthService _authService;

        public PaswordResetCommanHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<PaswordResetCommanResponse> Handle(PaswordResetCommanRequest request, CancellationToken cancellationToken)
        {
             await _authService.PasswordResetAsnyc(request.Email);
            return new();
        }
    }
}
