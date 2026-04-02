using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using OstaFeedbackApp.Data;
using OstaFeedbackApp.Models;
using OstaFeedbackApp.Hubs;
using QRCoder;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace OstaFeedbackApp.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHubContext<FeedbackHub> _hub;

        public FeedbackController(
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IHubContext<FeedbackHub> hub)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _hub = hub;
        }

        // =============================
        // CREATE (GET)
        // =============================
        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            if (!await _context.Questions.AnyAsync())
            {
                var defaultQuestions = new List<Question>
                {
                    new Question { Property="Commitment", TextEn="Government commitment to digital transformation", IsRequired=true, MaxRating=5, IsEnabled=true },
                    new Question { Property="Transparency", TextEn="Transparency and efficiency improvement", IsRequired=true, MaxRating=5, IsEnabled=true },
                    new Question { Property="Innovation", TextEn="Local innovation and independence", IsRequired=true, MaxRating=5, IsEnabled=true },
                    new Question { Property="CommunityImpact", TextEn="Community accessibility and impact", IsRequired=true, MaxRating=5, IsEnabled=true },
                    new Question { Property="YouthOpportunity", TextEn="Youth opportunities and inspiration", IsRequired=true, MaxRating=5, IsEnabled=true }
                };

                _context.Questions.AddRange(defaultQuestions);
                await _context.SaveChangesAsync();
            }

            ViewBag.Questions = await _context.Questions
                .Where(q => q.IsEnabled)
                .OrderBy(q => q.Id)
                .ToListAsync();

            return View();
        }

        // =============================
        // CREATE (POST)
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Questions = await _context.Questions
                    .Where(q => q.IsEnabled)
                    .ToListAsync();

                return View(feedback);
            }

            feedback.CreatedAt = DateTime.UtcNow;

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Realtime SignalR
            var questions = await _context.Questions.ToListAsync();
            var data = new Dictionary<string, object>();

            foreach (var q in questions)
            {
                if (string.IsNullOrWhiteSpace(q.Property)) continue;

                var prop = typeof(Feedback).GetProperty(q.Property);
                if (prop != null)
                    data[q.Property] = prop.GetValue(feedback) ?? 0;
            }

            data["CreatedAt"] = feedback.CreatedAt;

            await _hub.Clients.All.SendAsync("ReceiveFeedback", data);

            return RedirectToAction(nameof(ThankYou));
        }

        // =============================
        // THANK YOU PAGE
        // =============================
        [AllowAnonymous]
        public async Task<IActionResult> ThankYou()
        {
            var feedbacks = await _context.Feedbacks.AsNoTracking().ToListAsync();
            var questions = await _context.Questions.Where(q => q.IsEnabled).ToListAsync();

            ViewBag.Total = feedbacks.Count;

            var averages = new Dictionary<string, double>();

            foreach (var q in questions)
            {
                var prop = typeof(Feedback).GetProperty(q.Property);
                if (prop == null) continue;

                var values = feedbacks
                    .Select(f => prop.GetValue(f))
                    .Where(v => v != null)
                    .Select(v => Convert.ToDouble(v))
                    .ToList();

                averages[q.Property] = values.Any() ? values.Average() : 0;
            }

            ViewBag.Averages = averages;
            ViewBag.Questions = questions;

            return View();
        }

        // =============================
        // DASHBOARD (ADMIN)
        // =============================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            var feedbacks = await _context.Feedbacks
                .AsNoTracking()
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            ViewBag.Questions = await _context.Questions.ToListAsync();

            return View(feedbacks);
        }

        // =============================
        // EDIT QUESTION
        // =============================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditQuestion(Question model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Dashboard));

            if (model.Id == 0)
                _context.Questions.Add(model);
            else
            {
                var existing = await _context.Questions.FindAsync(model.Id);
                if (existing == null) return NotFound();

                existing.TextEn = model.TextEn;
                existing.TextAm = model.TextAm;
                existing.TextOr = model.TextOr;
                existing.IsRequired = model.IsRequired;
                existing.MaxRating = model.MaxRating;
                existing.IsEnabled = model.IsEnabled;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        // =============================
        // TOGGLE QUESTION
        // =============================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleQuestion(int id)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q == null) return NotFound();

            q.IsEnabled = !q.IsEnabled;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        // =============================
        // DELETE QUESTION
        // =============================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q == null) return NotFound();

            _context.Questions.Remove(q);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        // =============================
        // QR PAGE
        // =============================
        [AllowAnonymous]
        public IActionResult QRPage()
        {
            ViewBag.VisitorTypes = new List<string> { "VIP", "General" };
            return View();
        }

        // =============================
        // GENERATE QR
        // =============================
        [AllowAnonymous]
        public IActionResult GenerateQR(string visitorType)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var url = Url.Action("Create", "Feedback", new { visitorType }, Request.Scheme);

                var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrData);
                var qrBytes = qrCode.GetGraphic(20);

                return File(qrBytes, "image/png");
            }
        }
    }
}
