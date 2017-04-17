
namespace Sparkle.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Temporarily change the current language. Use the using(){} pattern with that object.
    /// </summary>
    public class ThreadCulture : IDisposable
    {
        private bool isDisposed;

        private CultureInfo previousCulture;
        private CultureInfo previousUiCulture;

        /// <summary>
        /// Temporarily change the current language. Use the using(){} pattern with that object.
        /// </summary>
        public ThreadCulture(CultureInfo culture, CultureInfo uiCulture)
        {
            if (culture != null)
            {
                this.previousCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = culture;
            }

            if (uiCulture != null)
            {
                this.previousUiCulture = Thread.CurrentThread.CurrentUICulture;
                Thread.CurrentThread.CurrentUICulture = uiCulture;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (this.previousCulture != null)
                    {
                        Thread.CurrentThread.CurrentCulture = this.previousCulture;
                    }

                    if (this.previousUiCulture != null)
                    {
                        Thread.CurrentThread.CurrentUICulture = this.previousUiCulture;
                    }
                }

                this.isDisposed = true;
            }
        }
    }
}
