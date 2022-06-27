using System;

namespace appNoSql.Domain.Core.Models
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
    }
}
