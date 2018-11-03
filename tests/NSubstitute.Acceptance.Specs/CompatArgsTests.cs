using System;
using System.Reflection;
using System.Linq;
using NSubstitute.Compatibility;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs {

    public class CompatArgsTests {

        [Test]
        public void CompatArgsAndInstanceShouldBeInSync() {
            var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
            var compatMembers =
                typeof(Arg.Compat).GetMethods(flags).Select(DescribeMethod).OrderBy(x => x);
            var compatInstanceMembers =
                typeof(CompatArg).GetMethods(flags).Select(DescribeMethod).OrderBy(x => x);

            Assert.AreEqual(
                compatMembers, compatInstanceMembers,
                "CompatArgs and CompatArgsInstance should have static/instance versions of the same members"
                );
        }

        private static string DescribeMethod(MethodInfo m) {
            return $"{m.Name}<{DescribeTypeList(m.GetGenericArguments())}>({DescribeParameters(m.GetParameters())})";
        }

        private static string DescribeTypeList(Type[] args) {
            return string.Join(", ", args.Select(x => x.Name));
        }

        private static string DescribeParameters(ParameterInfo[] parameters) {
            return string.Join(", ", parameters.Select(x => $"{x.ParameterType.Name} {x.Name}"));
        }

    }
}
