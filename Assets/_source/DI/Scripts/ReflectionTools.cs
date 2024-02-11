using System.Reflection;

namespace DI
{
    public static class ReflectionTools
    {
        public static MethodInfo[] GetMethods(object target)
        {
            MethodInfo[] methods = target.GetType().GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.FlattenHierarchy
                );
            return methods;
        }

        public static FieldInfo[] GetFields(object target)
        {
            FieldInfo[] fields = target.GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly
            );
            return fields;
        }

    }
}
