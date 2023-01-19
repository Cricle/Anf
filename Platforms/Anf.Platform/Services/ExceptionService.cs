using Anf.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Platform.Services
{
    public class ExceptionService : ObservableObject
    {
        public ExceptionService()
        {
            CopyExceptionCommand = new RelayCommand(CopyException);
            ClearExceptionCommand = new RelayCommand(ClearException);
        }
        private Exception exception;
        private bool hasException;

        public bool HasException
        {
            get { return hasException; }
            private set => SetProperty(ref hasException, value);
        }

        public Exception Exception
        {
            get { return exception; }
            set
            {
                SetProperty(ref exception, value);
                HasException = value != null;
            }
        }
        public RelayCommand CopyExceptionCommand { get; }
        public RelayCommand ClearExceptionCommand { get; }

        public void CopyException()
        {
            if (Exception != null)
            {
                AppEngine.GetRequiredService<IPlatformService>()
                    .Copy(Exception.ToString());
            }
        }
        public void ClearException()
        {
            Exception = null;
        }
    }
}
