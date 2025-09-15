// Path: Services/OutputService.cs
using System.Text.Json;
using DevMiniOS.Models;

namespace DevMiniOS.Services;

/// <summary>
/// Provides a service to generate a human-readable DSL summary and the generated files from a Plan.
/// </summary>
public class OutputService
{
    /// <summary>
    /// Generates a human-readable DSL summary and the generated files from a Plan.
    /// </summary>
    /// <param name="plan">The Plan to process.</param>
    /// <param name="language">The target programming language.</param>
    /// <returns>An OutputResult object containing the DSL summary and generated files.</returns>
    /// <example>
    /// <code>
    /// var plan = new Plan { Commands = new List<Command> { new Command { Op = "make_controller", Name = "FooController", Parameters = "{}" } } };
    /// var outputResult = outputService.GenerateOutput(plan, "csharp");
    /// // outputResult.DslSummary will contain a human-readable summary of the Plan.
    /// // outputResult.GeneratedFiles will contain the generated files.
    /// </code>
    /// </example>
    public OutputResult GenerateOutput(Plan plan, string language)
    {
        string dslSummary = GenerateDslSummary(plan);
        List<GeneratedFile> generatedFiles = GenerateFiles(plan, language);

        return new OutputResult { DslSummary = dslSummary, GeneratedFiles = generatedFiles };
    }

    private string GenerateDslSummary(Plan plan)
    {
        string summary = "Plan:\n";
        if (plan.Commands != null)
        {
            foreach (var command in plan.Commands)
            {
                summary += $"  Op: {command.Op}\n";
                summary += $"  Name: {command.Name}\n";
                summary += $"  Parameters: {command.Parameters}\n";
                summary += "---\n";
            }
        }
        else
        {
            summary += "No commands in plan.\n";
        }
        return summary;
    }

    private List<GeneratedFile> GenerateFiles(Plan plan, string language)
    {
        List<GeneratedFile> allGeneratedFiles = new();
        if (plan.Commands != null)
        {
            foreach (var command in plan.Commands)
            {
                List<GeneratedFile> generatedFiles = CodeGen.GenerateCode(command, language);
                allGeneratedFiles.AddRange(generatedFiles);
            }
        }
        return allGeneratedFiles;
    }
}

/// <summary>
/// Represents the output result, containing the DSL summary and generated files.
/// </summary>
public class OutputResult
{
    /// <summary>
    /// The human-readable DSL summary.
    /// </summary>
    public string DslSummary { get; set; } = "";

    /// <summary>
    /// The list of generated files.
    /// </summary>
    public List<GeneratedFile> GeneratedFiles { get; set; } = new();
}