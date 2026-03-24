using AI.Assistant.Core.Models;
using AI.Assistant.Infrastructure.Persistence.Models;
using AutoMapper;

namespace AI.Assistant.Infrastructure.Persistence.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MessageDto, Message>()
            .ForMember(dest => dest.Embedding,
                opt => opt.MapFrom(src => src.Embedding != null ? src.Embedding.ToArray() : null));

        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.Embedding,
                opt => opt.MapFrom(src => src.Embedding != null ? src.Embedding.ToList() : null));

        CreateMap<ContextDto, Context>().ReverseMap();
        CreateMap<ReminderDto, Reminder>().ReverseMap();
    }
}