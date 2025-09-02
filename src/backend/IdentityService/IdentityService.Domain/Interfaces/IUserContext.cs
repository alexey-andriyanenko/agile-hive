﻿namespace IdentityService.Domain.Interfaces;

public interface IUserContext
{
    Guid UserId { get; }
    Guid OrganizationId { get; }
}