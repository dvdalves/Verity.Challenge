﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.CreateTransaction;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.DeleteTransaction;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.GetTransactionById;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.GetTransactions;
using static Verity.Challenge.Transactions.Application.Transaction.Handlers.UpdateTransaction;

namespace Verity.Challenge.Transactions.Controllers;

[Route("api/transactions")]
[ApiController]
public class TransactionsController(IMediator _mediatr) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediatr.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetTransactionsQuery query, CancellationToken cancellationToken)
    {
        var transactions = await _mediatr.Send(query, cancellationToken);
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await _mediatr.Send(new GetTransactionByIdQuery(id), cancellationToken);

        return transaction == null ? NotFound() : Ok(transaction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _mediatr.Send(new DeleteTransactionCommand(id), cancellationToken);

        return !success ? NotFound() : NoContent();
    }

    [HttpPut()]
    public async Task<IActionResult> Update([FromBody] UpdateTransactionCommand command, CancellationToken cancellationToken)
    {
        var success = await _mediatr.Send(command, cancellationToken);

        return !success ? NotFound() : NoContent();
    }
}