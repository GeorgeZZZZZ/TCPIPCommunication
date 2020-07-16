﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System;
using System.Threading.Tasks;


namespace Georgescript
{
    public class TCPServer : MonoBehaviour
    {
        public bool enable = false;
        public int port = 45545;
        public byte CurrentState;
        private byte tcpMainState = 0;
        public float ServerThreadQuitDelayTime = 1f; // 1s delay to check if thread is close or not
        private float ServerThreadQuitDelayTimeCache;
        private TCPUnpackage unpack = new TCPUnpackage();
        private TCPPackage pack = new TCPPackage();
        private Task TCPServerTask;
        private Thread currentThread;
        public byte CheckAliveAllowTotalFailTime = 3;
        private byte CheckAliveFailCache;
        public float CheckAliveDelayTime = 1f;

        // Start is called before the first frame update
        public virtual void Start()
        {
            tcpMainState = 0;
        }

        private void OnDisable()
        {
            if (TCPServerTask != null && TCPServerTask.Status == TaskStatus.Running && currentThread.ThreadState != ThreadState.Unstarted)
            {
                currentThread.Abort();
            }
        }

        public virtual void Update()
        {
            CurrentState = tcpMainState;
            switch (tcpMainState)
            {
                case 0:
                    if (enable)
                    {
                        TCPServerTask = Task.Run(MyTCPServer);
                        tcpMainState = 10;
                    }
                    break;
                case 10:
                    if (!enable)
                    {
                        ServerThreadQuitDelayTimeCache = ServerThreadQuitDelayTime;
                        tcpMainState = 20;
                    }
                    break;
                case 20:
                    // check if server is finished in time or going to next state
                    if (TCPServerTask.Status == TaskStatus.RanToCompletion) tcpMainState = 0;
                    ServerThreadQuitDelayTimeCache -= Time.deltaTime;
                    if (ServerThreadQuitDelayTimeCache <= 0)
                    {
                        tcpMainState = 30;
                    }
                    break;
                case 30:
                    currentThread.Abort();
                    tcpMainState = 40;
                    break;
                case 40:
                    if (TCPServerTask.Status == TaskStatus.Faulted)
                        tcpMainState = 0;
                    break;
            }

        }
        private void MyTCPServer()
        {
            try
            {
                currentThread = Thread.CurrentThread;
                TcpListener Mylistener = new TcpListener(IPAddress.Any, port);
                Mylistener.Start();
                bool clientAlive = false;
                System.Timers.Timer aliveTimer = new System.Timers.Timer();
                aliveTimer.Interval = CheckAliveDelayTime *1000;    //timer is using ms
                aliveTimer.Elapsed += AliveTimer;
                do
                {
                    using (TcpClient c = Mylistener.AcceptTcpClient())
                    using (NetworkStream netStream = c.GetStream())
                    {
                        CheckAliveFailCache = 0;
                        clientAlive = true;
                        // start looping waitting for new message
                        do
                        {
                            // check if connection is alive
                            //clientAlive = TCPClientIsAlive.IsConnected(c);

                            if (!aliveTimer.Enabled) aliveTimer.Start();
                            if (AliveTimerReached)
                            {
                                AliveTimerReached = false;
                                // check alive, if not
                                if (!TCPClientIsAlive.IsConnected(c)) CheckAliveFailCache++;
                                else CheckAliveFailCache = 0;
                            }

                            if (CheckAliveFailCache >= CheckAliveAllowTotalFailTime) clientAlive = false;

                            if (clientAlive && enable && netStream.DataAvailable)
                            {
                                string msgIn = new BinaryReader(netStream).ReadString();
                                BinaryWriter write = new BinaryWriter(netStream);
                                string response;
                                if (unpack.MessageIn(msgIn, out List<float> outNum, out List<string> outStr))
                                {
                                    ReceivedMessage(outNum, outStr);
                                    response = pack.GetDoneMessage();
                                }
                                else
                                {
                                    response = pack.GetNotMatchMessage();
                                }
                                write.Write(response);

                                write.Flush();
                            }
                        } while (clientAlive && enable);
                        aliveTimer.Stop();
                    }
                } while (enable);
                aliveTimer.Dispose();
                Mylistener.Stop();
            }
            catch (Exception e)
            {
                if (e.Message.Length > 2) IfError(e.Message);
            }
        }
        private bool AliveTimerReached;
        private void AliveTimer(object sender, EventArgs e)
        {
            AliveTimerReached = true;
        }

        public virtual void ReceivedMessage(List<float> _num, List<string> _str)
        {
            // check commucation
            foreach (var n in _num) Debug.Log("num: " + n);
            foreach (var n in _str) Debug.Log("num: " + n);
        }

        public virtual void IfError(string _msg)
        {
            Debug.Log("Catch Error: " + _msg);
        }

    }
}
