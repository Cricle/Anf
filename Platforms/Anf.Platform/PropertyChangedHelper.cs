using System.Linq.Expressions;

namespace System.ComponentModel
{
    public static class PropertyChangedHelper
    {
        class Subscriber : IDisposable
        {
            private readonly INotifyPropertyChanged notify;
            private readonly PropertyChangedEventHandler sub;

            public Subscriber(INotifyPropertyChanged notify, PropertyChangedEventHandler sub)
            {
                this.notify = notify;
                this.sub = sub;
            }

            public void Dispose()
            {
                notify.PropertyChanged -= sub;
            }
        }
        public static IDisposable Subscribe<T>(this T notify, Expression<Func<T, object>> expression, PropertyChangedEventHandler handler)
            where T : INotifyPropertyChanged
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            string name = null;
            if (expression.Body is MemberExpression exp)
            {
                name = exp.Member.Name;
            }
            else if (expression.Body is UnaryExpression uexp && uexp.Operand is MemberExpression propExp)
            {
                name = propExp.Member.Name;
            }
            var sub = new PropertyChangedEventHandler((o, e) =>
              {
                  if (e.PropertyName == name)
                  {
                      handler(o, e);
                  }
              });
            notify.PropertyChanged += sub;
            return new Subscriber(notify, sub);
        }
        public static IDisposable Subscribe<T>(this T notify, Expression<Func<T, object>> expression, Action<object> handler)
           where T : INotifyPropertyChanged
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return Subscribe(notify, expression, (o, e) => handler(o));
        }
        public static IDisposable Subscribe<T>(this T notify, Expression<Func<T, object>> expression, Action handler)
          where T : INotifyPropertyChanged
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return Subscribe(notify, expression, (o, e) => handler());
        }
    }
}
