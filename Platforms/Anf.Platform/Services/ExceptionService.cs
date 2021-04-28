using Anf.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
            private set => Set(ref hasException, value);
        }

        public Exception Exception
        {
            get { return exception; }
            set
            {
                Set(ref exception, value);
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
