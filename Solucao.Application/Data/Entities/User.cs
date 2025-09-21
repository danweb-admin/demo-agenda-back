using System;

namespace Solucao.Application.Data.Entities
{
    public class User: BaseEntity
    {
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public DateTime? Token_Expire { get; set; }

    }
}