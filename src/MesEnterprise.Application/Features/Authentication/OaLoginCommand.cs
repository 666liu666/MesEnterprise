using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Shared.Responses;

namespace MesEnterprise.Application.Features.Authentication;

public record OaLoginCommand(string UserName, string Password) : IRequest<ApiResponse<bool>>;

public class OaLoginCommandValidator : AbstractValidator<OaLoginCommand>
{
    public OaLoginCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class OaLoginCommandHandler : IRequestHandler<OaLoginCommand, ApiResponse<bool>>
{
    private readonly IIdentityService _identityService;

    public OaLoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ApiResponse<bool>> Handle(OaLoginCommand request, CancellationToken cancellationToken)
    {
        var isValid = await _identityService.ValidateOaCredentialsAsync(request.UserName, request.Password, cancellationToken);
        if (!isValid)
        {
            return ApiResponse<bool>.Fail("Invalid credentials");
        }

        return ApiResponse<bool>.Ok(true, "OA login successful");
    }
}
