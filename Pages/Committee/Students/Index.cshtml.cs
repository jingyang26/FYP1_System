using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using FYP1System.Data;
using FYP1System.Models;
using StudentModel = FYP1System.Models.Student;

namespace FYP1System.Pages.Committee.Students
{
    [Authorize(Policy = "CommitteeOnly")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StudentModel> Students { get; set; } = new();
        public List<Lecturer> AvailableSupervisors { get; set; } = new();
        public SelectList ProgramList { get; set; } = new(new List<SelectListItem>());
        
        [BindProperty]
        public string? SelectedSession { get; set; }
        
        [BindProperty]
        public int? SelectedSemester { get; set; }
        
        [BindProperty]
        public int? SelectedProgramId { get; set; }

        public async Task OnGetAsync(string? session, int? semester, int? programId)
        {
            SelectedSession = session;
            SelectedSemester = semester;
            SelectedProgramId = programId;

            await LoadDataAsync();
        }

        public IActionResult OnPostFilter()
        {
            return RedirectToPage(new { 
                session = SelectedSession, 
                semester = SelectedSemester, 
                programId = SelectedProgramId 
            });
        }

        public async Task<IActionResult> OnPostAssignSupervisorAsync(int studentId, int supervisorId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound();
            }

            student.SupervisorId = supervisorId;
            await _context.SaveChangesAsync();

            var supervisor = await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == supervisorId);

            TempData["SuccessMessage"] = $"Supervisor {supervisor?.User?.FullName} assigned successfully to student.";
            return RedirectToPage(new { 
                session = SelectedSession, 
                semester = SelectedSemester, 
                programId = SelectedProgramId 
            });
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound();
            }

            student.IsActive = !student.IsActive;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Student status updated successfully.";
            return RedirectToPage(new { 
                session = SelectedSession, 
                semester = SelectedSemester, 
                programId = SelectedProgramId 
            });
        }

        private async Task LoadDataAsync()
        {
            // Load students with filters - ensuring all navigation properties are loaded
            var studentsQuery = _context.Students
                .Include(s => s.User)
                .Include(s => s.Program)
                .Include(s => s.Supervisor)
                    .ThenInclude(sup => sup!.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SelectedSession))
            {
                studentsQuery = studentsQuery.Where(s => s.Session == SelectedSession);
            }

            if (SelectedSemester.HasValue)
            {
                studentsQuery = studentsQuery.Where(s => s.Semester == SelectedSemester);
            }

            if (SelectedProgramId.HasValue)
            {
                studentsQuery = studentsQuery.Where(s => s.ProgramId == SelectedProgramId);
            }

            Students = await studentsQuery
                .OrderBy(s => s.User!.FullName)
                .ToListAsync();

            // Load available supervisors
            AvailableSupervisors = await _context.Lecturers
                .Include(l => l.User)
                .Where(l => l.IsActive)
                .OrderBy(l => l.User!.FullName)
                .ToListAsync();

            // Load programs for filter
            var programs = await _context.Programs
                .OrderBy(p => p.Name)
                .ToListAsync();

            ProgramList = new SelectList(programs, "Id", "Name", SelectedProgramId);
        }
    }
}
