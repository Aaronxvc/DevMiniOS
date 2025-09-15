using System.Text.RegularExpressions;

namespace DevMiniOS.Services;

/// <summary>
/// Provides text normalization services, including lowercasing, punctuation removal, and basic stemming.
/// </summary>
public static class TextNormalizer
{
    /// <summary>
    /// Normalizes the input text by lowercasing, trimming whitespace and punctuation, and applying basic stemming.
    /// </summary>
    /// <param name="text">The input text to normalize.</param>
    /// <returns>The normalized text.</returns>
    /// <example>
    /// <code>
    /// string normalized = TextNormalizer.Normalize("  Create a FooController! ");
    /// // normalized == "create a foocontroller"
    /// </code>
    /// </example>
    public static string Normalize(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // Lowercase and trim whitespace
        text = text.ToLower().Trim();

        // Remove punctuation
        text = Regex.Replace(text, @"[^\w\s]", string.Empty);

        // Basic stemming (remove "ing", "ed", "s" suffixes) -- very naive!
        text = Regex.Replace(text, @"(ing|ed|s)$", string.Empty);

        return text;
    }

    /// <summary>
    /// Expands a sequence of tokens by replacing them with their synonyms.
    /// </summary>
    /// <param name="tokens">The sequence of tokens to expand.</param>
    /// <returns>The expanded sequence of tokens.</returns>
    /// <example>
    /// <code>
    /// var tokens = new[] { "make", "a", "foo", "controller" };
    /// var expandedTokens = TextNormalizer.ExpandSynonyms(tokens);
    /// // The result would include tokens like "create", "generate" etc.
    /// </code>
    /// </example>
    public static IEnumerable<string> ExpandSynonyms(IEnumerable<string> tokens)
    {
        foreach (var token in tokens)
        {
            if (Synonyms.TryGetValue(token, out string[]? synonyms))
            {
                foreach (var synonym in synonyms)
                {
                    yield return synonym;
                }
            }
            else
            {
                yield return token;
            }
        }
    }

    /// <summary>
    /// This is added to store the synonyms
    /// </summary>
    private static readonly Dictionary<string, string[]> Synonyms = new()
    {
        {"crud", new[] {"create", "read", "update", "delete"} },
        {"make", new[] {"create", "generate"} },
        {"wire", new[] {"register", "inject"} },
        {"di", new[] {"dependecy injection"} }
    };

}
