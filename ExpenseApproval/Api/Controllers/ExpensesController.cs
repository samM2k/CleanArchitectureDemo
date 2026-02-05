namespace ExpenseApproval.Api.Controllers;

using ExpenseApproval.Api.DTOs;
using ExpenseApproval.Api.Mapping;
using ExpenseApproval.Application;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/expenses")]
public sealed class ExpensesController : ControllerBase
{
    private readonly ExpenseService _service;

    public ExpensesController(ExpenseService service)
    {
        this._service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDraft([FromBody] CreateExpenseRequest request, CancellationToken ct)
    {
        var command = ExpenseApiMapper.ToCommand(request);
        var id = await this._service.CreateDraftAsync(command, ct);

        return this.CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
    {
        await this._service.SubmitAsync(id, ct);
        return this.Ok();
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveExpenseRequest request, CancellationToken ct)
    {
        var command = ExpenseApiMapper.ToCommand(id, request);
        await this._service.ApproveAsync(command, ct);
        return this.Ok();
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectExpenseRequest request, CancellationToken ct)
    {
        var command = ExpenseApiMapper.ToCommand(id, request);
        await this._service.RejectAsync(command, ct);
        return this.Ok();
    }

    [HttpPost("{id:guid}/paid")]
    public async Task<IActionResult> MarkPaid(Guid id, CancellationToken ct)
    {
        await this._service.MarkPaidAsync(id, ct);
        return this.Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await this._service.RetrieveExpenseAsync(id, ct);
        return dto is null ? this.NotFound() : this.Ok(dto);
    }
}
