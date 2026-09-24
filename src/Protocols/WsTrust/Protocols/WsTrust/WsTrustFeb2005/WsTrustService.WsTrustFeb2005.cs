using Solid.IdentityModel.Protocols.WsTrust;
using Solid.Identity.Protocols.WsTrust.WsTrustFeb2005;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Solid.Identity.Protocols.WsTrust.WsTrustFeb2005;

namespace Solid.Identity.Protocols.WsTrust
{
    public partial class WsTrustService : IWsTrustFeb2005AsyncContract, IWsTrustFeb2005SyncContract
    {
        public Task<Message> TrustFeb2005CancelAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.WsTrustActions.CancelRequest,
                WsTrustConstants.TrustFeb2005.WsTrustActions.CancelFinal,
                WsTrustVersion.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005CancelResponseAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.WsTrustActions.CancelResponse,
                WsTrustConstants.TrustFeb2005.WsTrustActions.CancelFinal,
                WsTrustVersion.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005IssueAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.WsTrustActions.IssueRequest,
                WsTrustConstants.TrustFeb2005.WsTrustActions.IssueFinal,
                WsTrustVersion.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005IssueResponseAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.WsTrustActions.IssueResponse,
                WsTrustConstants.TrustFeb2005.WsTrustActions.IssueFinal,
                WsTrustVersion.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005RenewAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.WsTrustActions.RenewRequest,
                WsTrustConstants.TrustFeb2005.WsTrustActions.RenewFinal,
                WsTrustVersion.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005RenewResponseAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.WsTrustActions.RenewResponse,
                WsTrustConstants.TrustFeb2005.WsTrustActions.RenewFinal,
                WsTrustVersion.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005ValidateAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.WsTrustActions.ValidateRequest,
                WsTrustConstants.TrustFeb2005.WsTrustActions.ValidateFinal,
                WsTrustVersion.TrustFeb2005)
               .AsTask()
        ;

        public Task<Message> TrustFeb2005ValidateResponseAsync(Message request)
            => ProcessCoreAsync(
                request,
                WsTrustConstants.TrustFeb2005.WsTrustActions.ValidateResponse,
                WsTrustConstants.TrustFeb2005.WsTrustActions.ValidateFinal,
                WsTrustVersion.TrustFeb2005)
               .AsTask()
        ;
    }
}
