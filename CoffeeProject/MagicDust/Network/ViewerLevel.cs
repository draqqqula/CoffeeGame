﻿using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MagicDustLibrary.Display;

namespace MagicDustLibrary.Network
{
    public class ViewerLevel : GameLevel
    {
        public Dictionary<byte[], GameObject> NetworkCollection = new(new ByteKeyEqualityComparer());
        private IPAddress _adress;
        private int _port;
        private IPEndPoint _openAdress;
        private UdpClient _messageReciever;
        private volatile byte[] _actualData;
        private IUnpacker _messageUnpacker;
        private IUnpacker _stateUnpacker;
        private GameClient _mainClient;
        private MessageHandler _handler;

        private void SendClientInfo(GameClient client)
        {
            List<byte> buffer = new();
            buffer.AddRange(BitConverter.GetBytes(client.Window.X));
            buffer.AddRange(BitConverter.GetBytes(client.Window.Y));
            buffer.AddRange(BitConverter.GetBytes(client.Window.Width));
            buffer.AddRange(BitConverter.GetBytes(client.Window.Height));
            _messageReciever.SendAsync(buffer.ToArray(), _openAdress);
        }

        public void StartRecieveMessages()
        {
            _handler = new MessageHandler(_messageReciever);
            _handler.Start(CheckResult);
        }

        private void CheckResult(IPEndPoint host, byte[] data)
        {
            _actualData = data;
        }

        private void RecieveStateInfo()
        {
            var message = _messageReciever.ReceiveAsync().Result;
            _port = message.RemoteEndPoint.Port;
            _stateUnpacker.Unpack(message.Buffer);
        }

        protected override LevelSettings GetDefaults()
        {
            return new LevelSettings
            {
                CameraSettings = new CameraSettings()
            };
        }


        protected override void Initialize(IStateController state, LevelArgs arguments)
        {
            var IP = arguments.Data[0];
            _adress = IPAddress.Parse(Regex.Split(IP, ":")[0]);
            _port = int.Parse(Regex.Split(IP, ":")[1]);
            _openAdress = IPEndPoint.Parse(IP);
            _messageReciever = new UdpClient();

            //_stateUnpacker = new StateUnpacker(GameState, NetworkCollection, GameState.StateServices
            //    .GetService<StateLayerManager>()
            //    .GetLayer<UnpackLayer>());
            //_messageUnpacker = new MessageUnpacker(GameState, NetworkCollection);
        }

        protected override void OnClientUpdate(IStateController state, GameClient client)
        {
        }

        protected override void OnConnect(IStateController state, GameClient client)
        {
            SendClientInfo(client);
            RecieveStateInfo();
            StartRecieveMessages();
            _mainClient = client;
        }

        protected override void OnDisconnect(IStateController state, GameClient client)
        {
            _handler.Dispose();
            _messageReciever.Close();
            _messageReciever.Dispose();
        }

        protected override void Update(IStateController state, TimeSpan deltaTime)
        {
            if (_mainClient.Controls.OnAny())
            {
                var map = _mainClient.Controls.GetMap();
                _messageReciever.Send(new byte[1] { map }, new IPEndPoint(_adress, _port));
            }

            if (_actualData is null)
            {
                return;
            }

            _messageUnpacker.Unpack(_actualData);
        }
    }
}
