using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;

namespace VP.Common.Core.Reflection
{
    public static class AssemblyExtensions
    {
        // This method courtesy of Phil Haack: http://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx/
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            assembly.ThrowIfNull(nameof(assembly));

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static IEnumerable<MethodInfo> GetAsyncVoidMethods(this Assembly assembly)
        {
            return assembly.GetLoadableTypes()
              .SelectMany(type => type.GetMethods(
                BindingFlags.NonPublic
                | BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.DeclaredOnly))
              .Where(method => method.HasAttribute<AsyncStateMachineAttribute>())
              .Where(method => method.ReturnType == typeof(void));
        }

        public static bool HasAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute
        {
            return method.GetCustomAttributes(typeof(TAttribute), false).Any();
        }
    }
}