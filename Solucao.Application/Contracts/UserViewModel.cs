using System;
using System.Threading.Tasks;

namespace Solucao.Application.Contracts
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string NickName { get; set; }
        public string Token { get; set; }
        public DateTime? Token_Expire { get; set; }

        public static explicit operator Task<object>(UserViewModel v)
        {
            throw new NotImplementedException();
        }
    }
}
