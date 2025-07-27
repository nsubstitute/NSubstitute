using NSubstitute.Compatibility;
using NUnit.Framework;
using System.Reflection;

namespace NSubstitute.Acceptance.Specs;

#pragma warning disable CS0618 // Type or member is obsolete

public class CompatArgsTests
{

    [Test]
    public void CompatAndCompatArgInstanceShouldBeInSync()
    {
        var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
        var compatMembers =
            typeof(Arg.Compat).GetMethods(flags).Select(DescribeMethod).OrderBy(x => x);

        var compatInstanceMembers =
            typeof(CompatArg).GetMethods(flags).Select(DescribeMethod).OrderBy(x => x);

        Assert.That(
            compatInstanceMembers.ToList(), Is.EqualTo(compatMembers.ToList()),
            "Arg.Compat and CompatArg should have static/instance versions of the same methods"
            );
    }

    [Test]
    public void CompatAndArgShouldBeInSync()
    {
        var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
        var argMembers =
            typeof(Arg).GetMethods(flags).Select(DescribeMethod).OrderBy(x => x);
        var compatMembers =
            typeof(Arg.Compat).GetMethods(flags).Select(DescribeMethod).OrderBy(x => x);

        Assert.That(
            compatMembers.ToList(), Is.EqualTo(argMembers.ToList()),
            "Arg and Arg.Compat should have the same methods (just vary by out/ref)"
            );
    }

    private static string DescribeMethod(MethodInfo m)
    {
        return $"{m.Name}<{DescribeTypeList(m.GetGenericArguments())}>({DescribeParameters(m.GetParameters())})";
    }

    private static string DescribeTypeList(Type[] args)
    {
        return string.Join(", ", args.Select(x => x.Name));
    }

    private static string DescribeParameters(ParameterInfo[] parameters)
    {
        return string.Join(", ", parameters.Select(x => $"{x.ParameterType.Name} {x.Name}"));
    }

}
