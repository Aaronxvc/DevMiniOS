using System.Text.Json.Serialization;

namespace DevMiniOS.Models
{
    ///<summary> 
    ///Repersents a plan consisting of commands to be executed. 
    /// </summary>
    public class Plan
    {
            ///<summary>
            ///Gets or sets the list of commands in the plan. 
            /// </summary>

            [JsonPropertyName("commands")]
            public List<Command> Commands { get; set; } = new List<Command>();
        
    }
}
