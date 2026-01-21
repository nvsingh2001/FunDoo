using BusinessLogicLayer.Mapping;

namespace FunDooApp.Extensions;

public static class MapperExtension
{
   public static void AddAutoMapperProfile(this IServiceCollection services)
   {
      services.AddAutoMapper(typeof(UserProfile));

      services.AddAutoMapper(typeof(NoteProfile));

      services.AddAutoMapper(typeof(LabelProfile));

      services.AddAutoMapper(typeof(CollaboratorProfile));
   }
}