using GlowCare.Core.Contracts;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static GlowCare.Common.Constants.ProcedureConstants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GlowCare.Core.Implementations;

public class ProcedureService(
    IRepository<Procedure, int> procedureRepository,
    UserManager<GlowUser> userManager) 
    : IProcedureService
{
    public async Task CreateProcedureAsync(
        ProcedureAddViewModel model, 
        string clientId)
    {
        if (model == null)
        {
            throw new NullReferenceException("Entity not found.");
        }

        Procedure procedure = new()
        {
            ClientId = clientId,
            EmployeeId = model.EmployeeId,
            ServiceId = model.ServiceId,
            AppointmentDate = model.AppointmentDate,
            Status = Entities.Models.Enums.Status.Scheduled,
            Notes = model.Notes,
        };

        await procedureRepository.AddAsync(procedure);
    }

    public async Task DeleteProcedureAsync(
        ProcedureDeleteViewModel model)
    {
        Procedure procedure = await procedureRepository.GetByIdAsync(model.Id);

        if (procedure == null)
        {
            throw new NullReferenceException("Entity not found.");
        }

        if (procedure.IsDeleted)
        {
            throw new ArgumentException("Entity is already deleted.");
        }

        if (!procedure.IsDeleted || procedure != null)
        {
            procedure.IsDeleted = true;

            await procedureRepository.UpdateAsync(procedure);
        }
    }

    public async Task<Procedure> EditProcedureAsync(
        ProcedureEditViewModel model, 
        int id)
    {
        Procedure procedure = await procedureRepository.GetByIdAsync(id) 
            ?? throw new NullReferenceException("Entity not found.");

        if (procedure.IsDeleted)
        {
            throw new ArgumentException("Entity is already deleted.");
        }

        procedure.EmployeeId = model.EmployeeId;
        procedure.ServiceId = model.ServiceId;
        procedure.AppointmentDate = model.AppointmentDate;
        procedure.Notes = model.Notes;

        await procedureRepository.UpdateAsync(procedure);

        return procedure;
    }

    public async Task<IEnumerable<ProcedureDetailsViewModel>> GetAllProcedureDetailsByClientIdAsync(
        string clientId)
    {
        var client = await userManager.FindByIdAsync(clientId);

        if (client == null)
        {
            throw new NullReferenceException("User not found");
        }


        var procedures = await procedureRepository
                .GetAllAttached()
                .Include(p => p.Employee)
                .ThenInclude(e => e!.User)
                .Include(p => p.Service)
                .Where(p => p.IsDeleted == false && p.ClientId == clientId)
                .Select(p => new ProcedureDetailsViewModel
                {
                    Id = p.Id,
                    SpecialistName = $"{p.Employee!.User.FirstName} {p.Employee!.User.LastName}",
                    Service = p.Service!.Name,
                    Price = p.Service.Price,
                    AppointmentDate = p.AppointmentDate.ToString(AppointmentDateFormat)
                })
                .ToListAsync();

        return procedures;
    }



    public async Task<ProcedureDeleteViewModel> GetDeleteProcedureAsync(
        int id, 
        string clientId)
    {
        List<Procedure> entities = await procedureRepository.GetAllAttached()
            .Include(p => p.Service)
            .Include(p => p.Client)
            .ThenInclude(c => c!.User)
            .ToListAsync();

        Procedure entity = entities
            .FirstOrDefault(e => e.Id == id) 
            ?? throw new NullReferenceException("Entity not found.");

        if (entity.IsDeleted)
        {
            throw new NullReferenceException("Entity is already deleted.");
        }

        if (entity.ClientId == null || entity.ClientId != clientId)
        {
            throw new NullReferenceException("Invalid client id.");
        }

        Client? client = entity.Client;

        Service service = entity.Service ?? throw new NullReferenceException("Service not found.");



        var procedure = new ProcedureDeleteViewModel
        {
            Id = entity.Id,
            ClientName = $"{client!.User.FirstName} {client!.User.LastName}",
            ServiceName = service.Name,
        };

        return procedure;
    }

    public async Task<ProcedureEditViewModel> GetEditProcedureAsync(
        int id)
    {
        var entity = await procedureRepository.GetByIdAsync(id) 
            ?? throw new NullReferenceException("Entity not found");

        if (entity.IsDeleted)
        {
            throw new ArgumentException("Entity already deleted");
        }

        var procedure = new ProcedureEditViewModel()
        {
            EmployeeId = entity.EmployeeId,
            ServiceId = entity.ServiceId,
            AppointmentDate = entity.AppointmentDate,
            Notes = entity.Notes,
        };

        return procedure;
    }

}

