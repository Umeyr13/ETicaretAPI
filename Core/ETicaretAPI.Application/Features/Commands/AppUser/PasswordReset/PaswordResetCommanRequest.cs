using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AppUser.PasswordReset
{
    public class PaswordResetCommanRequest:IRequest<PaswordResetCommanResponse>
    {
        public string Email { get; set; }
    }
}