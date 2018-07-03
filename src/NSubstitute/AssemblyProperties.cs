using System.Runtime.CompilerServices;
using Castle.Core.Internal;

// Make internal types visible to Castle's dynamic assembly as we might want
// to dynamically define internal IsReadOnlyAttribute attribute.
[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)]