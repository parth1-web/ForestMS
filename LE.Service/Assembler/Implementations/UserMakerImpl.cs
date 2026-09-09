using LE.Infrastructure.Dto;
using LE.Service.Assembler.Interface;

namespace LE.Service.Assembler.Implementations
{
    public class UserMakerImpl : UserMaker
    {
        public void copy(Entities.User.User user, UserDto user_dto)
        {
            user.user_id = user_dto.user_id;

            user.full_name = user_dto.full_name;
            user.address_line_1 = user_dto.address_line_1;
            user.address_line_2 = user_dto.address_line_2;
            user.primary_contact = user_dto.primary_contact;
            user.secondary_contact = user_dto.secondary_contact;
            user.email = user_dto.email;
            user.created_by = user_dto.created_by;
            user.is_active = user_dto.is_active;
            user.image_path = user_dto.image_path;
        }
    }
}
