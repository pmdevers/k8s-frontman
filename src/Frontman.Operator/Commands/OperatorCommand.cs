using Frontman.Operator.Builder;
using Microsoft.Extensions.Hosting;

namespace Frontman.Operator.Commands;

[OperatorArgument("operator", Description = "Starts the operator.", Order = -2)]
public class OperatorCommand(IHost app) : IOperatorCommand
{
    public Task RunAsync(string[] args)
    {
        return app.RunAsync();
    }
}
