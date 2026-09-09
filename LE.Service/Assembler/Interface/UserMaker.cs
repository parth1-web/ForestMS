namespace LE.Service.Assembler.Interface
{
    using LE.Infrastructure.Dto;
    using userEntity = Entities.User.User;
    public interface UserMaker
    {
        void copy(userEntity user, UserDto user_dto);
    }
}
