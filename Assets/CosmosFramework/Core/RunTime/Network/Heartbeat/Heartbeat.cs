﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class Heartbeat : IHeartbeat
    {
        public long Conv { get; set; }
        /// <summary>
        /// 秒级别；
        /// 1代表1秒；
        /// </summary>
        public uint HeartbeatInterval { get; set; } 
        /// <summary>
        /// 秒级别；
        /// 上一次心跳时间；
        /// </summary>
        public long LatestHeartbeatTime { get; private set; }
        /// <summary>
        /// 是否存活
        /// </summary>
        public bool Available { get; private set; }
        /// <summary>
        /// 最大失效次数
        /// </summary>
        public byte MaxRecurCount { get; set; } 
        /// <summary>
        /// 失活时触发的委托；
        /// </summary>
        public Action UnavailableHandler { get; set; }
        /// <summary>
        /// 发送心跳的委托
        /// </summary>
        public Action<INetworkMessage> SendHeartbeatHandler { get; set; }
        /// <summary>
        /// 当前发送的心跳次数
        /// </summary>
        byte currentRecurCount;
        public Heartbeat(){}
        public Heartbeat(uint heartbeatInterval, byte maxRecurCount)
        {
            HeartbeatInterval = heartbeatInterval;
            MaxRecurCount = maxRecurCount;
        }
        public void OnActive()
        {
            LatestHeartbeatTime = Utility.Time.SecondNow() + HeartbeatInterval;
            currentRecurCount = 0;
            Available = true;
        }
        public void OnRefresh()
        {
            if (!Available)
                return;
            long now = Utility.Time.SecondNow();
            if (now <= LatestHeartbeatTime)
                return;
            LatestHeartbeatTime = now + HeartbeatInterval;
            currentRecurCount += 1;
            if (currentRecurCount >= MaxRecurCount)
            {
                Available = false;
                //UnavailableHandler?.Invoke();
                //TODO  心跳断开连接强耦合
                Facade.NetworkDisconnect(false);
                return;
            }
            SendHeartbeatHandler?.Invoke(UdpNetMessage.HeartbeatMessage(Conv));
            Utility.DebugLog($"Client heartbeat：Conv : {Conv} ; currentRecurCount : {currentRecurCount}",MessageColor.ORANGE);
        }
        public void OnRenewal()
        {
            long now = Utility.Time.SecondNow();
            LatestHeartbeatTime = now + HeartbeatInterval;
            currentRecurCount = 0;
            Utility.DebugLog($"Receive heartbeat ACK ：Conv : {Conv} ",MessageColor.INDIGO);
        }
        public void OnDeactive()
        {
            currentRecurCount = 0;
            //HeartbeatInterval = 0;
            LatestHeartbeatTime = 0;
            Available = false;
            Conv = 0;
        }
        public void Clear()
        {
            OnDeactive();
            UnavailableHandler = null;
        }
    }
}
