using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapsterMapper;
using MediatR;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Domain.DataCenter;
using MesEnterprise.Shared.Responses;

namespace MesEnterprise.Application.Features.DataCenter.Processes;

public record GetProcessesQuery(int PageNumber = 1, int PageSize = 50) : IRequest<ApiResponse<PagedResponse<ProcessDto>>>;

public record ProcessDto(Guid Id, string ProcessCode, string ProcessName, string? Description, int Sequence);

public class GetProcessesQueryHandler : IRequestHandler<GetProcessesQuery, ApiResponse<PagedResponse<ProcessDto>>>
{
    private readonly IRepository<ProcessDefinition> _repository;
    private readonly IMapper _mapper;

    public GetProcessesQueryHandler(IRepository<ProcessDefinition> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<ProcessDto>>> Handle(GetProcessesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var dtos = items.Select(p => _mapper.Map<ProcessDto>(p)).ToList();
        return ApiResponse<PagedResponse<ProcessDto>>.Ok(new PagedResponse<ProcessDto>(dtos, totalCount, request.PageNumber, request.PageSize));
    }
}

