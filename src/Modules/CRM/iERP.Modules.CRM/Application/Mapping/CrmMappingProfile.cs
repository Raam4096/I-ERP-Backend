using AutoMapper;
using iERP.Modules.CRM.Application.Leads.Dtos;
using iERP.Modules.CRM.Domain;

namespace iERP.Modules.CRM.Application.Mapping;

public sealed class CrmMappingProfile : Profile
{
    public CrmMappingProfile()
    {
        CreateMap<LeadAttachment, LeadAttachmentDto>();
        CreateMap<LeadFollowUp, LeadFollowUpDto>();
        CreateMap<Lead, LeadDto>()
            .ForMember(d => d.FollowUps, opt => opt.MapFrom(s => s.FollowUps));
    }
}
