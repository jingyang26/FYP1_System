using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FYP1System.Models;
using FYP1System.Data;

namespace FYP1System.Data
{
    public static class SeedData
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

            // Create default admin user
            await CreateDefaultUsers(userManager, context);
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
                    }
                };

                await context.Programs.AddRangeAsync(programs);
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateDefaultUsers(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            // Create default admin
            var adminEmail = "admin@fyp1.edu.my";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create sample committee member
            var committeeEmail = "committee@fyp1.edu.my";
            var committeeUser = await userManager.FindByEmailAsync(committeeEmail);

            if (committeeUser == null)
            {
                var program = await context.Programs.FirstAsync(p => p.Name == "Software Engineering");
                
                committeeUser = new ApplicationUser
                {
                    UserName = committeeEmail,
                    Email = committeeEmail,
                    FullName = "Dr. John Smith",
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
                    Domain = "Software Engineering",
                    OfficeLocation = "Block A, Level 2",
                    IsCommittee = true
                };

                context.Lecturers.Add(lecturer);
                await context.SaveChangesAsync();

                // Create committee member record
                var committeeMember = new CommitteeMember
                {
                    LecturerId = lecturer.Id,
                    ProgramId = program.Id,
                    Role = "Chair"
                };

                context.CommitteeMembers.Add(committeeMember);
                await context.SaveChangesAsync();
            }

            // Create sample student
            var studentEmail = "student@fyp1.edu.my";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);

            if (studentUser == null)
            {
                var program = await context.Programs.FirstAsync(p => p.Name == "Software Engineering");
                
                studentUser = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FullName = "Ahmad Ali",
                    EmailConfirmed = true,
                    ProgramId = program.Id
                };

                await userManager.CreateAsync(studentUser, "Student@123");
                await userManager.AddToRoleAsync(studentUser, "Student");

                // Create student record
                var student = new Student
                {
                    UserId = studentUser.Id,
                    ProgramId = program.Id,
                    StudentId = "SE12345",
                    Session = "2024/2025",
                    Semester = 1,
                    GPA = 3.75m
                };

                context.Students.Add(student);
                await context.SaveChangesAsync();
            }

            // Create sample supervisor
            var supervisorEmail = "supervisor@fyp1.edu.my";
            var supervisorUser = await userManager.FindByEmailAsync(supervisorEmail);

            if (supervisorUser == null)
            {
                var program = await context.Programs.FirstAsync(p => p.Name == "Software Engineering");
                
                supervisorUser = new ApplicationUser
                {
                    UserName = supervisorEmail,
                    Email = supervisorEmail,
                    FullName = "Dr. Sarah Johnson",
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
                    Domain = "Research",
                    OfficeLocation = "Block B, Level 3",
                    IsCommittee = false
                };

                context.Lecturers.Add(supervisorLecturer);
                await context.SaveChangesAsync();

                // Assign supervisor to the student
                var existingStudent = await context.Students.FirstOrDefaultAsync(s => s.StudentId == "SE12345");
                if (existingStudent != null)
                {
                    existingStudent.SupervisorId = supervisorLecturer.Id;
                    await context.SaveChangesAsync();
                }
            }

            // Create sample evaluator
            var evaluatorEmail = "evaluator@fyp1.edu.my";
            var evaluatorUser = await userManager.FindByEmailAsync(evaluatorEmail);

            if (evaluatorUser == null)
            {
                var program = await context.Programs.FirstAsync(p => p.Name == "Data Engineering");
                
                evaluatorUser = new ApplicationUser
                {
                    UserName = evaluatorEmail,
                    Email = evaluatorEmail,
                    FullName = "Prof. Michael Brown",
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
                    Domain = "Data Science",
                    OfficeLocation = "Block C, Level 1",
                    IsCommittee = false
                };

                context.Lecturers.Add(evaluatorLecturer);
                await context.SaveChangesAsync();
            }

            // Create another committee member for Data Engineering program
            var committee2Email = "committee2@fyp1.edu.my";
            var committee2User = await userManager.FindByEmailAsync(committee2Email);

            if (committee2User == null)
            {
                var program = await context.Programs.FirstAsync(p => p.Name == "Data Engineering");
                
                committee2User = new ApplicationUser
                {
                    UserName = committee2Email,
                    Email = committee2Email,
                    FullName = "Dr. Lisa Wong",
                    EmailConfirmed = true,
                    ProgramId = program.Id
                };

                await userManager.CreateAsync(committee2User, "Committee@123");
                await userManager.AddToRoleAsync(committee2User, "Committee");

                // Create lecturer record
                var lecturer2 = new Lecturer
                {
                    UserId = committee2User.Id,
                    ProgramId = program.Id,
                    Domain = "Data Engineering",
                    OfficeLocation = "Block C, Level 2",
                    IsCommittee = true
                };

                context.Lecturers.Add(lecturer2);
                await context.SaveChangesAsync();

                // Create committee member record
                var committeeMember2 = new CommitteeMember
                {
                    LecturerId = lecturer2.Id,
                    ProgramId = program.Id,
                    Role = "Member"
                };

                context.CommitteeMembers.Add(committeeMember2);
                await context.SaveChangesAsync();
            }

            // Create additional sample data for testing
            await CreateAdditionalSampleData(context);
        }

        private static async Task CreateAdditionalSampleData(ApplicationDbContext context)
        {
            // Add more students and assign supervisors
            var seProgram = await context.Programs.FirstAsync(p => p.Name == "Software Engineering");
            var deProgram = await context.Programs.FirstAsync(p => p.Name == "Data Engineering");
            var supervisorLecturer = await context.Lecturers.FirstAsync(l => l.Domain == "Research");

            // Get existing users for additional students
            var adminUser = await context.Users.FirstAsync(u => u.Email == "admin@fyp1.edu.my");
            var committeeUser = await context.Users.FirstAsync(u => u.Email == "committee@fyp1.edu.my");

            // Create additional students if they don't exist
            var existingStudentCount = await context.Students.CountAsync();
            if (existingStudentCount < 3)
            {
                var studentsToAdd = new List<Student>
                {
                    new Student
                    {
                        UserId = adminUser.Id, // Use admin user for additional student
                        ProgramId = seProgram.Id,
                        StudentId = "SE12346",
                        Session = "2024/2025",
                        Semester = 1,
                        GPA = 3.25m,
                        SupervisorId = supervisorLecturer.Id
                    },
                    new Student
                    {
                        UserId = committeeUser.Id, // Use committee user for additional student
                        ProgramId = deProgram.Id,
                        StudentId = "DE12347",
                        Session = "2024/2025",
                        Semester = 1,
                        GPA = 3.90m
                        // No supervisor assigned yet
                    }
                };

                context.Students.AddRange(studentsToAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}
