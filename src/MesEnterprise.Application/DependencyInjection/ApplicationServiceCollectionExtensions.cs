using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;         // 若需要 AutoValidation
using Mapster;
using MapsterMapper;
using MediatR;
using MesEnterprise.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace MesEnterprise.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // MediatR v12 的注册方式
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation：扫描并注册当前程序集内的验证器
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // 如需自动模型验证（结合 ASP.NET Core）
        // services.AddFluentValidationAutoValidation();

        // Mapster：注册 IMapper
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // MediatR 验证管道（注意泛型约束要写在行为类里）
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
