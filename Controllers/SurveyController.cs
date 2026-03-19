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
        private readonly IGenericRepository<QuestionOption> _optionRepo; // Şıklar için eklendi

        public SurveyController(
            IGenericRepository<Survey> surveyRepo,
            IGenericRepository<Question> questionRepo,
            IGenericRepository<QuestionOption> optionRepo)
        {
            _surveyRepo = surveyRepo;
            _questionRepo = questionRepo;
            _optionRepo = optionRepo;
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
                Type = (Models.Enums.QuestionType)model.Type, // 1=Yazılı, 2=Çoktan Seçmeli
                IsRequired = model.IsRequired,
                SurveyId = model.Id, // DTO'daki Id, SurveyId olarak eşleşir
                CreatedDate = DateTime.Now
            };

            await _questionRepo.AddAsync(question);
            await _questionRepo.SaveAsync();

            return Ok(new ResultDto { Status = true, Message = "Soru başarıyla eklendi." });
        }

        // YÖNETİCİ PANELİ: Soruya Şık (Seçenek) Ekleme (YENİ)
        [Authorize(Roles = "Admin")]
        [HttpPost("AddOption")]
        public async Task<IActionResult> AddOption(OptionCreateDto model)
        {
            var option = new QuestionOption
            {
                OptionText = model.OptionText,
                Order = model.Order,
                QuestionId = model.QuestionId,
                CreatedDate = DateTime.Now
            };

            await _optionRepo.AddAsync(option);
            await _optionRepo.SaveAsync();

            return Ok(new ResultDto { Status = true, Message = "Şık başarıyla eklendi." });
        }

        // YÖNETİCİ PANELİ: Anket Sonuçları (YENİ)
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/Results")]
        public async Task<IActionResult> GetSurveyResults(int id)
        {
            // İleride Final projesinde burada istatistiksel hesaplamalar yapılacak
            // Şimdilik sistemin uçtan uca çalışıp çalışmadığını gösteren bir mesaj dönüyoruz.
            return Ok(new ResultDto { Status = true, Message = $"{id} numaralı anketin sonuçları derleniyor. (İstatistikler Final projesinde aktif edilecek)" });
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