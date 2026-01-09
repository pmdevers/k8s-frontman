using Frontman.Operator.Helpers;

namespace Frontman.Operator.Metadata;

[AttributeUsage(AttributeTargets.Class)]
public class LabelSelectorAttribute(string labelSelector) : Attribute
{
    public string LabelSelector { get; set; } = labelSelector;
    public override string ToString()
        => DebuggerHelpers.GetDebugText(nameof(LabelSelector), LabelSelector);
}
