﻿using System;
namespace FreelanceManager
{
    public interface IEvent
    {
        Guid Id { get; }
        int Version { get; }
    }
}
