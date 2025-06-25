using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FYP1System.Models;

namespace FYP1System.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Models.Program> Programs { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<CommitteeMember> CommitteeMembers { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<EvaluatorAssignment> EvaluatorAssignments { get; set; }
    public DbSet<ProposalEvaluation> ProposalEvaluations { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ApplicationUser relationships
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne(e => e.Program)
                  .WithMany(e => e.Users)
                  .HasForeignKey(e => e.ProgramId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Student)
                  .WithOne(e => e.User)
                  .HasForeignKey<Student>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Lecturer)
                  .WithOne(e => e.User)
                  .HasForeignKey<Lecturer>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Lecturer relationships
        builder.Entity<Lecturer>(entity =>
        {
            entity.HasOne(e => e.Program)
                  .WithMany(e => e.Lecturers)
                  .HasForeignKey(e => e.ProgramId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Student relationships
        builder.Entity<Student>(entity =>
        {
            entity.HasOne(e => e.Program)
                  .WithMany(e => e.Students)
                  .HasForeignKey(e => e.ProgramId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Supervisor)
                  .WithMany(e => e.SupervisedStudents)
                  .HasForeignKey(e => e.SupervisorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure CommitteeMember relationships
        builder.Entity<CommitteeMember>(entity =>
        {
            entity.HasOne(e => e.Lecturer)
                  .WithMany(e => e.CommitteeRoles)
                  .HasForeignKey(e => e.LecturerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Program)
                  .WithMany(e => e.CommitteeMembers)
                  .HasForeignKey(e => e.ProgramId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Proposal relationships
        builder.Entity<Proposal>(entity =>
        {
            entity.HasOne(e => e.Student)
                  .WithMany(e => e.Proposals)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure EvaluatorAssignment relationships
        builder.Entity<EvaluatorAssignment>(entity =>
        {
            entity.HasOne(e => e.Proposal)
                  .WithMany(e => e.EvaluatorAssignments)
                  .HasForeignKey(e => e.ProposalId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Evaluator)
                  .WithMany(e => e.EvaluatorAssignments)
                  .HasForeignKey(e => e.EvaluatorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure ProposalEvaluation relationships
        builder.Entity<ProposalEvaluation>(entity =>
        {
            entity.HasOne(e => e.Proposal)
                  .WithMany(e => e.ProposalEvaluations)
                  .HasForeignKey(e => e.ProposalId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Evaluator)
                  .WithMany()
                  .HasForeignKey(e => e.EvaluatorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Comment relationships
        builder.Entity<Comment>(entity =>
        {
            entity.HasOne(e => e.Proposal)
                  .WithMany(e => e.Comments)
                  .HasForeignKey(e => e.ProposalId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ParentComment)
                  .WithMany(e => e.Replies)
                  .HasForeignKey(e => e.ParentCommentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure decimal precision for scores
        builder.Entity<ProposalEvaluation>(entity =>
        {
            entity.Property(e => e.OverallScore).HasPrecision(5, 2);
            entity.Property(e => e.TechnicalMeritScore).HasPrecision(5, 2);
            entity.Property(e => e.InnovationScore).HasPrecision(5, 2);
            entity.Property(e => e.FeasibilityScore).HasPrecision(5, 2);
            entity.Property(e => e.LiteratureReviewScore).HasPrecision(5, 2);
        });

        builder.Entity<Student>(entity =>
        {
            entity.Property(e => e.GPA).HasPrecision(4, 2);
        });
    }
}
