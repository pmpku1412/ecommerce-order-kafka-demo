namespace TQM.Backoffice.Application.Commands.Example;

public class ExampleCommand : IRequest<BaseResponse<string>>
{
    public string Request { get; set; } = string.Empty;

    public class ExampleCommandHandler : IRequestHandler<ExampleCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(ExampleCommand command, CancellationToken cancellationToken)
        {
            BaseResponse<string> response = new();
            response.Message = "Success";
            return await Task.FromResult(response);
        }
    }
}
