using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlowCare.ViewModels.Reviews
{
    public class AddReviewViewModel
    {
        public int Rating { get; set; }

        public string Comment { get; set; } = null!;

        public string PublisherId { get; set; } = null!;

        public int? ProcedureId { get; set; }

        public Guid EmployeeId { get; set; }
    }
}
