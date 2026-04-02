using Microsoft.AspNetCore.Authorization;
<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OstaFeedbackApp.Data;
using OstaFeedbackApp.Models;
=======
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using OstaFeedbackApp.Data;
using OstaFeedbackApp.Models;
using OstaFeedbackApp.Hubs;
>>>>>>> a47f374 (Clean repo without large files)
using QRCoder;
using System;
using System.Linq;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using System.Collections.Generic;
using System.Text.Json;
>>>>>>> a47f374 (Clean repo without large files)

namespace OstaFeedbackApp.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly AppDbContext _context;
<<<<<<< HEAD

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        // =============================
        // PUBLIC: CREATE FEEDBACK
        // =============================
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

=======
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHubContext<FeedbackHub> _hub;

        public FeedbackController(
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IHubContext<FeedbackHub> hub)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _hub = hub ?? throw new ArgumentNullException(nameof(hub));
        }

        // =============================
        // CREATE (GET)
        // =============================
        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            // Seed multilingual questions (only once)
            if (!await _context.Questions.AnyAsync())
            {
                var defaultQuestions = new List<Question>
                {
                    new Question {
                        Property="Commitment",
                        TextEn="Government commitment to digital transformation",
                        TextAm="የመንግስት ዲጂታል ለውጥ ቁርጠኝነት",
                        TextOr="Kutannoo mootummaa dijitaalaa",
                        IsRequired=true, MaxRating=5, IsEnabled=true
                    },
                    new Question {
                        Property="Transparency",
                        TextEn="Transparency and efficiency improvement",
                        TextAm="ግልጽነት እና ብቃት ማሻሻል",
                        TextOr="Iftoomina fi gahumsa",
                        IsRequired=true, MaxRating=5, IsEnabled=true
                    },
                    new Question {
                        Property="Innovation",
                        TextEn="Local innovation and independence",
                        TextAm="አካባቢያዊ ፈጠራ",
                        TextOr="Kalaqa biyya keessaa",
                        IsRequired=true, MaxRating=5, IsEnabled=true
                    },
                    new Question {
                        Property="CommunityImpact",
                        TextEn="Community accessibility and impact",
                        TextAm="የማህበረሰብ ተፅዕኖ",
                        TextOr="Dhiibbaa hawaasaa",
                        IsRequired=true, MaxRating=5, IsEnabled=true
                    },
                    new Question {
                        Property="YouthOpportunity",
                        TextEn="Youth opportunities and inspiration",
                        TextAm="የወጣቶች እድሎች",
                        TextOr="Carraa dargaggootaa",
                        IsRequired=true, MaxRating=5, IsEnabled=true
                    }
                };

                _context.Questions.AddRange(defaultQuestions);
                await _context.SaveChangesAsync();
            }

            var questions = await _context.Questions
    .Where(q => q.IsEnabled)
    .OrderBy(q => q.Id)
    .ToListAsync();

            ViewBag.Questions = questions;
            return View();
        }

        // =============================
        // CREATE (POST)
        // =============================
