using Frontman.Operator.Helpers;

namespace Frontman.Operator.Metadata;

[AttributeUsage(AttributeTargets.Assembly)]
public class OperatorNameAttribute(string name) : Attribute
{
    public static OperatorNameAttribute Default => new("operator");

    public string OperatorName => name;

    public override string ToString()
        => DebuggerHelpers.GetDebugText(nameof(OperatorName), OperatorName);
}

