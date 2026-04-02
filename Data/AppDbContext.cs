using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OstaFeedbackApp.Models;

namespace OstaFeedbackApp.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Feedback> Feedbacks { get; set; }
<<<<<<< HEAD
=======
        public DbSet<Question> Questions { get; set; }
>>>>>>> a47f374 (Clean repo without large files)
    }
}