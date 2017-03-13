using System;
using System.Transactions;

namespace Taxes.Tests {
    public abstract class TestBase : IDisposable {
        private readonly TransactionScope _scope;

        protected TestBase() {
            _scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, TransactionScopeAsyncFlowOption.Enabled);

        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if(!disposing)
                return;

            _scope.Dispose();
        }
    }
}
