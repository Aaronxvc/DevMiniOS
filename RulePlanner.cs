// Path: Services/RulePlanner.cs
using System.Text.Json;
using System.Text.RegularExpressions;
using DevMiniOS.Interfaces;
using DevMiniOS.Models;

namespace DevMiniOS.Services;

/// <summary>
/// Implements a rule-based planner using regular expressions and intent scoring to map natural language input to commands,
/// with a catch-all fallback for unhandled input.
/// </summary>
public class RulePlanner : IPlanner
{
    private List<Regex> _controllerRules = new();

    public RulePlanner()
    {
        // Initialize rules with synonym expansion.
        InitializeRules();
    }

    private void InitializeRules()
    {
        _controllerRules = new()
        {
            CreateRegexFromPattern("make a (.*?) controller with full crud"),
        };
    }

    private Regex CreateRegexFromPattern(string pattern)
    {
        var tokens = pattern.Split(' ');
        var expandedTokens = TextNormalizer.ExpandSynonyms(tokens).Distinct().ToArray();
        var regexPattern = string.Join(" ", expandedTokens);

        regexPattern = Regex.Replace(regexPattern, @"\b(create|read|update|delete)\b", "($1)");
        regexPattern = Regex.Replace(regexPattern, @"\b(create|generate)\b", "($1)");
        regexPattern = Regex.Replace(regexPattern, @"\b(register|inject)\b", "($1)");
        regexPattern = Regex.Replace(regexPattern, @"\b(dependency injection)\b", "($1)");

        return new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    private class IntentRule
    {
        public string Op { get; set; } = "";
        public Dictionary<string, int> Keywords { get; set; } = new();
        public Regex? SlotRegex { get; set; }
    }


    private readonly List<IntentRule> _intentRules = new()
    {
        new IntentRule
        {
            Op = "make_model",
            Keywords = new Dictionary<string, int> { { "model", 5 }, { "class", 3 }, { "entity", 3 }, { "create", 2 } },
            SlotRegex = new Regex(@"\((.*?)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase) // Extract fields inside parentheses
        },
        new IntentRule
        {
            Op = "wire_di",
            Keywords = new Dictionary<string, int> { { "wire", 5 }, { "inject", 5 }, { "dependency", 3 }, { "register", 2 }, { "di", 4 } }
        },
        new IntentRule
        {
            Op = "add_tests",
            Keywords = new Dictionary<string, int> { { "test", 5 }, { "tests", 5 }, { "unit", 3 }, { "integration", 3 } }
        }

    };

    private Command? ClassifyIntent(string normalizedInput)
    {
        IntentRule? bestRule = null;
        int bestScore = 0;

        foreach (var rule in _intentRules)
        {
            int score = 0;
            foreach (var keyword in rule.Keywords)
            {
                if (normalizedInput.Contains(keyword.Key))
                {
                    score += keyword.Value;
                }
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestRule = rule;
            }
        }

        if (bestRule != null && bestScore > 5) // Threshold
        {
            string parameters = "{}";
            if (bestRule.SlotRegex != null)
            {
                var match = bestRule.SlotRegex.Match(normalizedInput);
                if (match.Success)
                {
                    parameters = JsonSerializer.Serialize(new { slots = match.Groups[1].Value });
                }
            }

            return new Command { Op = bestRule.Op, Name = "natural_language", Parameters = parameters };
        }

        return null;
    }



    /// <inheritdoc />
    public async Task<Plan> CreatePlanAsync(string naturalLanguage)
    {
        // Normalize the input
        string normalizedInput = TextNormalizer.Normalize(naturalLanguage);

        foreach (var rule in _controllerRules)
        {
            var match = rule.Match(normalizedInput);
            if (match.Success)
            {
                var controllerName = match.Groups[1].Value;
                return new Plan
                {
                    Commands = new List<Command>
                    {
                        new() { Op = "make_controller", Name = controllerName, Parameters = "{}" }
                    }
                };
            }
        }

        // If no regex matches, try intent classification
        var intentCommand = ClassifyIntent(normalizedInput);
        if (intentCommand != null)
        {
            return new Plan { Commands = new List<Command> { intentCommand } };
        }

        // Catch-all Fallback
        return new Plan
        {
            Commands = new List<Command>
            {
                new Command
                {
                    Op = "assist_request",
                    Name = "natural_language",
                    Parameters = JsonSerializer.Serialize(new { text = naturalLanguage, hints = "unclear intent" })
                }
            }
        };
    }
}