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
    public class SurveyController : ControllerBase
    {
        private readonly IGenericRepository<Survey> _surveyRepo;
        private readonly IGenericRepository<Question> _questionRepo;

        public SurveyController(IGenericRepository<Survey> surveyRepo, IGenericRepository<Question> questionRepo)
        {
            _surveyRepo = surveyRepo;
            _questionRepo = questionRepo;
        }

        // GENEL ARA YÜZ: Aktif anketleri listeler
        [HttpGet]
        public async Task<IActionResult> GetSurveys()
        {
            var surveys = await _surveyRepo.GetAllAsync();
            var result = surveys.Where(s => s.IsActive).Select(s => new SurveyDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                CreatedDate = s.CreatedDate
            });
            return Ok(result);
        }

        // GENEL ARA YÜZ: Anket detayını getirir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSurveyById(int id)
        {
            var survey = await _surveyRepo.GetByIdAsync(id);
            if (survey == null || !survey.IsActive)
                return NotFound(new ResultDto { Status = false, Message = "Anket bulunamadı." });

            return Ok(survey);
        }

        // YÖNETİCİ PANELİ: Yeni Anket Oluşturma
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSurvey(SurveyCreateDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var survey = new Survey
            {
                Title = model.Title,
                Description = model.Description,
                EndDate = model.EndDate,
                AppUserId = userId!,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _surveyRepo.AddAsync(survey);
            await _surveyRepo.SaveAsync();

            return Ok(new ResultDto { Status = true, Message = "Anket başarıyla oluşturuldu." });
        }

        // YÖNETİCİ PANELİ: Ankete Soru Ekleme
        [Authorize(Roles = "Admin")]
        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion(QuestionDto model)
        {
            var question = new Question
            {
                Text = model.Text,
                Type = (Models.Enums.QuestionType)model.Type,
                IsRequired = model.IsRequired,
                SurveyId = model.Id, // DTO'daki Id, SurveyId olarak eşleşir
                CreatedDate = DateTime.Now
            };

            await _questionRepo.AddAsync(question);
            await _questionRepo.SaveAsync();

            return Ok(new ResultDto { Status = true, Message = "Soru başarıyla eklendi." });
        }

        // YÖNETİCİ PANELİ: Anket Silme (Soft Delete)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _surveyRepo.GetByIdAsync(id);
            if (survey == null) return NotFound();

            survey.IsActive = false; // Veri tabanından fiziksel olarak silmez, pasife çeker
            _surveyRepo.Update(survey);
            await _surveyRepo.SaveAsync();

            return Ok(new ResultDto { Status = true, Message = "Anket başarıyla silindi (pasife alındı)." });
        }
    }
}