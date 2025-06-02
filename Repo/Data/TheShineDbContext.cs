using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repo.Entities;
using File = Repo.Entities.File;

namespace Repo.Data
{
    public class TheShineDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public TheShineDbContext()
        {
        }

        public TheShineDbContext(DbContextOptions<TheShineDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Application> Applications { get; set; }

        public virtual DbSet<Candidate> Candidates { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Company> Companies { get; set; }

        public virtual DbSet<CvDetail> CvDetails { get; set; }

        public virtual DbSet<Entities.File> Files { get; set; }

        public virtual DbSet<Invitation> Invitations { get; set; }

        public virtual DbSet<Job> Jobs { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<Rating> Ratings { get; set; }

        public virtual DbSet<Recruiter> Recruiters { get; set; }

        public virtual DbSet<Subscription> Subscriptions { get; set; }

        public virtual DbSet<Tag> Tags { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Verification> Verifications { get; set; }

        public virtual DbSet<WorkHistory> WorkHistories { get; set; }
        public static string GetConnectionString(string connectionStringName)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString(connectionStringName);
            return connectionString;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySQL(GetConnectionString("DefaultConnection"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.ApplicationId).HasName("PK__applicat__3BCBDCF27B586E0C");

                entity.Property(e => e.ApplicationId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.AppliedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValue("Applied");

                entity.HasOne(d => d.Candidate).WithMany(p => p.Applications).HasConstraintName("FK__applicati__candi__03F0984C");

                entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__applicati__job_i__02FC7413");
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasKey(e => e.CandidateId).HasName("PK__candidat__39BD4C18354A8933");

                entity.Property(e => e.CandidateId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Featured).HasDefaultValue(false);
                entity.Property(e => e.IncomeRange).HasDefaultValue("1-3M");
                entity.Property(e => e.Status).HasDefaultValue("Chua nh?n vi?c");
                entity.Property(e => e.Verified).HasDefaultValue(false);

                entity.HasOne(d => d.User).WithOne(p => p.Candidate)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__candidate__user___4CA06362");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PK__categori__D54EE9B4DDD25EB7");

                entity.Property(e => e.CategoryId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.CompanyId).HasName("PK__companie__3E267235299690E3");

                entity.Property(e => e.CompanyId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.LogoFile).WithMany(p => p.Companies)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_companies_files");

                entity.HasOne(d => d.Recruiter).WithOne(p => p.Company)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__companies__recru__52593CB8");
            });

            modelBuilder.Entity<CvDetail>(entity =>
            {
                entity.HasKey(e => e.CvId).HasName("PK__cv_detai__C36883E6681921B8");

                entity.Property(e => e.CvId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Candidate).WithMany(p => p.CvDetails)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__cv_detail__candi__31B762FC");
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasKey(e => e.FileId).HasName("PK__files__07D884C68CA1CF9F");

                entity.Property(e => e.FileId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.UploadDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Candidate).WithMany(p => p.Files)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__files__candidate__2BFE89A6");
            });

            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.HasKey(e => e.InvitationId).HasName("PK__invitati__94B74D7C19DB176C");

                entity.Property(e => e.InvitationId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.SentAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValue("Pending");

                entity.HasOne(d => d.Candidate).WithMany(p => p.Invitations).HasConstraintName("FK__invitatio__candi__0B91BA14");

                entity.HasOne(d => d.Job).WithMany(p => p.Invitations).HasConstraintName("FK__invitatio__job_i__0C85DE4D");

                entity.HasOne(d => d.Recruiter).WithMany(p => p.Invitations)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__invitatio__recru__0A9D95DB");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.JobId).HasName("PK__jobs__6E32B6A5C5F2B37E");

                entity.Property(e => e.JobId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.IsUrgent).HasDefaultValue(false);
                entity.Property(e => e.Location).HasDefaultValue("Ho Chi Minh City");
                entity.Property(e => e.PostedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValue("Open");

                entity.HasOne(d => d.Category).WithMany(p => p.Jobs)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__jobs__category_i__778AC167");

                entity.HasOne(d => d.Company).WithMany(p => p.Jobs).HasConstraintName("FK__jobs__company_id__75A278F5");

                entity.HasOne(d => d.Recruiter).WithMany(p => p.Jobs)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__jobs__recruiter___74AE54BC");

                entity.HasOne(d => d.Subscription).WithMany(p => p.Jobs).HasConstraintName("FK__jobs__subscripti__76969D2E");

                entity.HasMany(d => d.Tags).WithMany(p => p.Jobs)
                    .UsingEntity<Dictionary<string, object>>(
                        "JobTag",
                        r => r.HasOne<Tag>().WithMany()
                            .HasForeignKey("TagId")
                            .HasConstraintName("FK__job_tags__tag_id__7B5B524B"),
                        l => l.HasOne<Job>().WithMany()
                            .HasForeignKey("JobId")
                            .HasConstraintName("FK__job_tags__job_id__7A672E12"),
                        j =>
                        {
                            j.HasKey("JobId", "TagId").HasName("PK__job_tags__1A1BDC8E5B68F0DB");
                            j.ToTable("job_tags");
                            j.HasIndex(new[] { "JobId" }, "idx_job_tags_job_id");
                            j.IndexerProperty<Guid>("JobId").HasColumnName("job_id");
                            j.IndexerProperty<Guid>("TagId").HasColumnName("tag_id");
                        });
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F6357338B");

                entity.Property(e => e.NotificationId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsRead).HasDefaultValue(false);

                entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__notificat__user___2645B050");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId).HasName("PK__payments__ED1FC9EAC80CA886");

                entity.Property(e => e.PaymentId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Status).HasDefaultValue("Pending");

                entity.HasOne(d => d.Application).WithOne(p => p.Payment)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__payments__applic__1332DBDC");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => e.RatingId).HasName("PK__ratings__D35B278B9864B1A5");

                entity.Property(e => e.RatingId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Reviewer).WithMany(p => p.Ratings).HasConstraintName("FK__ratings__reviewe__18EBB532");
            });

            modelBuilder.Entity<Recruiter>(entity =>
            {
                entity.HasKey(e => e.RecruiterId).HasName("PK__recruite__42ABA257E9F06E1E");

                entity.Property(e => e.RecruiterId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Verified).HasDefaultValue(false);

                entity.HasOne(d => d.User).WithOne(p => p.Recruiter)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__recruiter__user___59FA5E80");
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => e.SubscriptionId).HasName("PK__subscrip__863A7EC161176C70");

                entity.Property(e => e.SubscriptionId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CandidatesManagement).HasDefaultValue(false);
                entity.Property(e => e.StartDate).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValue("Active");
                entity.Property(e => e.UrgentHiring).HasDefaultValue(false);

                entity.HasOne(d => d.Recruiter).WithMany(p => p.Subscriptions)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__subscript__recru__6A30C649");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.TagId).HasName("PK__tags__4296A2B61A399BE7");

                entity.Property(e => e.TagId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__users__B9BE370FA57EB208");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Location).HasDefaultValue("Ho Chi Minh City");
                entity.Property(e => e.RememberMe).HasDefaultValue(false);
                entity.Property(e => e.TermsAgreed).HasDefaultValue(false);
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Verification>(entity =>
            {
                entity.HasKey(e => e.VerificationId).HasName("PK__verifica__24F179694FEA4951");

                entity.Property(e => e.VerificationId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Status).HasDefaultValue("Pending");

                entity.HasOne(d => d.User).WithOne(p => p.Verification)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__verificat__user___1F98B2C1");
            });

            modelBuilder.Entity<WorkHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId).HasName("PK__work_his__096AA2E992B2BFDA");

                entity.Property(e => e.HistoryId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Candidate).WithMany(p => p.WorkHistories)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__work_hist__candi__367C1819");
            });
        }
    }
}
