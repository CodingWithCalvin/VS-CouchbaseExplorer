using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CodingWithCalvin.CouchbaseExplorer.Services
{
    /// <summary>
    /// Service for securely storing and retrieving passwords using Windows Credential Manager.
    /// </summary>
    public static class CredentialManagerService
    {
        private const string CredentialPrefix = "CouchbaseExplorer:";

        public static void SavePassword(string connectionId, string password)
        {
            if (string.IsNullOrEmpty(connectionId))
                throw new ArgumentNullException(nameof(connectionId));

            var targetName = GetTargetName(connectionId);

            if (string.IsNullOrEmpty(password))
            {
                DeletePassword(connectionId);
                return;
            }

            var passwordBytes = Encoding.Unicode.GetBytes(password);

            var credential = new NativeMethods.CREDENTIAL
            {
                Type = NativeMethods.CRED_TYPE_GENERIC,
                TargetName = targetName,
                CredentialBlobSize = (uint)passwordBytes.Length,
                CredentialBlob = Marshal.AllocHGlobal(passwordBytes.Length),
                Persist = NativeMethods.CRED_PERSIST_LOCAL_MACHINE,
                UserName = connectionId
            };

            try
            {
                Marshal.Copy(passwordBytes, 0, credential.CredentialBlob, passwordBytes.Length);

                if (!NativeMethods.CredWrite(ref credential, 0))
                {
                    throw new InvalidOperationException($"Failed to save credential. Error: {Marshal.GetLastWin32Error()}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(credential.CredentialBlob);
            }
        }

        public static string GetPassword(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
                return null;

            var targetName = GetTargetName(connectionId);

            if (!NativeMethods.CredRead(targetName, NativeMethods.CRED_TYPE_GENERIC, 0, out var credentialPtr))
            {
                return null;
            }

            try
            {
                var credential = Marshal.PtrToStructure<NativeMethods.CREDENTIAL>(credentialPtr);

                if (credential.CredentialBlobSize > 0 && credential.CredentialBlob != IntPtr.Zero)
                {
                    var passwordBytes = new byte[credential.CredentialBlobSize];
                    Marshal.Copy(credential.CredentialBlob, passwordBytes, 0, (int)credential.CredentialBlobSize);
                    return Encoding.Unicode.GetString(passwordBytes);
                }

                return null;
            }
            finally
            {
                NativeMethods.CredFree(credentialPtr);
            }
        }

        public static void DeletePassword(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
                return;

            var targetName = GetTargetName(connectionId);
            NativeMethods.CredDelete(targetName, NativeMethods.CRED_TYPE_GENERIC, 0);
        }

        private static string GetTargetName(string connectionId)
        {
            return CredentialPrefix + connectionId;
        }

        private static class NativeMethods
        {
            public const int CRED_TYPE_GENERIC = 1;
            public const int CRED_PERSIST_LOCAL_MACHINE = 2;

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool CredWrite([In] ref CREDENTIAL credential, [In] uint flags);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool CredRead(string targetName, int type, int flags, out IntPtr credential);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool CredDelete(string targetName, int type, int flags);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern void CredFree(IntPtr credential);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct CREDENTIAL
            {
                public uint Flags;
                public int Type;
                public string TargetName;
                public string Comment;
                public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
                public uint CredentialBlobSize;
                public IntPtr CredentialBlob;
                public int Persist;
                public uint AttributeCount;
                public IntPtr Attributes;
                public string TargetAlias;
                public string UserName;
            }
        }
    }
}
