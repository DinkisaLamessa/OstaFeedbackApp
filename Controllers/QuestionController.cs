using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OstaFeedbackApp.Data;
using OstaFeedbackApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OstaFeedbackApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuestionController : Controller
    {
        private readonly AppDbContext _context;

        public QuestionController(AppDbContext context)
        {
            _context = context;
        }

        // =============================
        // LIST ALL QUESTIONS
        // =============================
        public async Task<IActionResult> Index()
        {
            var questions = await _context.Questions
                .OrderBy(q => q.Id)
                .ToListAsync();

            return View(questions);
        }

        // =============================
        // CREATE QUESTION
        // =============================
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question)
        {
            if (!ModelState.IsValid) return View(question);

            question.IsEnabled = true; // default to enabled
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =============================
        // EDIT QUESTION
        // =============================
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Question question)
        {
            if (id != question.Id) return BadRequest();
            if (!ModelState.IsValid) return View(question);

            try
            {
                _context.Update(question);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(question.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =============================
        // ENABLE / DISABLE QUESTION
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            question.IsEnabled = !question.IsEnabled;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =============================
        // DELETE QUESTION
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =============================
        // HELPER
        // =============================
        private bool QuestionExists(int id) => _context.Questions.Any(q => q.Id == id);
    }
}