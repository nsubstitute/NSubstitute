using NSubstitute.Compatibility;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Reflection;

namespace NSubstitute.Acceptance.Specs;


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

        ClassicAssert.AreEqual(
            compatMembers.ToList(), compatInstanceMembers.ToList(),
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

        ClassicAssert.AreEqual(
            argMembers.ToList(), compatMembers.ToList(),
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
