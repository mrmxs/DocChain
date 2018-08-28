using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using EthereumLibrary.Helper;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;
using Nethereum.Geth;
using Nethereum.Web3;

namespace EthereumLibrary.ContractService
{
   public class IpfsFileStorageService : AbstractContractService
   {
        public static string Abi => ResourceHelper.Get("IpfsFileStorage.abi");
        public static string ByteCode => ResourceHelper.Get("IpfsFileStorage.bin");

        // public static string Abi = @"[{'constant':false,'inputs':[{'name':'_index','type':'uint256'},{'name':'_value','type':'bytes32[3]'},{'name':'timestamp','type':'uint32'}],'name':'setName','outputs':[],'payable':false,'stateMutAbility':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'_mimeType','type':'bytes32'},{'name':'_ipfsHash','type':'bytes32[2]'},{'name':'_size','type':'bytes32'},{'name':'_name','type':'bytes32[3]'},{'name':'_description','type':'bytes32[6]'},{'name':'timestamp','type':'uint32'}],'name':'add','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutAbility':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_index','type':'uint256'}],'name':'get','outputs':[{'name':'mimeType','type':'bytes32'},{'name':'ipfsHash','type':'bytes32[2]'},{'name':'size','type':'bytes32'},{'name':'name','type':'bytes32[3]'},{'name':'description','type':'bytes32[6]'},{'name':'created','type':'uint32'},{'name':'modified','type':'uint32'}],'payable':false,'stateMutAbility':'view','type':'function'},{'constant':false,'inputs':[{'name':'_index','type':'uint256'},{'name':'_value','type':'bytes32[6]'},{'name':'timestamp','type':'uint32'}],'name':'setDescription','outputs':[],'payable':false,'stateMutAbility':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_index','type':'uint256'}],'name':'contains','outputs':[{'name':'result','type':'bool'}],'payable':false,'stateMutAbility':'view','type':'function'}]";
        // public static string ByteCode = "0x608060405234801561001057600080fd5b5061084a806100206000396000f30060806040526004361061006c5763ffffffff7c0100000000000000000000000000000000000000000000000000000000600035041663103532e981146100715780632ddded47146100c15780639507d39a146101735780639d0744fe14610251578063c34052e01461029f575b600080fd5b34801561007d57600080fd5b506040805160608181019092526100bf9160048035923692608491906024906003908390839080828437509396505050913563ffffffff1692506102cb915050565b005b3480156100cd57600080fd5b506040805180820182526101619160048035923692606491906024906002908390839080828437505060408051606081810190925294978635979096909560808201955093506020019150600390839083908082843750506040805160c0818101909252949796958181019594509250600691508390839080828437509396505050913563ffffffff16925061039b915050565b60408051918252519081900360200190f35b34801561017f57600080fd5b5061018b6004356104bb565b60408051888152906020820190889080838360005b838110156101b85781810151838201526020016101a0565b5050505091909101878152602001905085606080838360005b838110156101e95781810151838201526020016101d1565b5050505090500184600660200280838360005b838110156102145781810151838201526020016101fc565b505050509050018363ffffffff1663ffffffff1681526020018263ffffffff1663ffffffff16815260200197505050505050505060405180910390f35b34801561025d57600080fd5b506040805160c08181019092526100bf916004803592369260e491906024906006908390839080828437509396505050913563ffffffff169250610652915050565b3480156102ab57600080fd5b506102b76004356106e6565b604080519115158252519081900360200190f35b826102d5816106e6565b151561034257604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601260248201527f4e4f54204558495354494e4720494e4445580000000000000000000000000000604482015290519081900360640190fd5b600084815260208190526040902061035f9060040184600361070a565b5050600092835260208390526040909220600d01805463ffffffff9093166401000000000267ffffffff00000000199093169290921790915550565b600180548101808255604080516101008101825289815260208082018a81528284018a9052606083018990526080830188905263ffffffff871660a0840181905260c084015260e083018690526000948552908490529183208151815591519293909261040c91830190600261074a565b506040820151600380830191909155606083015161042f9160048401919061070a565b5060808201516104459060078301906006610779565b5060a0820151600d91909101805460c084015160e09094015163ffffffff1990911663ffffffff9384161767ffffffff00000000191664010000000093909416929092029290921768ff000000000000000019166801000000000000000091151591909102179055505060015495945050505050565b60006104c56107a8565b60006104cf6107c3565b6104d76107e2565b600080876104e4816106e6565b151561055157604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601260248201527f4e4f54204558495354494e4720494e4445580000000000000000000000000000604482015290519081900360640190fd5b60008981526020819052604090819020805460038201548351808501909452909a50975060010160028282826020028201915b815481526001909101906020018083116105845750505060008c8152602081905260409081902081516060810192839052949b5060040192506003915082845b815481526001909101906020018083116105c45750505060008c81526020819052604090819020815160c081019283905294995060070192506006915082845b815481526001909101906020018083116106045750505060009b8c52505060208a90526040909920600d0154979996989597949663ffffffff80871696640100000000900416945092505050565b8261065c816106e6565b15156106c957604080517f08c379a000000000000000000000000000000000000000000000000000000000815260206004820152601260248201527f4e4f54204558495354494e4720494e4445580000000000000000000000000000604482015290519081900360640190fd5b600084815260208190526040902061035f90600701846006610779565b6000908152602081905260409020600d015468010000000000000000900460ff1690565b826003810192821561073a579160200282015b8281111561073a578251825560209092019160019091019061071d565b50610746929150610801565b5090565b826002810192821561073a579160200282018281111561073a578251825560209092019160019091019061071d565b826006810192821561073a579160200282018281111561073a578251825560209092019160019091019061071d565b60408051808201825290600290829080388339509192915050565b6060604051908101604052806003906020820280388339509192915050565b60c0604051908101604052806006906020820280388339509192915050565b61081b91905b808211156107465760008155600101610807565b905600a165627a7a723058203b26290708c1c5da43d4e2a5baa921040b40d56c1787967c75a0a32fe184a4080029";

