using k8s;
using k8s.Models;

namespace Frontman.Operator.Generation;

internal class ClusterRoleBindingBuilder : KubernetesObjectBuilderWithMetaData<V1ClusterRoleBinding>
{
    public override V1ClusterRoleBinding Build()
    {
        var role = base.Build();
        role.Initialize();
        return role;
    }
}
