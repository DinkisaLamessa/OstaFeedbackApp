using System;
using System.ComponentModel.DataAnnotations;

namespace OstaFeedbackApp.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        // =============================
        // SECTION 1: Ratings (Required)
        // =============================
        [Range(1, 5)]
        public int Commitment { get; set; }

        [Range(1, 5)]
        public int Transparency { get; set; }

        [Range(1, 5)]
        public int Innovation { get; set; }

        [Range(1, 5)]
        public int CommunityImpact { get; set; }

        [Range(1, 5)]
        public int YouthOpportunity { get; set; }

        // =============================
        // SECTION 2: Optional Text
        // =============================
        public string? Impressed { get; set; }
        public string? Improvement { get; set; }
        public string? DigitalService { get; set; }
        public string? YouthSupport { get; set; }

        // =============================
        // SECTION 3: Optional Info
        // =============================
        public string? AgeGroup { get; set; }
        public string? Occupation { get; set; }
        public string? Region { get; set; }

        // =============================
        // EXTRA
        // =============================
        public string? VisitorType { get; set; }
        public string? Department { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    // =============================
    // DYNAMIC QUESTIONS MODEL
    // =============================
    public class Question
    {
        public int Id { get; set; }

        // Maps to Feedback property (e.g. "Commitment")
        [Required]
        public string Property { get; set; } = string.Empty;

        // 🌍 Multi-language support
        [Required]
        public string TextEn { get; set; } = string.Empty;

        [Required]
        public string TextAm { get; set; } = string.Empty;

        [Required]
        public string TextOr { get; set; } = string.Empty;

        public bool IsRequired { get; set; }

        [Range(1, 10)]
        public int MaxRating { get; set; } = 5;

        public bool IsEnabled { get; set; } = true;
    }
}
