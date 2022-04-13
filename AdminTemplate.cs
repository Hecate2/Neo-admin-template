using System.ComponentModel;
using System.Numerics;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;


namespace Neo.SmartContract.Framework
{
    [DisplayName("AdminTemplate")]
    [ManifestExtra("Author", "Hecate2")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "This is an AdminTemplate")]
    public class AdminTemplate : SmartContract
    {
        [InitialValue("Fill the admin wallet address here", ContractParameterType.Hash160)]
        private const UInt160 INITIAL_ADMIN = default;
        private const string PREFIX_ADMIN = "_ADMIN";

        public static void _deploy(object data, bool update)
        {
            if (update) return;
            Storage.Put(Storage.CurrentContext, PREFIX_ADMIN, INITIAL_ADMIN);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool Verify() => Runtime.CheckWitness((UInt160)Storage.Get(Storage.CurrentContext, PREFIX_ADMIN));

        public static void Update(ByteString nefFile, string manifest)
        {
            ExecutionEngine.Assert(Verify(), "No authorization.");
            ContractManagement.Update(nefFile, manifest, null);
        }
        public static void Destroy()
        {
            ExecutionEngine.Assert(Verify(), "No authorization.");
            ContractManagement.Destroy();
        }

        public static UInt160 GetAdmin() => (UInt160)Storage.Get(Storage.CurrentContext, PREFIX_ADMIN);
        public static void ChangeAdmin(UInt160 newAdmin)
        {
            StorageContext context = Storage.CurrentContext;
            ExecutionEngine.Assert(Runtime.CheckWitness((UInt160)Storage.Get(context, PREFIX_ADMIN)), "No authorization.");
            Storage.Put(context, PREFIX_ADMIN, newAdmin);
        }

        public static bool PikaNep17(UInt160 tokenContract, UInt160 targetAddress, BigInteger amount, object data=null)
        {
            ExecutionEngine.Assert(Verify(), "No authorization.");
            return (bool)Contract.Call(tokenContract, "transfer", CallFlags.All, Runtime.ExecutingScriptHash, targetAddress, amount, data);
        }
        public static bool PikaNep11NonDivisible(UInt160 tokenContract, UInt160 targetAddress, ByteString tokenId, object data =null)
        {
            ExecutionEngine.Assert(Verify(), "No authorization.");
            return (bool)Contract.Call(tokenContract, "transfer", CallFlags.All, targetAddress, tokenId, data);
        }
        public static bool PikaNep11Divisible(UInt160 tokenContract, UInt160 targetAddress, BigInteger amount, ByteString tokenId, object data = null)
        {
            ExecutionEngine.Assert(Verify(), "No authorization.");
            return (bool)Contract.Call(tokenContract, "transfer", CallFlags.All, Runtime.ExecutingScriptHash, targetAddress, amount, tokenId, data);
        }
    }
}
