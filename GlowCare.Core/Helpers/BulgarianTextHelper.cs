using GlowCare.Entities.Models.Enums;

namespace GlowCare.Core.Helpers;

public static class BulgarianTextHelper
{
    public static string GetGenderText(Gender gender)
        => gender switch
        {
            Gender.Male => "Мъж",
            Gender.Female => "Жена",
            Gender.Other => "Друго",
            _ => gender.ToString()
        };

    public static string GetMembershipTitleText(MembershipTitle title)
        => title switch
        {
            MembershipTitle.Welcome => "Добре дошъл",
            MembershipTitle.GlowEntry => "GlowEntry",
            MembershipTitle.GlowPlus => "GlowPlus",
            MembershipTitle.GlowElite => "GlowElite",
            _ => title.ToString()
        };

    public static string GetProcedureStatusText(Status status, CancelledBy? cancelledBy = null)
        => status switch
        {
            Status.Scheduled => "Планирана",
            Status.Completed => "Завършена",
            Status.Cancelled when cancelledBy == CancelledBy.User => "Отказана от клиент",
            Status.Cancelled when cancelledBy == CancelledBy.Employee => "Отказана от специалист",
            Status.Cancelled => "Отказана",
            _ => status.ToString()
        };

    public static string GetRequestStatusText(RequestStatus status)
        => status switch
        {
            RequestStatus.Accepted => "Одобрена",
            RequestStatus.Declined => "Отхвърлена",
            RequestStatus.Pending => "Изчакваща",
            RequestStatus.Revoked => "Отнета",
            _ => status.ToString()
        };
}
