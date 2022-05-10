using Microsoft.AspNetCore.Mvc;

namespace SemperPrecisStageTraker.API.Tests.Controllers.Common
{
    /// <summary>
    /// Structure to hold action result response
    /// </summary>
    /// <typeparam name="TActionResult">Type of action result implementation</typeparam>
    /// <typeparam name="TData">Type of data expected</typeparam>
    public class ActionResultStructure<TActionResult, TData>
        where TActionResult : IActionResult
    {
        /// <summary>
        /// Response
        /// </summary>
        public TActionResult Response { get; set; }

        /// <summary>
        /// Data inside response (parsed)
        /// </summary>
        public TData Data { get; set; }
    }
}