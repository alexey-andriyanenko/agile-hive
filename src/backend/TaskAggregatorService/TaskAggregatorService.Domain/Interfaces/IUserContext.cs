﻿namespace TaskAggregatorService.Domain.Interfaces;

public interface IUserContext
{
    Guid UserId { get; }
}