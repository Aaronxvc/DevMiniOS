using DevMiniOS.Interfaces;
using DevMiniOS.Models;
using DevMiniOS.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevMiniOS.Controllers
{
    public class IdeController : Controller
    {
        private readonly IPlanner _planner;
        private readonly OutputService _outputSerivce;
        private readonly GitService _gitService;

        public IdeController(IPlanner planner, OutputService outputSerivce, GitService gitService)
        {
            _planner = planner;
            _outputSerivce = outputSerivce;
            _gitService = gitService;
        }

        public IActionResult Index()
        {
            return View(); // return the Index view 
        }

        [HttpPost]
        public async Task<IActionResult> Generate(string naturalLanguage, string language)
        { 
            //Planning
            Plan plan = await _planner.CreatePlanAsync(naturalLanguage);

            //Output
            OutputResult outputResult = _outputSerivce.GenerateOutput(plan, language);

            //Write files to disk 
            string projectRoot = "./output"; // Or get from config
            string languageDirectory = Path.Combine(projectRoot, naturalLanguage);
            if (!Directory.Exists(languageDirectory))
            {
                Directory.CreateDirectory(languageDirectory);
            }

            foreach (var file in outputResult.GeneratedFiles)
            {
                string fullpath = Path.Combine(languageDirectory, file.Path);
                string? directory = Path.GetDirectoryName(fullpath);
                if(!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) 
                { 
                    Directory.CreateDirectory(directory);
                }

                System.IO.File.WriteAllText(fullpath, file.Contents);

            }

            //Git Operation
            _gitService.InitializeRepository();
            _gitService.AddChanges();
            _gitService.CommitChanges("Commit Successful");

            // Pass data to view
            ViewBag.DslSummary = outputResult.DslSummary;
            ViewBag.GeneratedFiles = outputResult.GeneratedFiles;

            return View("Output");



        }
    }
}
