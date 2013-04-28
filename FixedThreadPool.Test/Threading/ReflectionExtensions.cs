using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Svyaznoy.Threading
{
    internal static class ReflectionExtensions
    {
        public static TValue GetPropertyValue<TValue>(this object target, string propertyName)
        {
            if (target == null) throw new ArgumentNullException("target");

            var targetType = target.GetType();
            var propertyInfo = targetType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (propertyInfo == null)
                throw new InvalidOperationException(
                    string.Format("Class `{0}' has not instance property `{1}'", targetType.Name, propertyName));

            return (TValue)propertyInfo.GetValue(target, null);
        }
    }
}