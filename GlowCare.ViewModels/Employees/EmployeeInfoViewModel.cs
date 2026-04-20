using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlowCare.ViewModels.Employees
{
    public class EmployeeInfoViewModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Occupation { get; set; } = null!;

        public int ExperienceYears { get; set; }

        public string? Biography { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }

        public ICollection<string> Services { get; set; } = new List<string>();
        public ICollection<EmployeeScheduleViewModel> WorkingHours { get; set; }
    = new List<EmployeeScheduleViewModel>();
    }
}
