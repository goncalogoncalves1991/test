using AutoMapper;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class MapperConfig
    {
        
        
       /* public static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<@event, EventDetails>();
                cfg.CreateMap<community, CommunityDetails>()
                    .ForMember(d => d.members, opt => opt.MapFrom(src =>src.userInfo1))
                    .ForMember(d => d.admins, opt => opt.MapFrom(src => src.userInfo));
                cfg.CreateMap<session, SessionDetails>()
                    .ForMember(d => d.SpeakerName, opt => opt.MapFrom(src => src.speakerName))
                    .ForMember(d => d.SpeakerLastName, opt => opt.MapFrom(src => src.lastName))
                    .ForMember(d => d.SpeakerProfile, opt => opt.MapFrom(src => src.linkOfSpeaker));
                cfg.CreateMap<tag, TagDetails>();
                cfg.CreateMap<eventSubscribers, EventDetails.Subscribers>();
                cfg.CreateMap<userInfo, UserDetails>()
                    .ForMember(d => d.eventsOfUser, opt => opt.MapFrom(src => src.eventSubscribers))
                    .ForMember(d => d.admin, opt => opt.MapFrom(src => src.community))
                    .ForMember(d => d.member, opt => opt.MapFrom(src => src.community1));
                cfg.CreateMap<eventSubscribers, EventProperties>()
                    .ForMember(d => d.id, opt=>opt.MapFrom(src => src.@event.id))
                    .ForMember(d => d.title, opt => opt.MapFrom(src => src.@event.title))
                    .ForMember(d => d.description, opt => opt.MapFrom(src => src.@event.description))
                    .ForMember(d => d.initDate, opt => opt.MapFrom(src => src.@event.initDate))
                    .ForMember(d => d.endDate, opt => opt.MapFrom(src => src.@event.endDate))
                    .ForMember(d => d.local, opt => opt.MapFrom(src => src.@event.local))
                    .ForMember(d => d.nrOfTickets, opt => opt.MapFrom(src => src.@event.nrOfTickets));
                cfg.CreateMap<eventSubscribers, EventDetails>()
                    .ForMember(d => d.id, opt => opt.MapFrom(src => src.@event.id))
                    .ForMember(d => d.title, opt => opt.MapFrom(src => src.@event.title))
                    .ForMember(d => d.description, opt => opt.MapFrom(src => src.@event.description))
                    .ForMember(d => d.initDate, opt => opt.MapFrom(src => src.@event.initDate))
                    .ForMember(d => d.endDate, opt => opt.MapFrom(src => src.@event.endDate))
                    .ForMember(d => d.local, opt => opt.MapFrom(src => src.@event.local))
                    .ForMember(d => d.community, opt => opt.MapFrom(src => src.@event.community))
                    .ForMember(d => d.session, opt => opt.MapFrom(src => src.@event.session))
                    .ForMember(d => d.tag, opt => opt.MapFrom(src => src.@event.tag))
                    .ForMember(d => d.nrOfTickets, opt => opt.MapFrom(src => src.@event.nrOfTickets))
                    .ForMember(d => d.eventSubscribers, opt => opt.MapFrom(src => src.@event.eventSubscribers))
                    .ForMember(d => d.commentEvent, opt => opt.MapFrom(src => src.@event.commentEvent));
                cfg.CreateMap<notice, NoticeDetails>()
                    .ForMember(d => d.date, opt => opt.MapFrom(src => src.initialDate));
                cfg.CreateMap<commentCommunity, CommentDetails>()
                    .ForMember(d => d.date, opt => opt.MapFrom(src => src.initialDate));
                cfg.CreateMap<commentEvent, CommentDetails>()
                    .ForMember(d => d.date, opt => opt.MapFrom(src => src.initialDate));
                cfg.CreateMap<question, QuestionDetails>();
                cfg.CreateMap<userInfo, UserProperties>();
                cfg.CreateMap<@event, EventProperties>();     
                cfg.CreateMap<community, CommunityProperties>();
                cfg.CreateMap<notice, NoticeProperties>()
                    .ForMember(d => d.date, opt => opt.MapFrom(src => src.initialDate));
                cfg.CreateMap<session, SessionProperties>()
                    .ForMember(d => d.SpeakerName, opt => opt.MapFrom(src => src.speakerName))
                    .ForMember(d => d.SpeakerLastName, opt => opt.MapFrom(src => src.lastName))
                    .ForMember(d => d.SpeakerProfile, opt => opt.MapFrom(src => src.linkOfSpeaker));
                cfg.CreateMap<commentCommunity,CommentProperties>()
                    .ForMember(d => d.date, opt => opt.MapFrom(src => src.initialDate));
                cfg.CreateMap<commentEvent,CommentProperties>()
                    .ForMember(d => d.date, opt => opt.MapFrom(src => src.initialDate));
                cfg.CreateMap<tag, TagProperties>();
                cfg.CreateMap<question, QuestionProperties>();
                    

            });
             return config.CreateMapper();
        }*/
    }
}
