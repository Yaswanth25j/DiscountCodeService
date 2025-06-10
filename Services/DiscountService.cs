using Grpc.Core;
using DiscountCodeService.Protos;

namespace DiscountCodeService.Services
{
    public class DiscountService : Discount.DiscountBase
    {
        private readonly CodeRepository _repository;

        public DiscountService(CodeRepository repository)
        {
            _repository = repository;
        }

        public override Task<GenerateResponse> Generate(GenerateRequest request, ServerCallContext context)
        {
            bool success = _repository.AddCodes((int)request.Count, (int)request.Length);
            return Task.FromResult(new GenerateResponse { Result = success });
        }

        public override Task<UseCodeResponse> UseCode(UseCodeRequest request, ServerCallContext context)
        {
            byte result = _repository.UseCode(request.Code);
            return Task.FromResult(new UseCodeResponse { Result = result });
        }
    }
}