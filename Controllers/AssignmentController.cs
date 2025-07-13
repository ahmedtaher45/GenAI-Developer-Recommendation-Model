using System.Text;
using GenAI_Recommendation_Model.DTOs;
using GenAI_Recommendation_Model.Models;
using GenAI_Recommendation_Model.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GenAI_Recommendation_Model.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly GptAssignmentService gpt;
        private readonly GenAIContext context;

        public AssignmentController(GptAssignmentService _gpt, GenAIContext _context)
        {
            gpt = _gpt;
            context = _context;
        }

        [HttpPost]
        public async Task<IActionResult> Recommend([FromBody] TaskDTO task)
        {
            var developers = await context.Developers.ToListAsync();

            if (developers == null || developers.Count == 0)
            {
                return NotFound("No developers found in the database.");
            }

            (int minScore, int maxScore) = task.Difficulty?.ToLower() switch
            {
                "easy" => (50, 69),
                "medium" => (70, 84),
                "hard" => (85, 100),
                _ => (0, 100)
            };

            var suitableDevs = developers
                .Where(d => d.Score >= minScore && d.Score <= maxScore)
                .OrderByDescending(d => d.Score)
                .ToList();

            if (!suitableDevs.Any())
            {
                return Ok(new
                {
                    message = $"No developer fits the score range ({minScore}-{maxScore}) for this task.",
                    recommendedDeveloper = (Developer?)null
                });
            }

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine($"Task: {task.Description}");
            promptBuilder.AppendLine($"Difficulty: {task.Difficulty}");
            promptBuilder.AppendLine("Available Developers:");

            foreach (var dev in suitableDevs)
            {
                promptBuilder.AppendLine($"- {dev.Name}, Score: {dev.Score}");
            }

            promptBuilder.AppendLine("Recommend the best developer for this task and explain why.");

            string prompt = promptBuilder.ToString();

            var aiReply = await gpt.GetDeveloperRecommendation(prompt);

            return Ok(new
            {
                task = task.Description,
                difficulty = task.Difficulty,
                recommendation = aiReply
            });
        }

    }
}
