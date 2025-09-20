using Mapster;
using MesEnterprise.Application.Features.DataCenter.Processes;
using MesEnterprise.Application.Features.WorkOrders;
using MesEnterprise.Domain.DataCenter;
using MesEnterprise.Domain.Manufacturing;

namespace MesEnterprise.Application.DependencyInjection;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProcessDefinition, ProcessDto>();
        config.NewConfig<CreateWorkOrderCommand, WorkOrder>();
    }
}
