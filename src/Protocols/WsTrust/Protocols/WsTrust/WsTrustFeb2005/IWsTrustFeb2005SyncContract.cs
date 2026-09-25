using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.WsTrust.WsTrustFeb2005
{

    [ServiceContract(Name = WsTrustServiceContractConstants.Contracts.IWsTrustFeb2005Sync, Namespace = WsTrustServiceContractConstants.Namespace)]
    public interface IWsTrustFeb2005SyncContract
    {
        /// <summary>
        /// Definition of Async RST/Cancel method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005Cancel, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005CancelRequest, ReplyAction = WsTrustServiceContractConstants.Actions.TrustFeb2005CancelResponse)]
        Task<Message> TrustFeb2005CancelAsync(Message request);

        /// <summary>
        /// Definition of Async RST/Issue method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005Issue, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005IssueRequest, ReplyAction = WsTrustServiceContractConstants.Actions.TrustFeb2005IssueResponse)]
        Task<Message> TrustFeb2005IssueAsync(Message request);

        /// <summary>
        /// Definition of Async RST/Renew method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005Renew, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005RenewRequest, ReplyAction = WsTrustServiceContractConstants.Actions.TrustFeb2005RenewResponse)]
        Task<Message> TrustFeb2005RenewAsync(Message request);


        /// <summary>
        /// Definition of Async RST/Validate method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005Validate, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005ValidateRequest, ReplyAction = WsTrustServiceContractConstants.Actions.TrustFeb2005ValidateResponse)]
        Task<Message> TrustFeb2005ValidateAsync(Message request);

        /// <summary>
        /// Definition of Async RSTR/Cancel method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        //
        // NOTE:
        //      ReplyAction = "*" has a side effect of not generating this operation, port, or messages in the 
        //      WCF-generated WSDL. This is desired.
        //
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005CancelResponse, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005CancelResponse, ReplyAction = "*")]
        Task<Message> TrustFeb2005CancelResponseAsync(Message request);


        /// <summary>
        /// Definition of Async RSTR/Issue method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        //
        // NOTE:
        //      ReplyAction = "*" has a side effect of not generating this operation, port, or messages in the 
        //      WCF-generated WSDL. This is desired.
        //
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005IssueResponse, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005IssueResponse, ReplyAction = "*")]
        Task<Message> TrustFeb2005IssueResponseAsync(Message request);

        /// <summary>
        /// Definition of Async RSTR/Renew method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <param name="callback">AsyncCallback context.</param>
        /// <param name="state">Asyn state.</param>
        /// <returns>IAsyncResult result instance.</returns>
        //
        // NOTE:
        //      ReplyAction = "*" has a side effect of not generating this operation, port, or messages in the 
        //      WCF-generated WSDL. This is desired.
        //
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005RenewResponse, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005RenewResponse, ReplyAction = "*")]
        Task<Message> TrustFeb2005RenewResponseAsync(Message request);

        /// <summary>
        /// Definition of Async RSTR/Validate method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <param name="callback">AsyncCallback context.</param>
        /// <param name="state">Asyn state.</param>
        /// <returns>IAsyncResult result instance.</returns>
        //
        // NOTE:
        //      ReplyAction = "*" has a side effect of not generating this operation, port, or messages in the 
        //      WCF-generated WSDL. This is desired.
        //
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005ValidateResponse, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005ValidateResponse, ReplyAction = "*")]
        Task<Message> TrustFeb2005ValidateResponseAsync(Message request);
    }
}
