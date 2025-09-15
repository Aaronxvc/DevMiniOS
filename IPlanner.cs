using DevMiniOS.Models;

namespace DevMiniOS.Interfaces
{
    /// <summary>
    /// Interface for planner that converts free text into a structerd plan. 
    /// </summary>
    public interface IPlanner
    {
        /// <summary>
        /// Creates a plan from a given free-text prompt
        /// </summary>
        ///<param name="freeText">The free-Text prompt to interpret. </param>
        ///<returns> A <see cref="Plan"/> object representing the generated plan.</returns>
        Task<Plan> CreatePlanAsync(string freeText);
    }
}
