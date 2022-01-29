using Ao.Lang.Runtime;
using System;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Runtime.InteropServices;
using Windows.UI.Xaml.Documents;

namespace Anf
{
    public static class Lang
    {
        public static string GetText(FrameworkElement obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(FrameworkElement obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Lang), new PropertyMetadata(null, PropertyChangedCallback));

#if WINDOWS_UWP
        private static readonly DependencyProperty BindBoxProperty =
            DependencyProperty.RegisterAttached("BindBox", typeof(ILangStrBox), typeof(Lang), new PropertyMetadata(null));
#endif

        private static void PropertyChangedCallback([In] DependencyObject d, [In] DependencyPropertyChangedEventArgs e)
        {
            
            var key = (string)e.NewValue;
            if (!string.IsNullOrEmpty(key))
            {
                if (d is TextBlock tbx)
                {
                    Bind(tbx, TextBlock.TextProperty, key);
                }
                else if (d is Run run)
                {
#if WINDOWS_UWP
                    if (run.GetValue(BindBoxProperty) is ILangStrBox box)
                    {
                        box.Dispose();
                    }
                    box=LanguageManager.Instance.BindTo(key, run, x => x.Text);
                    run.SetValue(BindBoxProperty, box);
#else
                    Bind(run, Run.TextProperty, key);
#endif
                }
            }
        }
        private static void Bind(DependencyObject obj,DependencyProperty prop,string key)
        {
            var box = LangBindExtensions.CreateLangBox(LanguageManager.Instance, key);
            var bd = new Binding { Source = box, Path = new PropertyPath(nameof(ILangStrBox.Value)) };
            BindingOperations.SetBinding(obj, prop, bd);
        }
    }

}
