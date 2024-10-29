using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Locations.Commands;
using Domain.Locations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("locations")]
[ApiController]
public class LocationsController(ISender sender, ILocationQueries locationQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LocationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await locationQueries.GetAll(cancellationToken);
        return entities.Select(LocationDto.FromDomainModel).ToList();
    }
    
    [HttpGet("{locationsId:guid}")]
    public async Task<ActionResult<LocationDto>> GetById([FromRoute] Guid locationsId, CancellationToken cancellationToken)
    {
        var entity = await locationQueries.GetById(new LocationId(locationsId), cancellationToken);
        return entity.Match<ActionResult<LocationDto>>(
            l => LocationDto.FromDomainModel(l),
            () => NotFound());
    }
    
    [HttpPost]
    public async Task<ActionResult<LocationDto>> Create(
        [FromBody] LocationDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateLocationCommand
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            Capacity = request.Capacity
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<LocationDto>>(
            l => LocationDto.FromDomainModel(l),
            e => e.ToObjectResult());
    }
    [HttpPut]
    public async Task<ActionResult<LocationDto>> Update(
        [FromBody] LocationDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateLocationCommand
        {
            LocationId = request.Id!.Value,
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            Capacity = request.Capacity
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<LocationDto>>(
            l => LocationDto.FromDomainModel(l),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<LocationDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteLocationCommand
        {
            LocationId = id
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<LocationDto>>(
            l => LocationDto.FromDomainModel(l),
            e => e.ToObjectResult());
    }
}