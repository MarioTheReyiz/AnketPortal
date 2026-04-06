using AnketPortal.API.DTOs;
using AnketPortal.API.Models;
using AnketPortal.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace AnketPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Cevap vermek için giriş yapmış olmak şart
    public class AnswerController : ControllerBase
    {
        private readonly IGenericRepository<SurveyAnswer> _answerRepo;
        private readonly IGenericRepository<Survey> _surveyRepo; // EKSİK 1: Anket kontrolü için eklendi

        public AnswerController(IGenericRepository<SurveyAnswer> answerRepo, IGenericRepository<Survey> surveyRepo)
        {
            _answerRepo = answerRepo;
            _surveyRepo = surveyRepo;
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitAnswers(AnswerSubmitDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // GÜVENLİK 1: Boş liste gönderilmiş mi?
            if (model.Answers == null || !model.Answers.Any())
            {
                return BadRequest(new ResultDto { Status = false, Message = "Lütfen en az bir soruya cevap verin." });
            }

            // GÜVENLİK 2: Anket gerçekten var mı, yayında mı ve süresi devam ediyor mu?
            var survey = await _surveyRepo.GetByIdAsync(model.SurveyId);

            if (survey == null || !survey.IsActive)
                return NotFound(new ResultDto { Status = false, Message = "Bu anket bulunamadı veya yayından kaldırılmış." });

            if (survey.EndDate < DateTime.Now)
                return BadRequest(new ResultDto { Status = false, Message = "Bu anketin katılım süresi dolmuştur, cevap gönderemezsiniz." });

            // GÜVENLİK 3: Kullanıcı bu anketi daha önce çözmüş mü?
            bool hasAnswered = await _answerRepo.AsQueryable()
                .AnyAsync(a => a.AppUserId == userId && a.SurveyId == model.SurveyId);

            if (hasAnswered)
            {
                return BadRequest(new ResultDto { Status = false, Message = "Bu anketi zaten cevapladınız. Bir ankete sadece bir kez katılabilirsiniz." });
            }

            // KAYIT: Tüm güvenlik duvarları aşıldıysa cevapları veritabanına işle
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
            return Ok(new ResultDto { Status = true, Message = "Cevaplarınız başarıyla kaydedildi. Katılımınız için teşekkürler!" });
        }

        // EKSİK METOT: Kullanıcının Kendi Cevaplarını Görmesi
        [HttpGet("MyAnswers/{surveyId}")]
        public async Task<IActionResult> GetMyAnswers(int surveyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myAnswers = await _answerRepo.AsQueryable()
                .Where(a => a.AppUserId == userId && a.SurveyId == surveyId)
                .Select(a => new
                {
                    a.QuestionId,
                    a.SelectedOptionId,
                    a.TextAnswer,
                    a.CreatedDate
                }).ToListAsync();

            if (!myAnswers.Any())
                return NotFound(new ResultDto { Status = false, Message = "Bu ankete ait bir cevabınız bulunmamaktadır." });

            return Ok(new ResultDto { Status = true, Message = "Cevaplarınız başarıyla getirildi.", Data = myAnswers });
        }
    }
}