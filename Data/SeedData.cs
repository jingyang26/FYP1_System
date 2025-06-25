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
        }
    }
}
