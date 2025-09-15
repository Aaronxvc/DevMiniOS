using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DevMiniOS.Controllers
{
    /// <summary>
    /// Provides API endpoints for training and sampling from the Python-based
    /// mini-LM.
    /// </summary>
    /// <remarks>
    /// This controller acts as a bridge between the C# MVC application and the
    /// Python mini-LM. It uses `ProcessStartInfo` to execute the Python script
    /// and handles input/output via standard streams.
    ///
    /// To change paths, modify the `appsettings.json` file or environment variables
    /// for the Python executable path and model location.
    ///
    /// Example usage:
    /// POST /api/minilm/train
    /// GET /api/minilm/sample?prefix=open&maxTokens=24
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class MiniModelController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiniModelController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MiniModelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Trains the Python mini-LM with the current dataset.
        /// </summary>
        /// <returns>A result indicating success or failure.</returns>
        /// <remarks>
        /// This endpoint executes the Python training script using `ProcessStartInfo`.
        /// It returns `{ ok: true }` if the training process completes successfully.
        /// Any errors during the process are logged to the console.
        /// </remarks>
        [HttpPost("train")]
        public async Task<IActionResult> TrainModel()
        {
            try
            {
                string pythonPath = _configuration["MiniModel:PythonPath"] ?? "python"; // Default to "python"
                string corpusPath = Path.Combine("MiniModel", "py", "dataset_seed.txt"); // Assumed location
                string modelPath = Path.Combine("MiniModel", "out", "model.json");  // Save in /MiniModel/out

                //Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(modelPath)!);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"MiniModel/py/outloud_mini_lm.py train --corpus \"{corpusPath}\" --model \"{modelPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Directory.GetCurrentDirectory() // Set the working directory

                };

                using (Process process = Process.Start(psi)!)
                {
                    string error = await process.StandardError.ReadToEndAsync();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        Console.Error.WriteLine($"Python training failed: {error}");
                        return StatusCode(500, new { ok = false, error = error });
                    }

                    Console.WriteLine($"Python training output: {await process.StandardOutput.ReadToEndAsync()}"); //Log output

                    return Ok(new { ok = true, modelPath = modelPath });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error during training: {ex}");
                return StatusCode(500, new { ok = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Samples a completion from the Python mini-LM given a prefix and maximum
        /// number of tokens.
        /// </summary>
        /// <param name="prefix">The prefix to start the completion with.</param>
        /// <param name="maxTokens">The maximum number of tokens to generate.</param>
        /// <returns>A JSON object containing the completion and tokens.</returns>
        /// <remarks>
        /// This endpoint executes the Python sampling script using `ProcessStartInfo`.
        /// It passes the prefix and maxTokens parameters to the script and returns
        /// the JSON output. Any errors during the process are logged to the console.
        /// </remarks>
        [HttpGet("sample")]
        public async Task<IActionResult> SampleModel(string prefix, int maxTokens = 10)
        {
            try
            {
                string pythonPath = _configuration["MiniModel:PythonPath"] ?? "python"; // Default to "python"
                string modelPath = Path.Combine("MiniModel", "out", "model.json"); // Assumed location

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"MiniModel/py/outloud_mini_lm.py sample --model \"{modelPath}\" --prefix \"{prefix}\" --max_tokens {maxTokens}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Directory.GetCurrentDirectory() // Set the working directory
                };

                using (Process process = Process.Start(psi)!)
                {
                    string error = await process.StandardError.ReadToEndAsync();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        Console.Error.WriteLine($"Python sampling failed: {error}");
                        return StatusCode(500, new { ok = false, error = error });
                    }

                    string jsonOutput = await process.StandardOutput.ReadToEndAsync();
                    try
                    {
                        JsonDocument result = JsonDocument.Parse(jsonOutput);
                        return Ok(result);
                    }
                    catch (JsonException ex)
                    {
                        Console.Error.WriteLine($"Invalid JSON from Python: {jsonOutput}  Error: {ex.Message}");
                        return StatusCode(500, new { ok = false, error = "Invalid JSON from Python" });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error during sampling: {ex}");
                return StatusCode(500, new { ok = false, error = ex.Message });
            }
        }

        [HttpPost("add_training_data")]
        public async Task<IActionResult> AddTrainingData([FromBody] string NewTrainingText)
        {
            try
            {
                string corpusPath = Path.Combine("MiniModel", "py", "dataset_seed.txt");

                // **IMPORTANT: Sanitize the input to prevent injection attacks!**
                string sanitizedText = NewTrainingText.Replace("\"", ""); //Removes qoutes TODO: Add more input sanitation 

                await System.IO.File.AppendAllTextAsync(corpusPath, sanitizedText + Environment.NewLine);

                //Retrain the model

                return await TrainModel(); // Reuse the existing TrainModel endpoint
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding training data: {ex}");
                return StatusCode(500, new { ok = false, error = ex.Message });
            }
        }
    }
}