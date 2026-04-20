using System.ComponentModel.DataAnnotations;
using static GlowCare.Common.Constants.ReviewConstants;

namespace GlowCare.ViewModels.Reviews
{
    public class AddReviewViewModel
    {
        [Range(MinRating, MaxRating, ErrorMessage = "Оценката трябва да е между 1 и 5 звезди.")]
        [Required(ErrorMessage = "Моля, избери оценка.")]
        public int Rating { get; set; }

        [StringLength(CommentMaxLength, MinimumLength = CommentMinLength, ErrorMessage = "Коментарът трябва да бъде между 10 и 500 символа.")]
        [Required(ErrorMessage = "Моля, напиши коментар.")]
        public string Comment { get; set; } = null!;

        [Required(ErrorMessage = "Моля, избери процедура.")]
        public int? ProcedureId { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }

        public int ServiceId { get; set; }

        public string SpecialistName { get; set; } = string.Empty;

        public IEnumerable<ProcedureOptionViewModel> AvailableProcedures { get; set; }
            = new List<ProcedureOptionViewModel>();
    }
}