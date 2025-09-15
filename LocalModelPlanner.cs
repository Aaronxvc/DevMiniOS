using DevMiniOS.Models;
using global::DevMiniOS.Interfaces;
using global::DevMiniOS.Model;
using System.Threading.Tasks;


namespace DevMiniOS.Services
{
        /// <summary>
        /// Placeholder for a planner that uses a local LLM.  Not implemented yet.
        /// </summary>
        public sealed class LocalModelPlanner : IPlanner
        {
            /// <summary>
            /// Creates a plan from a given free-text prompt.
            /// </summary>
            /// <param name="freeText">The free-text prompt to interpret.</param>
            /// <returns>A <see cref="Plan"/> object representing the generated plan.</returns>
            /// <exception cref="NotSupportedException">Thrown because this planner is not yet implemented.</exception>
            public async Task<Plan> CreatePlanAsync(string freeText)
            {
                throw new NotSupportedException("Local model planner is not yet implemented.  Configure 'Planner.Mode' to 'Rules' in appsettings.json.");
            }
        }
    
}
