using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MapsterMapper;
using MediatR;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Domain.Manufacturing;
using MesEnterprise.Shared.Responses;

namespace MesEnterprise.Application.Features.WorkOrders;

public record CreateWorkOrderCommand(string WorkOrderNumber, Guid ModelId, int Quantity, DateTimeOffset PlannedStart, DateTimeOffset PlannedEnd) : IRequest<ApiResponse<Guid>>;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderNumber).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.PlannedStart).LessThan(x => x.PlannedEnd);
    }
}

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, ApiResponse<Guid>>
{
    private readonly IRepository<WorkOrder> _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreateWorkOrderCommandHandler(IRepository<WorkOrder> repository, IDateTimeProvider dateTimeProvider, IMapper mapper)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<WorkOrder>(request);
        entity.Status = WorkOrderStatus.Released;
        entity.CreatedAt = _dateTimeProvider.UtcNow;

        await _repository.AddAsync(entity, cancellationToken);

        return ApiResponse<Guid>.Ok(entity.Id);
    }
}
