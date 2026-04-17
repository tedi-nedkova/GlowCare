using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlowCare.ViewModels.SpecialistRequest;

public class ReviewApplicationViewModel
{
    public int Id { get; set; }

    public string? RejectionReason { get; set; }
}
