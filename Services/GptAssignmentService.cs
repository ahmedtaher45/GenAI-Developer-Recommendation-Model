using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace GenAI_Recommendation_Model.Services
{
    public class GptAssignmentService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey = "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";
        private readonly string _model = "mistralai/mistral-7b-instruct:free";

        public GptAssignmentService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetDeveloperRecommendation(string prompt)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new {role = "system", content ="You are an AI that recommends the best developer for a task based on performance and skill."},
                    new {role ="user", content = prompt}
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Headers.Add("HTTP-Referer", "https://localhost");
            request.Headers.Add("X-Title", "DeveloperAssignment");

            var json = JsonSerializer.Serialize(requestBody);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);

            var reply = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

            return reply;
        }
    }
}