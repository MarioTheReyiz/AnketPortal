using AnketPortal.API.DTOs;
using AnketPortal.API.Models;
using AnketPortal.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AnketPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Cevap vermek için giriş yapmış olmak şart
    public class AnswerController : ControllerBase
    {
        private readonly IGenericRepository<SurveyAnswer> _answerRepo;

        public AnswerController(IGenericRepository<SurveyAnswer> answerRepo)
        {
            _answerRepo = answerRepo;
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitAnswers(AnswerSubmitDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var item in model.Answers)
            {
                var answer = new SurveyAnswer
                {
                    SurveyId = model.SurveyId,
                    QuestionId = item.QuestionId,
                    TextAnswer = item.TextAnswer,
                    SelectedOptionId = item.SelectedOptionId,
                    AppUserId = userId!,
                    CreatedDate = DateTime.Now
                };
                await _answerRepo.AddAsync(answer);
            }

            await _answerRepo.SaveAsync();
            return Ok(new ResultDto { Status = true, Message = "Cevaplarınız başarıyla kaydedildi." });
        }
    }
}