using System.Text.Json.Serialization;

namespace DevMiniOS.Models
{
    /// <summary>
    /// Repersents a single command within a plan
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Gets or sets operation type of the command (e.g., "make_controller").
        /// </summary>
        [JsonPropertyName("op")]
        public string Op { get; set; } = string.Empty;

        ///<summary>
        ///Gets and sets the name associated with the command (e.g., controller name).
        /// </summary>

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional parameters for the command
        /// </summary>
        [JsonPropertyName("parameters")]
        public string Parameters { get; set; } = string.Empty;

    }
}
