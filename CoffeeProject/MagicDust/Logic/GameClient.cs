using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MagicDustLibrary.Network;

namespace MagicDustLibrary.Logic
{
    /// <summary>
    /// Обозначает игрока.<br/>
    /// <list>
    /// <item>Для <b>одиночной игры</b> всегда один и тот же.</item>
    /// <item>Для <b>многопользовательской игры</b> свой для каждого подключенного игрока.</item>
    /// </list>
    /// </summary>
    public class GameClient : IDisposable
    {
        public enum GameLanguage
        {
            Russian,
            English
        }

        public readonly GameLanguage Language;
        public readonly GameControls Controls;
        public Rectangle Window { get; set; }
        public readonly bool IsRemote;
        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _remoteHost;
        private readonly MessageHandler _handler;

        #region CONSTRUCTORS
        private GameClient(Rectangle window, GameControls controls, GameLanguage language, bool isRemote, IPEndPoint remoteHost)
        {
            Window = window;
            Controls = controls;
            Language = language;
            IsRemote = isRemote;
            if (IsRemote)
            {
                _udpClient = new UdpClient(0);
                _remoteHost = remoteHost;
                CreateRemoteControls();
                _handler = new MessageHandler(_udpClient);
                _handler.Start(HandleData);
            }
        }

        public GameClient(Rectangle window, GameControls controls, GameLanguage language) :
        this(window, controls, language, false, null)
        {
        }

        public GameClient(Rectangle window, GameControls controls, GameLanguage language, IPEndPoint remoteHost) :
        this(window, controls, language, true, remoteHost)
        {
        }

        public GameClient(Rectangle window, GameControls controls, GameLanguage language, string adress) :
        this(window, controls, language, true, IPEndPoint.Parse(adress))
        {
        }
        #endregion

        #region NETWORK
        public void SendData(byte[] data)
        {
            if (IsRemote)
                _udpClient.SendAsync(data, _remoteHost);
            else
                throw new Exception("Non-Remote client unable to send data to remote host");
        }


        private bool[] ControlsMap = new bool[8];

        private void HandleData(IPEndPoint host, byte[] data)
        {
            bool[] controlsMap = GetControlMap(data[0], Enum.GetValues<Control>().Count());
            for (byte i = 0; i < controlsMap.Length; i++)
            {
                ControlsMap[i] = controlsMap[i];
            }
        }

        private bool[] GetControlMap(byte data, int length)
        {
            bool[] boolArray = new bool[length];

            for (int i = 0; i < length; i++)
            {
                boolArray[i] = (data & (1 << i)) != 0;
            }

            return boolArray;
        }
        public void CreateRemoteControls()
        {
            Controls.ChangeControl(Control.left, () => ControlsMap[0]);
            Controls.ChangeControl(Control.right, () => ControlsMap[1]);
            Controls.ChangeControl(Control.jump, () => ControlsMap[2]);
            Controls.ChangeControl(Control.dash, () => ControlsMap[3]);
            Controls.ChangeControl(Control.pause, () => ControlsMap[4]);
            Controls.ChangeControl(Control.lookUp, () => ControlsMap[5]);
            Controls.ChangeControl(Control.lookDown, () => ControlsMap[6]);
        }

        #endregion

        public void Dispose()
        {
            if (_udpClient is not null)
            {
                _udpClient.Dispose();
                OnDispose(this);
            }
        }
        public Action<GameClient> OnUpdate = delegate { };
        public Action<GameClient> OnDispose = delegate { };
    }
}
