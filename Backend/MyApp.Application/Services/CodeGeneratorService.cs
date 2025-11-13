using MyApp.Application.Interfaces.Services;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using System.Security.Cryptography;

namespace MyApp.Application.Services
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly IGenericRepository<UserPasswordResetsEntity> _userPasswordResetRepository;
        private readonly IGenericRepository<UserVerificationsEntity> _repository;

        public CodeGeneratorService(IGenericRepository<UserVerificationsEntity> repository, IGenericRepository<UserPasswordResetsEntity> userPasswordResetRepository)
        {
            _repository = repository;
            _userPasswordResetRepository = userPasswordResetRepository;
        }

        public async Task<string> GenerateUniqueCode()
        {
            string code;
            UserVerificationsEntity? exists;

            do
            {
                var bytes = RandomNumberGenerator.GetBytes(4);
                int number = BitConverter.ToInt32(bytes, 0) % 90000 + 10000;
                code = Math.Abs(number).ToString("D5");
                exists = await _repository.GetByCondition(x => x.CodeValidation == code);
            }
            while (exists is not null);

            return code;
        }

        public async Task<string> GenerateCodeByResetPassword()
        {
            string code;
            UserPasswordResetsEntity? exists;

            do
            {
                var bytes = RandomNumberGenerator.GetBytes(4);
                int number = BitConverter.ToInt32(bytes, 0) % 90000 + 10000;
                code = Math.Abs(number).ToString("D5");
                exists = await _userPasswordResetRepository.GetByCondition(x => x.ResetPasswordCode == code);
            }
            while (exists is not null);

            return code;
        }
    }
}
