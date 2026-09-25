using Solid.IdentityModel.Protocols.WsTrust;
using Solid.Identity.Protocols.WsTrust.WsTrustFeb2005;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.WsTrust
{
    public partial class WsTrustService : IWsTrustFeb2005AsyncContract, IWsTrustFeb2005SyncContract
    {
        public Task<Message> TrustFeb2005CancelAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.Actions.CancelRequest,
                WsTrustConstants.TrustFeb2005.Actions.CancelResponse,
                WsTrustConstants.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005IssueAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.Actions.IssueRequest,
                WsTrustConstants.TrustFeb2005.Actions.IssueResponse,
                WsTrustConstants.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005RenewAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.Actions.RenewRequest,
                WsTrustConstants.TrustFeb2005.Actions.RenewResponse,
                WsTrustConstants.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005ValidateAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.Actions.ValidateRequest,
                WsTrustConstants.TrustFeb2005.Actions.ValidateResponse,
                WsTrustConstants.TrustFeb2005)
               .AsTask()
        ;

    }
}
