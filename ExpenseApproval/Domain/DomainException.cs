// <copyright file="DomainException.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Domain;

/// <summary>
/// Represents errors that occur when a business rule or domain constraint is violated within the application domain.
/// </summary>
/// <remarks>Use this exception to indicate that an operation has failed due to a violation of domain-specific
/// rules or invariants. This exception is intended for scenarios where the error is related to business logic rather
/// than system or infrastructure failures.</remarks>
public sealed class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DomainException(string message)
        : base(message)
    {
    }
}
