// <copyright file="IExpenseRepository.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Application;

using ExpenseApproval.Domain;

/// <summary>
/// Defines a contract for CRUD operations relating to <see cref="Expense"/> entities against a data persistence mechanism.
/// </summary>
/// <remarks>Implementations of this interface should ensure thread safety and handle persistence concerns.</remarks>
public interface IExpenseRepository
{
    /// <summary>
    /// Asynchronously adds the specified <see cref="Expense"/> to the data store.
    /// </summary>
    /// <param name="expense">The expense to add. Cannot be null.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the add operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous add operation.</returns>
    Task AddAsync(Expense expense, CancellationToken ct);

    /// <summary>
    /// Asynchronously retrieves the <see cref="Expense"/> with the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The unique identifier of the <see cref="Expense"/> to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the relevant <see cref="Expense"/> if found; otherwise, <see langword="null"/>.</returns>
    Task<Expense?> GetAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Asynchronously determines whether a receipt with the specified hash exists.
    /// </summary>
    /// <param name="receiptHash">The hash value of the receipt to check for existence.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if a receipt with
    /// the specified hash exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> ReceiptHashExistsAsync(string receiptHash, CancellationToken ct);

    /// <summary>
    /// Asynchronously updates the specified expense record.
    /// </summary>
    /// <param name="expense">The <see cref="Expense"/> entity to update.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the update operation.</param>
    /// <returns>A task <see cref="Task"/> represents the asynchronous update operation.</returns>
    Task UpdateAsync(Expense expense, CancellationToken ct);
}