using Solid.IdentityModel.Protocols.WsTrust;
using Solid.Identity.Protocols.WsTrust.WsTrust13;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.WsTrust
{
    public partial class WsTrustService : IWsTrust13AsyncContract, IWsTrust13SyncContract
    {
        public Task<Message> Trust13CancelAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.Trust13.Actions.CancelRequest,
                WsTrustConstants.Trust13.Actions.CancelFinal,
                WsTrustConstants.Trust13)
               .AsTask()
        ;

        public Task<Message> Trust13CancelResponseAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.Trust13.Actions.CancelResponse,
                WsTrustConstants.Trust13.Actions.CancelFinal,
                WsTrustConstants.Trust13)
               .AsTask()
        ;

        public Task<Message> Trust13IssueAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.Trust13.Actions.IssueRequest,
                WsTrustConstants.Trust13.Actions.IssueFinal,
                WsTrustConstants.Trust13)
               .AsTask()
        ;

        public Task<Message> Trust13IssueResponseAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.Trust13.Actions.IssueResponse,
                WsTrustConstants.Trust13.Actions.IssueFinal,
                WsTrustConstants.Trust13)
               .AsTask()
        ;

        public Task<Message> Trust13RenewAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.Trust13.Actions.RenewRequest,
                WsTrustConstants.Trust13.Actions.RenewFinal,
                WsTrustConstants.Trust13)
               .AsTask()
        ;

        public Task<Message> Trust13RenewResponseAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.Trust13.Actions.RenewResponse,
                WsTrustConstants.Trust13.Actions.RenewFinal,
                WsTrustConstants.Trust13)
               .AsTask()
        ;

        public Task<Message> Trust13ValidateAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.Trust13.Actions.ValidateRequest,
                WsTrustConstants.Trust13.Actions.ValidateFinal,
                WsTrustConstants.Trust13)
               .AsTask()
        ;

        public Task<Message> Trust13ValidateResponseAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.Trust13.Actions.ValidateResponse,
                WsTrustConstants.Trust13.Actions.ValidateFinal,
                WsTrustConstants.Trust13)
               .AsTask()
        ;
    }
}
