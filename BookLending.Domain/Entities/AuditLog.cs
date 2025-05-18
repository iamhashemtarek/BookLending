using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; } 
        public string TableName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }
}
