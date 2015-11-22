using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Utils
{
    /// <summary>
    /// Предоставляет функционал для работы с сертификатами.
    /// </summary>
    internal class Certificates
    {
        public static X509Certificate2 FindCertificate(string serial)
        {
            var localStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            localStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            try
            {
                X509Certificate2Collection matches = localStore.Certificates.Find(
                    X509FindType.FindBySerialNumber,
                    serial,
                    true);

                if (matches.Count > 0)
                {
                    return matches[0];
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                localStore.Close();
            }
        }

        public static X509Certificate2 FindCertificate(string subjectDistinguishedName, X509KeyUsageFlags usage)
        {
            var localStore = new X509Store(StoreName.My);

            localStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            try
            {
                X509Certificate2Collection matches = localStore.Certificates.Find(
                    X509FindType.FindBySubjectDistinguishedName,
                    subjectDistinguishedName,
                    true);

                if (matches.Count > 0)
                {
                    foreach (X509Certificate2 cert in matches)
                    {
                        foreach (X509Extension extension in cert.Extensions)
                        {
                            var usageExtension = extension as X509KeyUsageExtension;

                            if (usageExtension != null)
                            {
                                bool matchesUsageRequirements = ((usage & usageExtension.KeyUsages) == usage);

                                if (matchesUsageRequirements)
                                {
                                    return cert;
                                }
                            }
                        }
                    }

                    return null;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                localStore.Close();
            }
        }

        public static List<string> SelectCertificates(StoreName name, StoreLocation location)
        {
            var localStore = new X509Store(name, location);

            localStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            try
            {
                return
                    localStore.Certificates.Cast<X509Certificate2>().Select(
                        a => a.Subject + "(" + a.SerialNumber + ") [" + a.PrivateKey + "]").ToList();
            }
            finally
            {
                localStore.Close();
            }
        }

        public static void CopySertificateTo(string serialNumber, StoreName storeNameFrom,
                                             StoreLocation storeLocationFrom, StoreName storeNameTo,
                                             StoreLocation storeLocationTo)
        {
            var fromStore = new X509Store(storeNameFrom, storeLocationFrom);
            fromStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var toMachine = new X509Store(storeNameTo, storeLocationTo);
            toMachine.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);
            try
            {
                X509Certificate2 cert =
                    fromStore.Certificates.Cast<X509Certificate2>().FirstOrDefault(v => v.SerialNumber == serialNumber);
                if (cert != null)
                    toMachine.Add(cert);
            }
            finally
            {
                toMachine.Close();
                fromStore.Close();
            }
        }

        public static void RemoveCertificateFrom(string serialNumber, StoreName storeNameFrom,
                                                 StoreLocation storeLocationFrom)
        {
            var fromStore = new X509Store(storeNameFrom, storeLocationFrom);

            fromStore.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);
            try
            {
                X509Certificate2 cert =
                    fromStore.Certificates.Cast<X509Certificate2>().FirstOrDefault(v => v.SerialNumber == serialNumber);
                if (cert != null)
                    fromStore.Remove(cert);
            }
            finally
            {
                fromStore.Close();
            }
        }

        public static void ImportFromFile(string path, string password, StoreName storeNameTo,
                                          StoreLocation storeLocationTo)
        {
            var fromStore = new X509Store(storeNameTo, storeLocationTo);

            fromStore.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);
            try
            {
                fromStore.Certificates.Import(path, password, X509KeyStorageFlags.MachineKeySet);
            }
            finally
            {
                fromStore.Close();
            }
        }

        public static int CertsCount()
        {
            var localStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            localStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            return localStore.Certificates.Count;
        }
    }
}