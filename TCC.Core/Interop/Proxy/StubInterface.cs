﻿using System.Threading.Tasks;
using TCC.Utils;

namespace TCC.Interop.Proxy
{
    public class StubInterface
    {
        private static StubInterface? _instance;
        public static StubInterface Instance => _instance ?? new StubInterface();

        public readonly StubClient StubClient;
        private readonly RpcServer StubServer;
        private readonly StubMessageHandler MessageHandler;

        public bool IsFpsModAvailable { get; set; }
        public bool IsStubAvailable { get; set; }

        private StubInterface()
        {
            _instance = this;

            StubServer = new RpcServer();
            StubClient = new StubClient();
            MessageHandler = new StubMessageHandler();
            
        }

        public async Task Init()
        {
            StubServer.RequestReceived += MessageHandler.HandleRequest;
            StubServer.ResponseReceived += MessageHandler.HandleResponse;

            if (!App.Settings.EnableProxy) return;
            IsStubAvailable = await StubClient.PingStub();
            if (!IsStubAvailable)
            {
                Log.F("Stub not found");
                return;
            }
            StubClient.Initialize();
            StubServer.Start();
            IsFpsModAvailable = await StubClient.GetIsModAvailable("fps-utils") || await StubClient.GetIsModAvailable("fps-manager");
        }

        public void Disconnect()
        {
            StubServer.Stop();
            StubServer.RequestReceived -= MessageHandler.HandleRequest;
            StubServer.ResponseReceived -= MessageHandler.HandleResponse;

        }
    }
}