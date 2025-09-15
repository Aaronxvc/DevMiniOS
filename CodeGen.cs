// Path: Services/CodeGen.cs
using System.Text.Json;
using System.Text.RegularExpressions;
using DevMiniOS.Models;

namespace DevMiniOS.Services;

/// <summary>
/// Provides code generation services based on static templates.
/// </summary>
public static class CodeGen
{
    /// <summary>
    /// Generates code based on the provided command and language.
    /// </summary>
    /// <param name="command">The command to generate code for.</param>
    /// <param name="language">The target programming language (e.g., "csharp", "cpp", "javascript").</param>
    /// <returns>A list of generated files, each with a path and content.</returns>
    /// <example>
    /// <code>
    /// var command = new Command { Op = "make_controller", Name = "Foo", Parameters = "{}" };
    /// var generatedFiles = CodeGen.GenerateCode(command, "csharp");
    /// // generatedFiles will contain a list of GeneratedFile objects,
    /// // one for the controller file (e.g., "FooController.cs").
    /// </code>
    /// </example>
    public static List<GeneratedFile> GenerateCode(Command command, string language)
    {
        return command.Op switch
        {
            "make_controller" => GenerateController(command, language),
            "make_model" => GenerateModel(command, language),
            "wire_di" => GenerateDiRegistration(command, language),
            "add_tests" => GenerateTests(command, language),
            _ => new List<GeneratedFile>() // Return empty list for unknown operations.
        };
    }

    private static List<GeneratedFile> GenerateController(Command command, string language)
    {
        string controllerName = command.Name;
        string modelName = controllerName.Replace("Controller", ""); //Naively remove controller from name

        if (language == "csharp")
        {
            string controllerContent = $@"
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Controllers
{{
    [ApiController]
    [Route(""[controller]"")]
    public class {controllerName} : ControllerBase
    {{
        [HttpGet]
        public IActionResult Get()
        {{
            return Ok(""Hello from {controllerName}"");
        }}
    }}
}}";

            return new List<GeneratedFile> { new GeneratedFile { Path = $"Controllers/{controllerName}.cs", Contents = controllerContent } };
        }
        else
        {
            return new List<GeneratedFile>(); // Unsupported language.
        }
    }

    private static List<GeneratedFile> GenerateModel(Command command, string language)
    {
        if (language == "csharp")
        {
            string modelName = "ExampleModel"; // Default name
            string fields = "";

            if (command.Parameters != null)
            {
                try
                {
                    var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(command.Parameters);
                    if (parameters != null && parameters.ContainsKey("slots"))
                    {
                        fields = parameters["slots"];

                        string[] fieldArray = fields.Split(',');
                        fields = "";
                        foreach (string field in fieldArray)
                        {
                            string cleanedField = field.Trim();
                            string[] parts = cleanedField.Split(':');
                            if (parts.Length == 2)
                            {
                                string name = parts[0].Trim();
                                string type = parts[1].Trim();
                                fields += $"        public {type} {name} {{ get; set; }}\n";
                            }
                        }
                        modelName = "GeneratedModel";

                    }
                }
                catch (JsonException)
                {
                    // Handle JSON parsing errors
                }
            }

            string modelContent = $@"
namespace MyProject.Models
{{
    public class {modelName}
    {{
{fields}
    }}
}}";

            return new List<GeneratedFile> { new GeneratedFile { Path = $"Models/{modelName}.cs", Contents = modelContent } };
        }
        else
        {
            return new List<GeneratedFile>(); // Unsupported language.
        }
    }

    private static List<GeneratedFile> GenerateDiRegistration(Command command, string language)
    {
        if (language == "csharp")
        {
            string className = "Foo"; // Replace with parsing logic if needed from command.Name or Parameters

            string registrationContent = $@"
using Microsoft.Extensions.DependencyInjection;

namespace MyProject.Configuration
{{
    public static class DependencyInjectionConfig
    {{
        public static void RegisterDependencies(this IServiceCollection services)
        {{
            services.AddScoped<IFoo, {className}>(); // Replace IFoo with interface if available.
        }}
    }}
}}";
            return new List<GeneratedFile> { new GeneratedFile { Path = $"Configuration/DependencyInjectionConfig.cs", Contents = registrationContent } };
        }
        else
        {
            return new List<GeneratedFile>(); // Unsupported language.
        }
    }

    private static List<GeneratedFile> GenerateTests(Command command, string language)
    {
        if (language == "csharp")
        {
            string testClassName = "ExampleTests"; //Replace with parsing logic if needed.
            string testContent = $@"
using Xunit;

namespace MyProject.Tests
{{
    public class {testClassName}
    {{
        [Fact]
        public void Test1()
        {{
            Assert.True(true);
        }}
    }}
}}";
            return new List<GeneratedFile> { new GeneratedFile { Path = $"Tests/{testClassName}.cs", Contents = testContent } };
        }
        else
        {
            return new List<GeneratedFile>(); // Unsupported language.
        }
    }
}

/// <summary>
/// Represents a generated file with its path and contents.
/// </summary>
public class GeneratedFile
{
    /// <summary>
    /// The path of the generated file.
    /// </summary>
    public string Path { get; set; } = "";

    /// <summary>
    /// The contents of the generated file.
    /// </summary>
    public string Contents { get; set; } = "";
}