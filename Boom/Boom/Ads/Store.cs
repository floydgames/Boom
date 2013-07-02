using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StoreLauncher;

namespace Boom
{
    class Store
    {
        private static Store _instance;

        private static Store Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new Store();
                }

                return _instance;
            }
        }

        private StoreBase _store;

        protected Store()
        {
            if (Environment.OSVersion.Version.Major >= 8)
            {
                _store = StoreLauncher.StoreLauncher.GetStoreInterface("StoreWrapper.Store, StoreWrapper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            }
            else
            {
                _store = null;
            }
        }

        public static bool Available
        {
            get
            {
                return Instance._store != null;
            }
        }

        public static bool HasPurchased(string productID)
        {
            if (Available)
            {
                if (Instance._store.LicenseInformation.ProductLicenses.Keys.Contains(productID) && Instance._store.LicenseInformation.ProductLicenses[productID].IsActive)
                {
                    return true;
                }
            }

            return false;
        }

        public static void Purchase(string productID, Action completed)
        {
            if (Available)
            {
                Instance._store.RequestProductPurchaseAsync(productID, false).Completed = (IAsyncOperationBase<string> operation, StoreAsyncStatus status) =>
                {
                    if (status == StoreAsyncStatus.Completed)
                    {
                        completed();
                    }
                };
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
