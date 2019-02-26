using RemotePlusClient.ViewModels;
using RemotePlusLibrary.Core;
using RemotePlusLibrary.Core.IOC;
using RemotePlusLibrary.RequestSystem;
using RemotePlusLibrary.RequestSystem.DefaultRequestBuilders;
using RemotePlusLibrary.RequestSystem.DefaultUpdateRequestBuilders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotePlusClient.Requests
{
    class ProgressRequest : StandordRequest<ProgressRequestBuilder, ProgressUpdateBuilder>
    {
        public override string URI => "r_progress";
        public override bool ShowProperties => false;
        public override string FriendlyName => "Console Progress Request";
        public override string Description => "Provdes a console implementation for the progress request.";
        public override NetworkSide SupportedSides => NetworkSide.Client;
        private Guid _server = Guid.Empty;
        public override RawDataResponse RequestData(ProgressRequestBuilder builder, NetworkSide executingSide)
        {
            _server = builder.RequestingServer;
            Task.Run(() => IOCContainer.GetService<IWindowManager>().GetAllByID<ConsoleViewModel>(builder.RequestingServer).ToList().ForEach(f => f.ViewModel.LongOperationStarted(builder.Message, builder.Maximum)));
            return RawDataResponse.Success(null);
        }
        public override void Update(ProgressUpdateBuilder message)
        {
            IOCContainer.GetService<IWindowManager>().GetAllByID<ConsoleViewModel>(message.RequestingServer).ToList().ForEach(f => f.ViewModel.UpdateLongOperation(message.NewValue, message.Text));
        }
        public override void Dispose()
        {
            IOCContainer.GetService<IWindowManager>().GetAllByID<ConsoleViewModel>(_server).ToList().ForEach(f => f.ViewModel.FinishedLongOperation());
        }
    }
}