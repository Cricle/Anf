using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace Anf.Desktop
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (!(type is null))
            {
                return (Control)Activator.CreateInstance(type);
            }
            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object data)
        {
            return data is ObservableObject;
        }
    }
}
