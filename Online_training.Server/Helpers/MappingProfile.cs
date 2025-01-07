using static System.Net.Mime.MediaTypeNames;
using Stripe;
using Online_training.Server.Models.DTOs;
using Online_training.Server.Models;
using AutoMapper;

namespace Online_training.Server.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {

            CreateMap<PanierItem, PanierItemDTO>()
                         .ForMember(b => b.CourseId, o => o.MapFrom(c => c.Formation!.Id))
                         .ForMember(b => b.Title, o => o.MapFrom(c => c.Formation!.Title))
                         .ForMember(b => b.Price, o => o.MapFrom(c => c.Formation!.Price))
                         .ForMember(b => b.Image, o => o.MapFrom(c => c.Formation!.ImageFormation));

            CreateMap<Panier, PanierDTO>();


        }
    }
}
