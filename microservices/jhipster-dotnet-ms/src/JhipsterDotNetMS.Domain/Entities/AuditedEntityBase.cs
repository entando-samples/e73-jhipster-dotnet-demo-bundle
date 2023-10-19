using System;
using JhipsterDotNetMS.Domain.Entities.Interfaces;

namespace JhipsterDotNetMS.Domain.Entities;

public abstract class AuditedEntityBase<TKey> : BaseEntity<TKey>, IAuditedEntityBase
{
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime LastModifiedDate { get; set; }
}
