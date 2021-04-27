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
        public static IDisposable Subscribe<T>(this T notify,Expression<Func<T,object>> expression, PropertyChangedEventHandler handler)
            where T:INotifyPropertyChanged
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var exp = expression.Body as MemberExpression;
            var name = exp.Member.Name;
            var sub = new PropertyChangedEventHandler((o, e) =>
              {
                  if (e.PropertyName == name)
                  {
                      handler(o, e);
                  }
              });
            return new Subscriber(notify, sub);
        }
        public static IDisposable Subscribe<T>(this T notify, Expression<Func<T, object>> expression, Action<object> handler)
           where T : INotifyPropertyChanged
        {
            return Subscribe(notify, expression, (o, e) => handler(o));
        }
        public static IDisposable Subscribe<T>(this T notify, Expression<Func<T, object>> expression, Action handler)
          where T : INotifyPropertyChanged
        {
            return Subscribe(notify, expression, (o, e) => handler());
        }
    }
}
