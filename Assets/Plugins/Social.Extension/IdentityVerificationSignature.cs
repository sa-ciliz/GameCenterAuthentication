using System;
using System.Runtime.InteropServices;
using AOT;

namespace Plugins.Social.Extension
{
    public static class IdentityVerificationSignature
    {
        public static string PublicKeyUrl;
        public static byte[] Signature;
        public static byte[] Salt;
        public static ulong Timestamp;
        public static string Error;

        public static event Action UpdateIdentity;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void IdentityVerificationSignatureCallback(
            string publicKeyUrl, 
            IntPtr signaturePointer, int signatureLength,
            IntPtr saltPointer, int saltLength,
            ulong timestamp,
            string error);

#if UNITY_IOS
        [DllImport("__Internal")]
        public static extern void generateIdentityVerificationSignature(
            [MarshalAs(UnmanagedType.FunctionPtr)]IdentityVerificationSignatureCallback callback);
#else
        public static void generateIdentityVerificationSignature(IdentityVerificationSignatureCallback callback)
        {
            throw new NotSupportedException();
        }
#endif

        // Note: This callback has to be static because Unity's il2Cpp doesn't support marshalling instance methods.
        [MonoPInvokeCallback(typeof(IdentityVerificationSignatureCallback))]
        public static void OnIdentityVerificationSignatureGenerated(
            string publicKeyUrl, 
            IntPtr signaturePointer, int signatureLength,
            IntPtr saltPointer, int saltLength,
            ulong timestamp,
            string error)
        {
            // Create a managed array for the signature
            var signature = new byte[signatureLength];
            Marshal.Copy(signaturePointer, signature, 0, signatureLength);

            // Create a managed array for the salt
            var salt = new byte[saltLength];
            Marshal.Copy(saltPointer, salt, 0, saltLength);

            PublicKeyUrl = publicKeyUrl;
            Signature = signature;
            Salt = salt;
            Timestamp = timestamp;
            Error = error;

            UnityEngine.Debug.Log($"publicKeyUrl: {publicKeyUrl}");
            UnityEngine.Debug.Log($"signature: {signature.Length}");
            UnityEngine.Debug.Log($"salt: {salt.Length}");
            UnityEngine.Debug.Log($"timestamp: {timestamp}");
            UnityEngine.Debug.Log($"error: {error}");
            
            UpdateIdentity?.Invoke();
        }
    }
}
