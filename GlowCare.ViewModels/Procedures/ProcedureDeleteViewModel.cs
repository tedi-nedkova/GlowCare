namespace GlowCare.ViewModels.Procedures;

public class ProcedureDeleteViewModel
{
    public int Id { get; set; }
    public required string ClientName { get; set; }

    public required string ServiceName { get; set; }
}