        public static Task<string> DeployContractAsync(Web3Geth web3, string addressFrom,  HexBigInteger gas = null, HexBigInteger valueAmount = null) 
        {
            return web3.Eth.DeployContract.SendRequestAsync(Abi, ByteCode, addressFrom, gas, valueAmount );
        }

        private Contract contract;

        public IpfsFileStorageService(Web3Geth web3, string address)
        {
            this.web3 = web3;
            this.contract = web3.Eth.GetContract(Abi, address);
        }

        public Function GetFunctionAdd() {
            return contract.GetFunction("add");
        }
        public Function GetFunctionGet() {
            return contract.GetFunction("get");
        }
        public Function GetFunctionSetName() {
            return contract.GetFunction("setName");
        }
        public Function GetFunctionSetDescription() {
            return contract.GetFunction("setDescription");
        }
        public Function GetFunctionContains() {
            return contract.GetFunction("contains");
        }

        public Task<string> AddAsync(string addressFrom, byte[] _mimeType, byte[][] _ipfsHash, byte[] _size, byte[][] _name, byte[][] _description, int timestamp, HexBigInteger gas = null, HexBigInteger valueAmount = null) {
            var function = GetFunctionAdd();
            return function.SendTransactionAsync(addressFrom, gas, valueAmount, _mimeType, _ipfsHash, _size, _name, _description, timestamp);
        }
        public Task<string> SetNameAsync(string addressFrom, BigInteger _index, byte[][] _value, int timestamp, HexBigInteger gas = null, HexBigInteger valueAmount = null) {
            var function = GetFunctionSetName();
            return function.SendTransactionAsync(addressFrom, gas, valueAmount, _index, _value, timestamp);
        }
        public Task<string> SetDescriptionAsync(string addressFrom, BigInteger _index, byte[][] _value, int timestamp, HexBigInteger gas = null, HexBigInteger valueAmount = null) {
            var function = GetFunctionSetDescription();
            return function.SendTransactionAsync(addressFrom, gas, valueAmount, _index, _value, timestamp);
        }

        public Task<BigInteger> AddAsyncCall(byte[] _mimeType, byte[][] _ipfsHash, byte[] _size, byte[][] _name, byte[][] _description, int timestamp) {
            var function = GetFunctionAdd();
            return function.CallAsync<BigInteger>(_mimeType, _ipfsHash, _size, _name, _description, timestamp);
        }
        public Task<bool> ContainsAsyncCall(BigInteger _index) {
            var function = GetFunctionContains();
            return function.CallAsync<bool>(_index);
        }
       
        public Task<GetDTO> GetAsyncCall(BigInteger _index) {
            var function = GetFunctionGet();
            return function.CallDeserializingToObjectAsync<GetDTO>(_index);
        }
    }

    [FunctionOutput]
    public class GetDTO 
    {
        [Parameter("bytes32", "mimeType", 1)]
        public string MimeType {get; set;}

        [Parameter("bytes32[2]", "ipfsHash", 2)]
        public List<string> IpfsHash {get; set;}

        [Parameter("bytes32", "size", 3)]
        public string Size {get; set;}
        
        [Parameter("bytes32[3]", "name", 4)]
        public List<string> Name {get; set;}

        [Parameter("bytes32[6]", "description", 5)]
        public List<string> Description {get; set;}

        [Parameter("uint32", "created", 6)]
        public long Created {get; set;}

        [Parameter("uint32", "modified", 7)]
        public long Modified {get; set;}
        
        public ReadableGetDTO ToReadable()
        {
            return new ReadableGetDTO
            {
                MimeType = MimeType,
                IpfsHash = String.Join<string>("", IpfsHash.ToArray()),
                Size = Size,
                Name = String.Join<string>("", Name.ToArray()),
                Description = String.Join<string>("", Description.ToArray()),
                Created = new DateTime(this.Created),
                Modified = new DateTime(this.Modified),
            };
        }
    }
    
    
    [FunctionOutput]
    public class ReadableGetDTO 
    {
        [Parameter("bytes32", "mimeType", 1)]
        public string MimeType {get; set;}

        [Parameter("bytes32[2]", "ipfsHash", 2)]
        public string IpfsHash {get; set;}

        [Parameter("bytes32", "size", 3)]
        public string Size {get; set;}

        [Parameter("bytes32[3]", "name", 4)]
        public string Name {get; set;}

        [Parameter("bytes32[6]", "description", 5)]
        public string Description {get; set;}

        [Parameter("uint32", "created", 6)]
        public DateTime Created {get; set;}

        [Parameter("uint32", "modified", 7)]
        public DateTime Modified {get; set;}
    }



}

