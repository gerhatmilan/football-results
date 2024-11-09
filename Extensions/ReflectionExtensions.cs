using System.Reflection;
using System.Runtime.CompilerServices;

namespace Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the  <see cref="Type"/> is a user-defined type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns><see langword="true"/> if the type is user-defined; otherwise <see langword="false"/>.</returns>
        public static bool IsUserDefinedType(this Type type)
        {
            return type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length < 1;
        }

        /// <summary>
        /// Gets all user defined types in the specified namespace in the given <see cref="Assembly"/>.
        /// </summary>
        /// <returns>All user defined types in the specified namespace in the given <see cref="Assembly"/>.</returns>
        public static Type[] GetUserDefinedClassesFromNamespace(this Assembly assembly, string targetNamespace)
        {
            return assembly
                .GetTypes()
                .Where(type => type.IsUserDefinedType() && type.IsClass && type.Namespace != null && type.Namespace.Equals(targetNamespace))
                .ToArray();
        }
    }
}
