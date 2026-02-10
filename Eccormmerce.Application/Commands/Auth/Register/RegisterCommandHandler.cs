using Eccommerce.Domain.Entities;
using Eccormmerce.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccormmerce.Application.Commands.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }   

        public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("User already exists");

            var passwordHash = _passwordHasher.Hash(request.Password);

            //var allowedRoles = new[] { "Customer" };

            var role = string.IsNullOrWhiteSpace(request.Role) ? "Customer" : request.Role;

            var user = new User(
                    request.Email,
                    passwordHash,
                    role
                );
            await _userRepository.AddAsync(user);
            return user.Id;
        }
    }
}
