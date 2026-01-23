namespace GlowCare.ViewModels.Procedures;

public class DetailsProcedureViewModel
{
    public required int Id { get; set; }

    public required string SpecialistName { get; set; }


    public required string Service { get; set; }

    public required decimal Price { get; set; }

    public required string AppointmentDate { get; set; }
}

