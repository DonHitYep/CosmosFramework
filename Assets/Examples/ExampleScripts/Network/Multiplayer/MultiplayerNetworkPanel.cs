﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
using Cosmos.Network;
using System.IO;
namespace Cosmos.Test
{
    public class MultiplayerNetworkPanel : UIResidentForm
    {
        Button btnConnect;
        Button btnDisconnect;
        InputField inputMsg;

        protected override void OnInitialization()
        {
            btnConnect = GetUIForm<Button>("BtnConnect");
            btnConnect.onClick.AddListener(ConnectClick);
            btnDisconnect = GetUIForm<Button>("BtnDisconnect");
            btnDisconnect.onClick.AddListener(DisconnectClick);
            inputMsg = GetUIForm<InputField>("InputMsg");
        }
        void ConnectClick()
        {
            MultiplayerManager.Instance.Connect();
        }
        void DisconnectClick()
        {
            MultiplayerManager.Instance.Disconnect();
        }
    }
}