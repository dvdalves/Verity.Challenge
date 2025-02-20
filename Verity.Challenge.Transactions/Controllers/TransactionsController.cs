using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Verity.Challenge.Transactions.Application.Commands.CreateTransactionCommandHandler;
using static Verity.Challenge.Transactions.Application.Commands.DeleteTransactionCommandHandler;
using static Verity.Challenge.Transactions.Application.Commands.UpdateTransactionCommandHandler;
using static Verity.Challenge.Transactions.Application.Queries.GetTransactionByIdQueryHandler;
using static Verity.Challenge.Transactions.Application.Queries.GetTransactionsQueryHandler;

namespace Verity.Challenge.Transactions.Controllers;

[Route("api/transactions")]
[ApiController]
public class TransactionsController(IMediator _mediatr) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
    {
        var transactionId = await _mediatr.Send(command);
        return CreatedAtAction(nameof(CreateTransaction), new { id = transactionId }, null);
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] GetTransactionsQuery query)
    {
        var transactions = await _mediatr.Send(query);
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        var transaction = await _mediatr.Send(new GetTransactionByIdQuery(id));

        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        var success = await _mediatr.Send(new DeleteTransactionCommand(id));

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateTransactionCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID in URL and body must match.");

        var success = await _mediatr.Send(command);

        if (!success)
            return NotFound();

        return NoContent();
    }
}
