using System;
using System.ComponentModel.DataAnnotations;

namespace Audit.Net.PoC
{
   public class MessageAudit
    {
        [Key] public Guid AuditLogId { get; set; }
        [Required] public string AuditData { get; set; }
        public DateTimeOffset AuditTimestamp { get; set; }
        public string AuditAction { get; set; }
        public Guid MessageId { get; set; }
    }
}
