﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repo.Entities;

[Table("payments")]
[Index("ApplicationId", Name = "UQ__payments__3BCBDCF3629EA847", IsUnique = true)]
public partial class Payment
{
    [Key]
    [Column("payment_id")]
    public Guid PaymentId { get; set; }

    [Column("application_id")]
    public Guid? ApplicationId { get; set; }

    [Column("amount", TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; }

    [Column("transaction_id")]
    [StringLength(100)]
    public string TransactionId { get; set; }

    [Column("paid_at", TypeName = "datetime")]
    public DateTime? PaidAt { get; set; }

    [ForeignKey("ApplicationId")]
    [InverseProperty("Payment")]
    public virtual Application Application { get; set; }
}