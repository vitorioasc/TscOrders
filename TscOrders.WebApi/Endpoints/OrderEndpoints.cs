using MediatR;
using TscOrders.Core.Abstraction.Common;
using TscOrders.Core.Orders.Commands.CancelOrder;
using TscOrders.Core.Orders.Commands.CreateOrder;
using TscOrders.Core.Orders.Dtos;
using TscOrders.Core.Orders.Queries.GetOrderById;
using TscOrders.Core.Orders.Queries.GetOrders;

namespace TscOrders.WebApi.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders").RequireAuthorization();

        group.MapPost("/", async (CreateOrderCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var order = await sender.Send(command, cancellationToken);
            return Results.Created($"/api/orders/{order.Id}", order);
        })
        .WithName("CreateOrder")
        .Produces<OrderDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        group.MapGet("/", async (int page, int pageSize, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetOrdersQuery(page == 0 ? 1 : page, pageSize == 0 ? 10 : pageSize);
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetOrders")
        .Produces<PagedResult<OrderDto>>(StatusCodes.Status200OK)
        .ProducesValidationProblem();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var order = await sender.Send(new GetOrderByIdQuery(id), cancellationToken);
            return Results.Ok(order);
        })
        .WithName("GetOrderById")
        .Produces<OrderDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{id:guid}/cancel", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            await sender.Send(new CancelOrderCommand(id), cancellationToken);
            return Results.NoContent();
        })
        .WithName("CancelOrder")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);

        return app;
    }
}
