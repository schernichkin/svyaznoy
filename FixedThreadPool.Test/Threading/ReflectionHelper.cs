using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Svyaznoy.Threading
{
    [Obsolete("Use ReflectionExtensions")]
    //TODO: заменить статическим классом. 
    internal sealed class ReflectionHelper<TValue>
    {
        public ReflectionHelper(TValue value)
        {
            m_Value = value;
        }

        public ReflectionHelper<object> GetProperty(string propertyName)
        {
            return GetProperty<object>(propertyName);
        }

        public ReflectionHelper<TPropertyValue> GetProperty<TPropertyValue>(string propertyName)
        {
            var valueType = typeof(TValue);
            if (valueType.IsValueType)
            {
                return GetInstanceProperty<TPropertyValue>(valueType, propertyName);
            }
            else
            {
                if (Value == null)
                {
                    throw new InvalidOperationException("Unable get property of reference object set to null.");
                }
                else
                {
                    var realType = Value.GetType();
                    return GetInstanceProperty<TPropertyValue>(realType, propertyName);
                }
            }
        }

        #region public TValue Value

        private readonly TValue m_Value;

        public TValue Value { get { return m_Value; } }

        #endregion

        private ReflectionHelper<TPropertyValue> GetInstanceProperty<TPropertyValue>(Type reflectedType, string propertyName)
        {
            if (reflectedType == null) throw new ArgumentNullException("reflectedType");

            return new ReflectionHelper<TPropertyValue>(
                (TPropertyValue) reflectedType
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(this.Value, null));
        }
    }
}