using DevMiniOS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevMiniOS.Views.Index
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string DslSummary { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<GeneratedFile> GeneratedFiles { get; set; } = new List<GeneratedFile>();
    }
}
