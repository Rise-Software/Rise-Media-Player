﻿using System;

namespace Rise.Models
{
    /// <summary>
    /// Base class for database entities.
    /// </summary>
    public class DbObject
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
