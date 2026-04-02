using System;
<<<<<<< HEAD
=======
using System.ComponentModel.DataAnnotations;

>>>>>>> a47f374 (Clean repo without large files)

namespace OstaFeedbackApp.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        // Section 1 (Required ratings)
        public int Commitment { get; set; }
        public int Transparency { get; set; }
        public int Innovation { get; set; }
        public int CommunityImpact { get; set; }
        public int YouthOpportunity { get; set; }

        // Section 2 (Optional text)
        public string? Impressed { get; set; }
        public string? Improvement { get; set; }
        public string? DigitalService { get; set; }
        public string? YouthSupport { get; set; }

        // Section 3 (Optional info)
        public string? AgeGroup { get; set; }
        public string? Occupation { get; set; }
        public string? Region { get; set; }

        // Optional (only if used)
        public string? VisitorType { get; set; }
        public string? Department { get; set; }
        //public DateTime Date { get; set; } // or FeedbackDate if you have it

        public DateTime CreatedAt { get; set; }
    }
<<<<<<< HEAD
}
=======
    public class Question
    {
        public int Id { get; set; }

        public string Property { get; set; } = string.Empty;

        // 🌍 MULTI-LANGUAGE ONLY
        public string TextEn { get; set; } = string.Empty;
        public string TextAm { get; set; } = string.Empty;
        public string TextOr { get; set; } = string.Empty;

        public bool IsRequired { get; set; }
        public int MaxRating { get; set; } = 5;

        public bool IsEnabled { get; set; } = true;
    }
}

>>>>>>> a47f374 (Clean repo without large files)
