﻿// <auto-generated />
using System;
using FYP1System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FYP1System.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250628074132_UpdatedModelsForSupervisorEvaluator")]
    partial class UpdatedModelsForSupervisorEvaluator
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.6");

            modelBuilder.Entity("FYP1System.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProgramId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("ProgramId");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("FYP1System.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("LecturerId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ParentCommentId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProposalId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LecturerId");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("ProposalId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("FYP1System.Models.CommitteeMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AssignedDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LecturerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProgramId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Role")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LecturerId");

                    b.HasIndex("ProgramId");

                    b.ToTable("CommitteeMembers");
                });

            modelBuilder.Entity("FYP1System.Models.EvaluatorAssignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AssignedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("AssignedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("AssignedDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("EvaluationId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EvaluatorId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProposalId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EvaluationId");

                    b.HasIndex("EvaluatorId");

                    b.HasIndex("ProposalId");

                    b.ToTable("EvaluatorAssignments");
                });

            modelBuilder.Entity("FYP1System.Models.Lecturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanSupervise")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Domain")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCommittee")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OfficeLocation")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("ProgramId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Specialization")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("StaffId")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProgramId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Lecturers");
                });

            modelBuilder.Entity("FYP1System.Models.Program", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("FYP1System.Models.Proposal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AgreementFilePath")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<string>("ExpectedOutcomes")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Methodology")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Objectives")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<int?>("Semester")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Session")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StudentId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("SubmittedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("Proposals");
                });

            modelBuilder.Entity("FYP1System.Models.ProposalEvaluation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Comments")
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("EvaluatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("EvaluatorId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal?>("FeasibilityScore")
                        .HasPrecision(5, 2)
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("InnovationScore")
                        .HasPrecision(5, 2)
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("LiteratureReviewScore")
                        .HasPrecision(5, 2)
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("OverallScore")
                        .HasPrecision(5, 2)
                        .HasColumnType("TEXT");

                    b.Property<int>("ProposalId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Recommendations")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Strengths")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("TechnicalMeritScore")
                        .HasPrecision(5, 2)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Weaknesses")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EvaluatorId");

                    b.HasIndex("ProposalId");

                    b.ToTable("ProposalEvaluations");
                });

            modelBuilder.Entity("FYP1System.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("GPA")
                        .HasPrecision(4, 2)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("LecturerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProgramId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Semester")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Session")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("StudentId")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<int?>("SupervisorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LecturerId");

                    b.HasIndex("ProgramId");

                    b.HasIndex("SupervisorId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Students");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("FYP1System.Models.ApplicationUser", b =>
                {
                    b.HasOne("FYP1System.Models.Program", "Program")
                        .WithMany("Users")
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Program");
                });

            modelBuilder.Entity("FYP1System.Models.Comment", b =>
                {
                    b.HasOne("FYP1System.Models.Lecturer", null)
                        .WithMany("Comments")
                        .HasForeignKey("LecturerId");

                    b.HasOne("FYP1System.Models.Comment", "ParentComment")
                        .WithMany("Replies")
                        .HasForeignKey("ParentCommentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("FYP1System.Models.Proposal", "Proposal")
                        .WithMany("Comments")
                        .HasForeignKey("ProposalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FYP1System.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ParentComment");

                    b.Navigation("Proposal");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FYP1System.Models.CommitteeMember", b =>
                {
                    b.HasOne("FYP1System.Models.Lecturer", "Lecturer")
                        .WithMany("CommitteeRoles")
                        .HasForeignKey("LecturerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FYP1System.Models.Program", "Program")
                        .WithMany("CommitteeMembers")
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lecturer");

                    b.Navigation("Program");
                });

            modelBuilder.Entity("FYP1System.Models.EvaluatorAssignment", b =>
                {
                    b.HasOne("FYP1System.Models.ProposalEvaluation", "Evaluation")
                        .WithMany()
                        .HasForeignKey("EvaluationId");

                    b.HasOne("FYP1System.Models.Lecturer", "Evaluator")
                        .WithMany("EvaluatorAssignments")
                        .HasForeignKey("EvaluatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FYP1System.Models.Proposal", "Proposal")
                        .WithMany("EvaluatorAssignments")
                        .HasForeignKey("ProposalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Evaluation");

                    b.Navigation("Evaluator");

                    b.Navigation("Proposal");
                });

            modelBuilder.Entity("FYP1System.Models.Lecturer", b =>
                {
                    b.HasOne("FYP1System.Models.Program", "Program")
                        .WithMany("Lecturers")
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FYP1System.Models.ApplicationUser", "User")
                        .WithOne("Lecturer")
                        .HasForeignKey("FYP1System.Models.Lecturer", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Program");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FYP1System.Models.Proposal", b =>
                {
                    b.HasOne("FYP1System.Models.Student", "Student")
                        .WithMany("Proposals")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("FYP1System.Models.ProposalEvaluation", b =>
                {
                    b.HasOne("FYP1System.Models.Lecturer", "Evaluator")
                        .WithMany()
                        .HasForeignKey("EvaluatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FYP1System.Models.Proposal", "Proposal")
                        .WithMany("ProposalEvaluations")
                        .HasForeignKey("ProposalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Evaluator");

                    b.Navigation("Proposal");
                });

            modelBuilder.Entity("FYP1System.Models.Student", b =>
                {
                    b.HasOne("FYP1System.Models.Lecturer", null)
                        .WithMany("Students")
                        .HasForeignKey("LecturerId");

                    b.HasOne("FYP1System.Models.Program", "Program")
                        .WithMany("Students")
                        .HasForeignKey("ProgramId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FYP1System.Models.Lecturer", "Supervisor")
                        .WithMany("SupervisedStudents")
                        .HasForeignKey("SupervisorId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("FYP1System.Models.ApplicationUser", "User")
                        .WithOne("Student")
                        .HasForeignKey("FYP1System.Models.Student", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Program");

                    b.Navigation("Supervisor");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("FYP1System.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("FYP1System.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FYP1System.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("FYP1System.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FYP1System.Models.ApplicationUser", b =>
                {
                    b.Navigation("Lecturer");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("FYP1System.Models.Comment", b =>
                {
                    b.Navigation("Replies");
                });

            modelBuilder.Entity("FYP1System.Models.Lecturer", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("CommitteeRoles");

                    b.Navigation("EvaluatorAssignments");

                    b.Navigation("Students");

                    b.Navigation("SupervisedStudents");
                });

            modelBuilder.Entity("FYP1System.Models.Program", b =>
                {
                    b.Navigation("CommitteeMembers");

                    b.Navigation("Lecturers");

                    b.Navigation("Students");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("FYP1System.Models.Proposal", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("EvaluatorAssignments");

                    b.Navigation("ProposalEvaluations");
                });

            modelBuilder.Entity("FYP1System.Models.Student", b =>
                {
                    b.Navigation("Proposals");
                });
#pragma warning restore 612, 618
        }
    }
}
