using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FYP1System.Pages.Evaluator
{
    [Authorize(Roles = "Evaluator")]
    public class GuidelinesModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
