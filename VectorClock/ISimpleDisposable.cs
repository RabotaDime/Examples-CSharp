using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorClock
{
    class SimpleDisposable : IDisposable
    {
        private List<IDisposable> DisposableResources = new List<IDisposable> ();

        private bool IsDisposed = false; // To detect redundant calls

        public void RegisterManagedResource (IDisposable aObject)
        {
            DisposableResources.Add(aObject);
        }

        protected virtual void Dispose (bool aDisposing)
        {
            if (IsDisposed) return;

            if (aDisposing)
            {
                // Disposing managed state (managed objects).
                while (DisposableResources.Count > 0)
                {
                    IDisposable ResObj = DisposableResources[DisposableResources.Count - 1];
                    ResObj.Dispose();
                    ResObj = null;
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.

            // TODO: set large fields to null.

            IsDisposed = true;
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SimpleDisposable() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public virtual void Dispose ()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
    }
}
