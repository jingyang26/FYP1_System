using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FYP1System.Models;
using FYP1System.Data;

namespace FYP1System.Data
{
    /// <summary>
    /// Comprehensive database seeding class for the FYP1 Management System.
    /// This class creates a focused set of test accounts for system demonstration and testing.
    /// 
    /// Features:
    /// - Creates 1 Admin account
    /// - Creates 3 Committee members (SE, DE, CS)
    /// - Creates 4 Supervisor accounts (2 SE, 2 DE)
    /// - Creates 4 Evaluator accounts (2 SE, 2 DE)
    /// - Creates 4 Student accounts (2 SE, 2 DE) with specific supervisor assignments
    /// - Establishes evaluator assignments as per project requirements
    /// - Creates sample proposals and comments for testing
    /// - Supports all 4 programs but focuses on SE and DE for user accounts
    /// 
    /// User Relationships:
    /// - Ahmad Ali (SE12001) → Supervisor: Dr. Sarah Johnson → Evaluators: Prof. Kevin Martinez, Dr. Nancy Rodriguez
    /// - Siti Nurhaliza (SE12002) → Supervisor: Dr. Michael Brown → Evaluators: Prof. Kevin Martinez, Dr. Steven Lewis
    /// - Chen Wei Jie (DE12001) → Supervisor: Prof. Robert Wilson → Evaluators: Dr. Steven Lewis, Dr. Dorothy Clark
    /// - Nurul Ain (DE12002) → Supervisor: Dr. Linda Taylor → Evaluators: Dr. Steven Lewis, Dr. Dorothy Clark
    /// </summary>
    public static class SeedDataNew
    {
        public static async Task Initialize(IServiceProvider serviceProvider, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Create roles
            await CreateRoles(roleManager);

            // Create default programs
            await CreatePrograms(context);

            // Create all users
            await CreateUsers(userManager, context);
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Committee", "Student", "Supervisor", "Evaluator" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CreatePrograms(ApplicationDbContext context)
        {
            if (!context.Programs.Any())
            {
                var programs = new List<Models.Program>
                {
                    new Models.Program
                    {
                        Name = "Software Engineering",
                        Description = "Bachelor of Software Engineering program",
                        IsActive = true
                    },
                    new Models.Program
                    {
                        Name = "Data Engineering",
                        Description = "Bachelor of Data Engineering program",
                        IsActive = true
                    },
                    new Models.Program
                    {
                        Name = "Computer Science",
                        Description = "Bachelor of Computer Science program",
                        IsActive = true
                    },
                    new Models.Program
                    {
                        Name = "Information Technology",
                        Description = "Bachelor of Information Technology program",
                        IsActive = true
                    }
                };

                await context.Programs.AddRangeAsync(programs);
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateUsers(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            // Get programs - check if they exist first
            var seProgram = await context.Programs.FirstOrDefaultAsync(p => p.Name == "Software Engineering");
            var deProgram = await context.Programs.FirstOrDefaultAsync(p => p.Name == "Data Engineering");
            var csProgram = await context.Programs.FirstOrDefaultAsync(p => p.Name == "Computer Science");
            var itProgram = await context.Programs.FirstOrDefaultAsync(p => p.Name == "Information Technology");

            // If any programs don't exist, skip their related user creation
            var availablePrograms = new List<Models.Program>();
            if (seProgram != null) availablePrograms.Add(seProgram);
            if (deProgram != null) availablePrograms.Add(deProgram);
            if (csProgram != null) availablePrograms.Add(csProgram);
            if (itProgram != null) availablePrograms.Add(itProgram);

            if (availablePrograms.Count == 0)
            {
                // If no programs exist, just create admin accounts
                await CreateAdminAccounts(userManager, context);
                return;
            }

            // Create Admin accounts
            await CreateAdminAccounts(userManager, context);

            // Create other accounts only for available programs
            await CreateCommitteeAccounts(userManager, context, availablePrograms.ToArray());
            await CreateSupervisorAccounts(userManager, context, availablePrograms.ToArray());
            await CreateEvaluatorAccounts(userManager, context, availablePrograms.ToArray());
            await CreateStudentAccounts(userManager, context, availablePrograms.ToArray());
            
            // Create additional sample data for testing
            await CreateAdditionalSampleData(context, availablePrograms.ToArray());
        }

        private static async Task CreateAdminAccounts(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            var adminUsers = new[]
            {
                new { Email = "admin@fyp1.edu.my", FullName = "System Administrator", Password = "Admin@123" }
            };

            foreach (var adminData in adminUsers)
            {
                var adminUser = await userManager.FindByEmailAsync(adminData.Email);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminData.Email,
                        Email = adminData.Email,
                        FullName = adminData.FullName,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(adminUser, adminData.Password);
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task CreateCommitteeAccounts(UserManager<ApplicationUser> userManager, ApplicationDbContext context, params Models.Program[] programs)
        {
            // Only create accounts for available programs
            var programLookup = programs.ToDictionary(p => p.Name, p => p);

            var committeeUsers = new[]
            {
                new { Email = "committee.se@fyp1.edu.my", FullName = "Dr. John Smith", Domain = "Software Engineering", Office = "Block A, Level 2", ProgramName = "Software Engineering", Role = "Chair" },
                new { Email = "committee.de@fyp1.edu.my", FullName = "Dr. Lisa Wong", Domain = "Data Engineering", Office = "Block C, Level 2", ProgramName = "Data Engineering", Role = "Chair" },
                new { Email = "committee.cs@fyp1.edu.my", FullName = "Prof. David Lee", Domain = "Computer Science", Office = "Block B, Level 3", ProgramName = "Computer Science", Role = "Chair" }
            };

            foreach (var committeeData in committeeUsers)
            {
                // Skip if program doesn't exist
                if (!programLookup.ContainsKey(committeeData.ProgramName))
                    continue;

                var committeeUser = await userManager.FindByEmailAsync(committeeData.Email);
                if (committeeUser == null)
                {
                    var program = programLookup[committeeData.ProgramName];
                    
                    committeeUser = new ApplicationUser
                    {
                        UserName = committeeData.Email,
                        Email = committeeData.Email,
                        FullName = committeeData.FullName,
                        EmailConfirmed = true,
                        ProgramId = program.Id
                    };

                    await userManager.CreateAsync(committeeUser, "Committee@123");
                    await userManager.AddToRoleAsync(committeeUser, "Committee");

                    // Create lecturer record
                    var lecturer = new Lecturer
                    {
                        UserId = committeeUser.Id,
                        ProgramId = program.Id,
                        Domain = committeeData.Domain,
                        OfficeLocation = committeeData.Office,
                        IsCommittee = true
                    };

                    context.Lecturers.Add(lecturer);
                    await context.SaveChangesAsync();

                    // Create committee member record
                    var committeeMember = new CommitteeMember
                    {
                        LecturerId = lecturer.Id,
                        ProgramId = program.Id,
                        Role = committeeData.Role
                    };

                    context.CommitteeMembers.Add(committeeMember);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task CreateSupervisorAccounts(UserManager<ApplicationUser> userManager, ApplicationDbContext context, params Models.Program[] programs)
        {
            var programLookup = programs.ToDictionary(p => p.Name, p => p);

            var supervisorUsers = new[]
            {
                new { Email = "supervisor1.se@fyp1.edu.my", FullName = "Dr. Sarah Johnson", Domain = "Web Development", Office = "Block A, Level 4", ProgramName = "Software Engineering" },
                new { Email = "supervisor2.se@fyp1.edu.my", FullName = "Dr. Michael Brown", Domain = "Mobile Development", Office = "Block A, Level 5", ProgramName = "Software Engineering" },
                new { Email = "supervisor1.de@fyp1.edu.my", FullName = "Prof. Robert Wilson", Domain = "Machine Learning", Office = "Block C, Level 4", ProgramName = "Data Engineering" },
                new { Email = "supervisor2.de@fyp1.edu.my", FullName = "Dr. Linda Taylor", Domain = "Big Data Analytics", Office = "Block C, Level 5", ProgramName = "Data Engineering" }
            };

            foreach (var supervisorData in supervisorUsers)
            {
                // Skip if program doesn't exist
                if (!programLookup.ContainsKey(supervisorData.ProgramName))
                    continue;

                var supervisorUser = await userManager.FindByEmailAsync(supervisorData.Email);
                if (supervisorUser == null)
                {
                    var program = programLookup[supervisorData.ProgramName];
                    
                    supervisorUser = new ApplicationUser
                    {
                        UserName = supervisorData.Email,
                        Email = supervisorData.Email,
                        FullName = supervisorData.FullName,
                        EmailConfirmed = true,
                        ProgramId = program.Id
                    };

                    await userManager.CreateAsync(supervisorUser, "Supervisor@123");
                    await userManager.AddToRoleAsync(supervisorUser, "Supervisor");

                    // Create lecturer record for supervisor
                    var supervisorLecturer = new Lecturer
                    {
                        UserId = supervisorUser.Id,
                        ProgramId = program.Id,
                        Domain = supervisorData.Domain,
                        OfficeLocation = supervisorData.Office,
                        IsCommittee = false
                    };

                    context.Lecturers.Add(supervisorLecturer);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task CreateEvaluatorAccounts(UserManager<ApplicationUser> userManager, ApplicationDbContext context, params Models.Program[] programs)
        {
            var programLookup = programs.ToDictionary(p => p.Name, p => p);

            var evaluatorUsers = new[]
            {
                new { Email = "evaluator1.se@fyp1.edu.my", FullName = "Prof. Kevin Martinez", Domain = "Software Architecture", Office = "Block A, Level 7", ProgramName = "Software Engineering" },
                new { Email = "evaluator2.se@fyp1.edu.my", FullName = "Dr. Nancy Rodriguez", Domain = "System Analysis", Office = "Block A, Level 8", ProgramName = "Software Engineering" },
                new { Email = "evaluator1.de@fyp1.edu.my", FullName = "Dr. Steven Lewis", Domain = "Data Mining", Office = "Block C, Level 6", ProgramName = "Data Engineering" },
                new { Email = "evaluator2.de@fyp1.edu.my", FullName = "Dr. Dorothy Clark", Domain = "Statistical Analysis", Office = "Block C, Level 7", ProgramName = "Data Engineering" }
            };

            foreach (var evaluatorData in evaluatorUsers)
            {
                // Skip if program doesn't exist
                if (!programLookup.ContainsKey(evaluatorData.ProgramName))
                    continue;

                var evaluatorUser = await userManager.FindByEmailAsync(evaluatorData.Email);
                if (evaluatorUser == null)
                {
                    var program = programLookup[evaluatorData.ProgramName];
                    
                    evaluatorUser = new ApplicationUser
                    {
                        UserName = evaluatorData.Email,
                        Email = evaluatorData.Email,
                        FullName = evaluatorData.FullName,
                        EmailConfirmed = true,
                        ProgramId = program.Id
                    };

                    await userManager.CreateAsync(evaluatorUser, "Evaluator@123");
                    await userManager.AddToRoleAsync(evaluatorUser, "Evaluator");

                    // Create lecturer record for evaluator
                    var evaluatorLecturer = new Lecturer
                    {
                        UserId = evaluatorUser.Id,
                        ProgramId = program.Id,
                        Domain = evaluatorData.Domain,
                        OfficeLocation = evaluatorData.Office,
                        IsCommittee = false
                    };

                    context.Lecturers.Add(evaluatorLecturer);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task CreateStudentAccounts(UserManager<ApplicationUser> userManager, ApplicationDbContext context, params Models.Program[] programs)
        {
            var programLookup = programs.ToDictionary(p => p.Name, p => p);

            var studentUsers = new[]
            {
                // Software Engineering Students
                new { Email = "student1.se@fyp1.edu.my", FullName = "Ahmad Ali", StudentId = "SE12001", GPA = 3.75m, Session = "2024/2025", Semester = 1, ProgramName = "Software Engineering", SupervisorEmail = "supervisor1.se@fyp1.edu.my" },
                new { Email = "student2.se@fyp1.edu.my", FullName = "Siti Nurhaliza", StudentId = "SE12002", GPA = 3.85m, Session = "2024/2025", Semester = 1, ProgramName = "Software Engineering", SupervisorEmail = "supervisor2.se@fyp1.edu.my" },

                // Data Engineering Students
                new { Email = "student1.de@fyp1.edu.my", FullName = "Chen Wei Jie", StudentId = "DE12001", GPA = 3.80m, Session = "2024/2025", Semester = 1, ProgramName = "Data Engineering", SupervisorEmail = "supervisor1.de@fyp1.edu.my" },
                new { Email = "student2.de@fyp1.edu.my", FullName = "Nurul Ain", StudentId = "DE12002", GPA = 3.70m, Session = "2024/2025", Semester = 1, ProgramName = "Data Engineering", SupervisorEmail = "supervisor2.de@fyp1.edu.my" }
            };

            foreach (var studentData in studentUsers)
            {
                // Skip if program doesn't exist
                if (!programLookup.ContainsKey(studentData.ProgramName))
                    continue;

                var studentUser = await userManager.FindByEmailAsync(studentData.Email);
                
                if (studentUser == null)
                {
                    var program = programLookup[studentData.ProgramName];
                    
                    studentUser = new ApplicationUser
                    {
                        UserName = studentData.Email,
                        Email = studentData.Email,
                        FullName = studentData.FullName,
                        EmailConfirmed = true,
                        ProgramId = program.Id
                    };

                    await userManager.CreateAsync(studentUser, "Student@123");
                    await userManager.AddToRoleAsync(studentUser, "Student");

                    // Find supervisor by email
                    var supervisorUser = await userManager.FindByEmailAsync(studentData.SupervisorEmail);
                    var supervisor = supervisorUser != null ? 
                        await context.Lecturers.FirstOrDefaultAsync(l => l.UserId == supervisorUser.Id) : null;

                    // Create student record
                    var student = new Student
                    {
                        UserId = studentUser.Id,
                        ProgramId = program.Id,
                        StudentId = studentData.StudentId,
                        Session = studentData.Session,
                        Semester = studentData.Semester,
                        GPA = studentData.GPA,
                        SupervisorId = supervisor?.Id
                    };

                    context.Students.Add(student);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task CreateAdditionalSampleData(ApplicationDbContext context, Models.Program[] availablePrograms)
        {
            // Only create additional data if we have programs available
            if (availablePrograms.Length == 0) return;

            // Create sample proposals for each student
            var students = await context.Students
                .Include(s => s.User)
                .Include(s => s.Program)
                .Include(s => s.Supervisor)
                .ToListAsync();

            foreach (var student in students)
            {
                // Check if student already has a proposal
                var existingProposal = await context.Proposals.FirstOrDefaultAsync(p => p.StudentId == student.Id);
                if (existingProposal == null)
                {
                    var proposal = new Proposal
                    {
                        Title = $"FYP Proposal: {GetProposalTitle(student.User.FullName, student.Program.Name)}",
                        Description = $"Final Year Project proposal by {student.User.FullName} for {student.Program.Name} program. " +
                                    $"This project focuses on innovative solutions in {student.Supervisor?.Domain ?? "Technology"}.",
                        StudentId = student.Id,
                        Type = ProposalType.Development,
                        Status = ProposalStatus.Submitted,
                        Session = student.Session,
                        Semester = student.Semester,
                        SubmittedAt = DateTime.Now.AddDays(-3),
                        CreatedAt = DateTime.Now.AddDays(-5),
                        UpdatedAt = DateTime.Now.AddDays(-3)
                    };

                    context.Proposals.Add(proposal);
                    await context.SaveChangesAsync();

                    // Create evaluator assignments based on your specifications
                    await CreateEvaluatorAssignments(context, proposal, student);
                }
            }

            // Create sample comments for proposals
            var proposals = await context.Proposals
                .Include(p => p.Student)
                    .ThenInclude(s => s.Supervisor)
                .ToListAsync();

            foreach (var proposal in proposals.Take(2)) // Only add comments to first 2 proposals
            {
                var existingComments = await context.Comments.CountAsync(c => c.ProposalId == proposal.Id);
                if (existingComments == 0 && proposal.Student.SupervisorId.HasValue)
                {
                    var comments = new List<Comment>
                    {
                        new Comment
                        {
                            ProposalId = proposal.Id,
                            UserId = proposal.Student.Supervisor?.UserId ?? string.Empty,
                            Content = "Good progress on the proposal. Please elaborate on the methodology section.",
                            CreatedAt = DateTime.Now.AddDays(-2),
                            UpdatedAt = DateTime.Now.AddDays(-2)
                        },
                        new Comment
                        {
                            ProposalId = proposal.Id,
                            UserId = proposal.Student.UserId,
                            Content = "Thank you for the feedback. I will enhance the methodology section as suggested.",
                            CreatedAt = DateTime.Now.AddDays(-1),
                            UpdatedAt = DateTime.Now.AddDays(-1)
                        }
                    };

                    context.Comments.AddRange(comments);
                }
            }

            await context.SaveChangesAsync();
        }

        private static string GetProposalTitle(string studentName, string programName)
        {
            var titles = new Dictionary<string, string>
            {
                ["Ahmad Ali"] = "Smart Mobile Learning Platform for Programming Education",
                ["Siti Nurhaliza"] = "E-Commerce Mobile Application with AI Recommendation System",
                ["Chen Wei Jie"] = "Big Data Analytics for Student Performance Prediction",
                ["Nurul Ain"] = "Real-time Data Processing System for IoT Sensor Networks"
            };

            return titles.ContainsKey(studentName) ? titles[studentName] : $"Innovative {programName} Project";
        }

        private static async Task CreateEvaluatorAssignments(ApplicationDbContext context, Proposal proposal, Student student)
        {
            // Define evaluator assignments based on your specifications
            var evaluatorAssignments = new Dictionary<string, string[]>
            {
                ["Ahmad Ali"] = new[] { "evaluator1.se@fyp1.edu.my", "evaluator2.se@fyp1.edu.my" },
                ["Siti Nurhaliza"] = new[] { "evaluator1.se@fyp1.edu.my", "evaluator1.de@fyp1.edu.my" },
                ["Chen Wei Jie"] = new[] { "evaluator1.de@fyp1.edu.my", "evaluator2.de@fyp1.edu.my" },
                ["Nurul Ain"] = new[] { "evaluator1.de@fyp1.edu.my", "evaluator2.de@fyp1.edu.my" }
            };

            if (evaluatorAssignments.ContainsKey(student.User.FullName))
            {
                var evaluatorEmails = evaluatorAssignments[student.User.FullName];
                
                foreach (var evaluatorEmail in evaluatorEmails)
                {
                    var evaluatorUser = await context.Users.FirstOrDefaultAsync(u => u.Email == evaluatorEmail);
                    if (evaluatorUser != null)
                    {
                        var evaluatorLecturer = await context.Lecturers
                            .FirstOrDefaultAsync(l => l.UserId == evaluatorUser.Id);
                        
                        if (evaluatorLecturer != null)
                        {
                            var existingAssignment = await context.EvaluatorAssignments
                                .FirstOrDefaultAsync(ea => ea.ProposalId == proposal.Id && ea.EvaluatorId == evaluatorLecturer.Id);

                            if (existingAssignment == null)
                            {
                                var assignment = new EvaluatorAssignment
                                {
                                    ProposalId = proposal.Id,
                                    EvaluatorId = evaluatorLecturer.Id,
                                    AssignedDate = DateTime.Now.AddDays(-2),
                                    AssignedAt = DateTime.Now.AddDays(-2),
                                    IsActive = true
                                };

                                context.EvaluatorAssignments.Add(assignment);
                            }
                        }
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}