using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.WsTrust.WsTrustFeb2005
{
    [ServiceContract(Name = WsTrustServiceContractConstants.Contracts.IWsTrustFeb2005Async, Namespace = WsTrustServiceContractConstants.Namespace)]
    public interface IWsTrustFeb2005AsyncContract
    {
        /// <summary>
        /// Definiton of Async RST/Cancel method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005CancelAsync, AsyncPattern = true, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005CancelRequest, ReplyAction = WsTrustServiceContractConstants.Actions.TrustFeb2005CancelResponse)]
        Task<Message> TrustFeb2005CancelAsync(Message request);

        /// <summary>
        /// Definiton of Async RST/Issue method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005IssueAsync, AsyncPattern = true, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005IssueRequest, ReplyAction = WsTrustServiceContractConstants.Actions.TrustFeb2005IssueResponse)]
        Task<Message> TrustFeb2005IssueAsync(Message request);

        /// <summary>
        /// Definiton of Async RST/Renew method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005RenewAsync, AsyncPattern = true, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005RenewRequest, ReplyAction = WsTrustServiceContractConstants.Actions.TrustFeb2005RenewResponse)]
        Task<Message> TrustFeb2005RenewAsync(Message request);


        /// <summary>
        /// Definiton of Async RST/Validate method for WS-Trust Feb 2005
        /// </summary>
        /// <param name="request">Request Message containing the RST.</param>
        /// <returns>IAsyncResult result instance.</returns>
        [OperationContract(Name = WsTrustServiceContractConstants.Operations.TrustFeb2005ValidateAsync, AsyncPattern = true, Action = WsTrustServiceContractConstants.Actions.TrustFeb2005ValidateRequest, ReplyAction = WsTrustServiceContractConstants.Actions.TrustFeb2005ValidateResponse)]
        Task<Message> TrustFeb2005ValidateAsync(Message request);

    }
}
