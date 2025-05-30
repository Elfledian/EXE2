﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Repo.Entities;

[Table("invitations")]
public partial class Invitation
{
    [Key]
    [Column("invitation_id")]
    public Guid InvitationId { get; set; }

    [Column("recruiter_id")]
    public Guid? RecruiterId { get; set; }

    [Column("candidate_id")]
    public Guid? CandidateId { get; set; }

    [Column("job_id")]
    public Guid? JobId { get; set; }

    [Column("message")]
    public string Message { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; }

    [Column("sent_at", TypeName = "datetime")]
    public DateTime? SentAt { get; set; }

    [ForeignKey("CandidateId")]
    [InverseProperty("Invitations")]
    public virtual Candidate Candidate { get; set; }

    [ForeignKey("JobId")]
    [InverseProperty("Invitations")]
    public virtual Job Job { get; set; }

    [ForeignKey("RecruiterId")]
    [InverseProperty("Invitations")]
    public virtual Recruiter Recruiter { get; set; }
}