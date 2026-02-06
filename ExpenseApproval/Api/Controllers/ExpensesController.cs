namespace ExpenseApproval.Api.Controllers;

using ExpenseApproval.Api.DTOs;
using ExpenseApproval.Api.Mapping;
using ExpenseApproval.Application;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides API endpoints for expense approval use cases.
/// </summary>
[ApiController]
[Route("api/expenses")]
public sealed class ExpensesController : ControllerBase
{
    private readonly ExpenseService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpensesController"/> class with the specified expense service.
    /// </summary>
    /// <param name="service">The expense service to be used for handling expense-related operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="service"/> is null.</exception>"
    public ExpensesController(ExpenseService service)
    {
        ArgumentNullException.ThrowIfNull(service, nameof(service));

        this._service = service;
    }

    /// <summary>
    /// Creates a new expense draft using the provided <see cref="CreateExpenseRequest"/> data.
    /// </summary>
    /// <remarks>The response includes a location header pointing to the resource representing the created
    /// draft. This method does not persist the expense as a finalized record; it creates a draft that can be updated or
    /// submitted later.</remarks>
    /// <param name="request">The expense details to use when creating the draft.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the create draft operation.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateDraft([FromBody] CreateExpenseRequest request, CancellationToken ct)
    {
        var command = ExpenseApiMapper.ToCommand(request);
        var id = await this._service.CreateDraftAsync(command, ct);

        return this.CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Submits the entity identified by the specified ID for processing.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to submit.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the submit operation.</returns>
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
    {
        await this._service.SubmitAsync(id, ct);
        return this.Ok();
    }

    /// <summary>
    /// Approves the specified <see cref="ApproveExpenseRequest"/> identified by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the expense to approve.</param>
    /// <param name="request">The details of the expense approval request..</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the approval operation.</returns>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveExpenseRequest request, CancellationToken ct)
    {
        var command = ExpenseApiMapper.ToCommand(id, request);
        await this._service.ApproveAsync(command, ct);
        return this.Ok();
    }

    /// <summary>
    /// Rejects the specified <see cref="RejectExpenseRequest"/> by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the expense to reject.</param>
    /// <param name="request">The details of the rejection, including reason and any additional information..</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the rejection operation.</returns>
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectExpenseRequest request, CancellationToken ct)
    {
        var command = ExpenseApiMapper.ToCommand(id, request);
        await this._service.RejectAsync(command, ct);
        return this.Ok();
    }

    /// <summary>
    /// Marks the specified invoice as paid.
    /// </summary>
    /// <remarks>This action is typically used to update the payment status of an invoice. If the invoice does
    /// not exist or cannot be marked as paid, an appropriate error response may be returned.</remarks>
    /// <param name="id">The unique identifier of the invoice to mark as paid.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [HttpPost("{id:guid}/paid")]
    public async Task<IActionResult> MarkPaid(Guid id, CancellationToken ct)
    {
        await this._service.MarkPaidAsync(id, ct);
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the expense record with the specified unique identifier.
    /// </summary>
    /// <remarks>Returns a 404 Not Found response if no expense exists with the specified identifier. This
    /// method is intended for use in HTTP GET requests to retrieve a single expense resource.</remarks>
    /// <param name="id">The unique identifier of the expense to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the expense data if found; otherwise, a NotFound result.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var dto = await this._service.RetrieveExpenseAsync(id, ct);
        return dto is null ? this.NotFound() : this.Ok(dto);
    }
}
