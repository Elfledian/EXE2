﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repo.Entities;

[Table("verifications")]
[Index("UserId", Name = "UQ__verifica__B9BE370EE289CFF9", IsUnique = true)]
public partial class Verification
{
    [Key]
    [Column("verification_id")]
    public Guid VerificationId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("blockchain_hash")]
    [StringLength(255)]
    public string BlockchainHash { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; }

    [Column("verified_at", TypeName = "datetime")]
    public DateTime? VerifiedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Verification")]
    public virtual User User { get; set; }
}