>>>>>>> a47f374 (Clean repo without large files)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
<<<<<<< HEAD
                return View(feedback);
            }

            try
            {
                feedback.CreatedAt = DateTime.UtcNow;

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ThankYou));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error saving feedback: " + ex.InnerException?.Message);
                return View(feedback);
            }
        }

        // =============================
        // THANK YOU PAGE
        // =============================
        [AllowAnonymous]
        public IActionResult ThankYou()
        {
=======
                ViewBag.Questions = await _context.Questions
                    .Where(q => q.IsEnabled)
                    .ToListAsync();

                return View(feedback);
            }

            feedback.CreatedAt = DateTime.UtcNow;

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            var questions = await _context.Questions.ToListAsync();

            // USER RATINGS
            var userRatings = new Dictionary<string, int>();

            foreach (var q in questions)
            {
                if (string.IsNullOrWhiteSpace(q.Property)) continue;

                var prop = typeof(Feedback).GetProperty(q.Property);
                if (prop == null) continue;

                var value = prop.GetValue(feedback);
                if (value is int intValue)
                    userRatings[q.Property] = intValue;
            }

            TempData["UserRatingsJson"] = JsonSerializer.Serialize(userRatings);

            // REALTIME DATA
            var data = new Dictionary<string, object>();

            foreach (var q in questions)
            {
                if (string.IsNullOrWhiteSpace(q.Property)) continue;

                var prop = typeof(Feedback).GetProperty(q.Property);
                if (prop != null)
                    data[q.Property] = prop.GetValue(feedback) ?? 0;
            }

            data["Department"] = feedback.Department ?? "";
            data["CreatedAt"] = feedback.CreatedAt;

            await _hub.Clients.All.SendAsync("ReceiveFeedback", data);

            return RedirectToAction(nameof(ThankYou));
        }

        // =============================
        // THANK YOU
        // =============================
        [AllowAnonymous]
        public async Task<IActionResult> ThankYou()
        {
            var feedbacks = await _context.Feedbacks
                .AsNoTracking()
                .ToListAsync();

            var questions = await _context.Questions
                .Where(q => q.IsEnabled)
                .ToListAsync();

            // Total
            ViewBag.Total = feedbacks.Count;

            // =============================
            // CALCULATE AVERAGES PER QUESTION
            // =============================
            var averages = new Dictionary<string, double>();

            foreach (var q in questions)
            {
                if (string.IsNullOrWhiteSpace(q.Property)) continue;

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

            // =============================
            // OVERALL AVERAGE (ALL USERS)
            // =============================
            var allScores = new List<double>();

            foreach (var f in feedbacks)
            {
                foreach (var q in questions)
                {
                    var prop = typeof(Feedback).GetProperty(q.Property);
                    if (prop == null) continue;

                    var val = prop.GetValue(f);
                    if (val != null)
                        allScores.Add(Convert.ToDouble(val));
                }
            }

            var overallAvg = allScores.Any() ? allScores.Average() : 0;
            ViewBag.AvgOverall = overallAvg;

            // =============================
            // LAST USER AVERAGE
            // =============================
            var last = feedbacks.OrderByDescending(f => f.CreatedAt).FirstOrDefault();

            double userAvg = 0;

            if (last != null)
            {
                var userScores = new List<double>();

                foreach (var q in questions)
                {
                    var prop = typeof(Feedback).GetProperty(q.Property);
                    if (prop == null) continue;

                    var val = prop.GetValue(last);
                    if (val != null)
                        userScores.Add(Convert.ToDouble(val));
                }

                userAvg = userScores.Any() ? userScores.Average() : 0;
            }

            ViewBag.UserAvg = userAvg;

            // =============================
            // INSIGHT LOGIC
            // =============================
            string insight;

            if (userAvg >= 4.5)
                insight = "🌟 Excellent! You had a very positive experience.";
            else if (userAvg >= 3.5)
                insight = "👍 Good! Overall satisfaction is strong.";
            else if (userAvg >= 2.5)
                insight = "⚖️ متوسط / Fair – There is room for improvement.";
            else
                insight = "⚠️ Needs Improvement – Your feedback highlights key issues.";

            ViewBag.Insight = insight;

            ViewBag.Questions = questions;

>>>>>>> a47f374 (Clean repo without large files)
            return View();
        }

        // =============================
<<<<<<< HEAD
        // 🔐 ADMIN DASHBOARD
=======
        // DASHBOARD
>>>>>>> a47f374 (Clean repo without large files)
        // =============================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
<<<<<<< HEAD
            var feedbacks = await _context.Feedbacks.ToListAsync();

            if (feedbacks.Count == 0)
                return View(feedbacks);

            ViewBag.AvgCommitment = feedbacks.Average(f => f.Commitment);
            ViewBag.AvgTransparency = feedbacks.Average(f => f.Transparency);
            ViewBag.AvgInnovation = feedbacks.Average(f => f.Innovation);
            ViewBag.AvgCommunityImpact = feedbacks.Average(f => f.CommunityImpact);
            ViewBag.AvgYouthOpportunity = feedbacks.Average(f => f.YouthOpportunity);

            return View(feedbacks);
        }

        // =============================
        // QR CODE GENERATION
        // =============================
        [AllowAnonymous]
        public IActionResult GenerateQR()
        {
            // 👉 Replace with your real IP or ngrok URL
            string feedbackUrl = "http://192.168.1.6:7222/Feedback/Create";

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(feedbackUrl, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new BitmapByteQRCode(qrData);

                byte[] qrCodeImage = qrCode.GetGraphic(20);

                return File(qrCodeImage, "image/png");
            }
        }

        // =============================
        // QR PAGE
        // =============================
        [AllowAnonymous]
        public IActionResult QRPage()
        {
            return View();
        }
=======
            var feedbacks = await _context.Feedbacks
                .AsNoTracking()
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            ViewBag.Questions = await _context.Questions.ToListAsync();

            return View(feedbacks ?? new List<Feedback>());
        }

        // =============================
        // ADMIN: EDIT QUESTION
        // =============================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditQuestion(Question model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Dashboard));

            if (model.Id == 0)
            {
                _context.Questions.Add(model);
            }
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
        // TOGGLE
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
        // DELETE
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
        // QR
        // =============================

        // GET: /Feedback/QRPage
        public IActionResult QRPage()
        {
            // You can pass dynamic visitor types if needed via ViewBag
            ViewBag.VisitorTypes = new List<string> { "VIP", "General" };
            return View();
        }

        // This generates the QR image for a visitor type
        public IActionResult GenerateQR(string visitorType)
        {
            // Example: generate QR code URL
            // Use QRCoder or your preferred library
            using (var qrGenerator = new QRCoder.QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(
                    Url.Action("Create", "Feedback", new { visitorType }, Request.Scheme),
                    QRCoder.QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCoder.PngByteQRCode(qrData);
                var qrBytes = qrCode.GetGraphic(20);

                return File(qrBytes, "image/png");
            }
        }
>>>>>>> a47f374 (Clean repo without large files)
    }
}