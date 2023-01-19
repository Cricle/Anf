#if !NETSTANDARD1_1&&!NETSTANDARD1_3
#define CAN_PREV_CHECK
#endif
using System;
using System.ComponentModel;

namespace Anf
{
    internal static class JsonVisitorExtensions
    {
        public static bool TryGet<T>(this IJsonVisitor visitor, out T result)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return TryGet(visitor, out _, out result);
        }
        public static bool TryGet<T>(this IJsonVisitor visitor, out Exception exception, out T result)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            result = default;
            var ok = TryGet(visitor, typeof(T), out exception, out var r);
            if (ok)
            {
                result = (T)r;
            }
            return ok;
        }
        public static bool TryGet(this IJsonVisitor visitor, Type type, out object result)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return TryGet(visitor, type, out _, out result);
        }
        public static bool TryGet(this IJsonVisitor visitor, Type type,out Exception exception, out object result)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            exception = null;
            result = null;
            if (type == typeof(string))
            {
                result = visitor.ToString();
                return true;
            }
            if (type.GenericTypeArguments.Length != 0 && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return TryGet(visitor, Nullable.GetUnderlyingType(type),out exception, out result);
            }
#if CAN_PREV_CHECK
            if (type.IsEnum)
#endif
            {
                var text = visitor.ToString();
                try
                {
                    result = Enum.Parse(type, text);
                }
                catch (Exception ex)
                {
                    exception = ex;
#if CAN_PREV_CHECK
                    return false;
#endif
                }
            }
#if CAN_PREV_CHECK
            if (type.IsPrimitive || type == typeof(decimal))
#endif
            {
                try
                {
                    result = Convert.ChangeType(visitor.ToString(), type);
                    return true;
                }
                catch (Exception ex)
                {
                    exception = ex;
#if CAN_PREV_CHECK
                    return false;
#endif
                }
            }
            var convert = TypeDescriptor.GetConverter(type);
            if (convert is null || !convert.CanConvertFrom(typeof(string)))
            {
                return false;
            }
            try
            {
                result = convert.ConvertFromInvariantString(visitor.ToString());
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
    }
}
