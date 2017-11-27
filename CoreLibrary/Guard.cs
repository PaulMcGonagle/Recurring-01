using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CoreLibrary
{
    public static class Guard
    {
        [NotNull]
        public static void AgainstNull(object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }

        [NotNull]
        public static void AgainstNullOrWhiteSpace(string argument, string argumentName)
        {
            AgainstNull(argument, argumentName);

            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentOutOfRangeException(argumentName);
        }

        [NotNull]
        public static void AgainstNullOrDefault(ValueType argument, string argumentName)
        {
            AgainstNull(argument, argumentName);

            if (argument.Equals(GetDefault(argument.GetType())))
                throw new ArgumentNullException(argumentName);
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
