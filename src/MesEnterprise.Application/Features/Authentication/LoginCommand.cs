using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Shared.Responses;

namespace MesEnterprise.Application.Features.Authentication;

public record LoginCommand(string UserName, string Password) : IRequest<ApiResponse<LoginResult>>;

public record LoginResult(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, Guid UserId, Guid TenantId, string DisplayName, IEnumerable<string> Roles, IEnumerable<string> Permissions);

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResult>>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ITenantContext _tenantContext;

    public LoginCommandHandler(IIdentityService identityService, ITokenService tokenService, ITenantContext tenantContext)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _tenantContext = tenantContext;
    }

    public async Task<ApiResponse<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserAsync(request.UserName, cancellationToken);
        if (user is null)
        {
            return ApiResponse<LoginResult>.Fail("Invalid credentials");
        }

        if (!await _identityService.ValidateCredentialsAsync(request.UserName, request.Password, cancellationToken))
        {
            return ApiResponse<LoginResult>.Fail("Invalid credentials");
        }

        var roles = await _identityService.GetRolesAsync(user.Id, cancellationToken);
        var permissions = await _identityService.GetPermissionsAsync(user.Id, cancellationToken);

        var accessToken = await _tokenService.GenerateTokenAsync(user, roles, permissions, cancellationToken);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user, cancellationToken);

        var tenant = _tenantContext.CurrentTenant;
        var expiresAt = DateTimeOffset.UtcNow.AddHours(2);

        return ApiResponse<LoginResult>.Ok(new LoginResult(accessToken, refreshToken, expiresAt, user.Id, tenant?.Id ?? Guid.Empty, user.DisplayName, roles, permissions));
    }
}
