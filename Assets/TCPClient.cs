using System.Collections;
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
    public class TCPClient : MonoBehaviour
    {
        public bool enable = false;
        public byte CurrentState;
        private byte clientState = 0;
        public float  messageCycleSendingTime = 1f; // 1s
        private float timerCache = 0;
        public string IpAddressToConnect = "localhost";
        public int port = 45545;
        private TcpClient Aclient;
        private NetworkStream AStream;
        protected TCPPackage pack = new TCPPackage();
        private Task TCPClientTask;
        private Thread currentThread;
        public float ClientThreadQuitDelayTime = 1f; // 1s delay to check if thread is close or not
        private float ClientThreadQuitDelayTimeCache;

        // Start is called before the first frame update
        private void Start()
        {
            clientState = 0;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            CurrentState = clientState;
            switch (clientState)
            {
                case 0:
                    if (enable)
                    {
                        TCPClientTask = Task.Run(ConnectToTCPServer);
                        clientState = 10;
                    }
                    break;
                case 10:
                    if (Aclient != null)
                    { 
                        if (Aclient.Connected)
                        {
                            TCPClientTask = Task.Run(WaitingMessage);
                            clientState = 20;
                        }
                    }
                    if (!enable && Aclient == null) clientState = 0;
                    break;
                case 20:
                    if (!TCPClientIsAlive.IsConnected(Aclient) || !enable)
                    {// check alive, if not
                        DisconnectFromTCPServer();
                        ClientThreadQuitDelayTimeCache = ClientThreadQuitDelayTime;
                        clientState = 30;
                    }
                    else
                    {// if connection alive then send message according to delay
                        if (timerCache > 0)
                        {
                            timerCache -= Time.deltaTime;
                            return;
                        }
                        else timerCache = messageCycleSendingTime;
                        string msgToSend = NewMessageToSend();
                        // send message if there are any
                        if (msgToSend != null) SendingMessage(msgToSend);

                    }
                    break;
                case 30:
                    if (TCPClientTask.Status == TaskStatus.RanToCompletion) clientState = 0;
                    ClientThreadQuitDelayTimeCache -= Time.deltaTime;
                    if (ClientThreadQuitDelayTimeCache <=0)
                    {
                        currentThread.Abort();
                        clientState = 40;
                    }
                    break;
                case 40:
                    Debug.Log(TCPClientTask.Status);
                    if (TCPClientTask.Status == TaskStatus.Faulted) clientState = 0;
                    break;
            }
        }
        private void ConnectToTCPServer()
        {
            try
            {
                Aclient = new TcpClient();
                Aclient.Connect(IpAddressToConnect, port);
                AStream = Aclient.GetStream();
            }
            catch (Exception e) 
            {
                Aclient = null;
                IfError(e.Message); 
            }
        }
        private void SendingMessage(string _msgIn)
        {
            try
            {
                if (!Aclient.Connected) return;
                BinaryWriter w = new BinaryWriter(AStream);
                w.Write(_msgIn);
                w.Flush();
            }
            catch (Exception e) { IfError(e.Message); }
        }
        private void WaitingMessage()
        {
            currentThread = Thread.CurrentThread;
            do
            {
                if (!Aclient.Connected) return;
                if (AStream.DataAvailable)
                    try
                    {
                        ReceivedNewMessage(new BinaryReader(AStream).ReadString());
                    }
                    catch (Exception e) { IfError(e.Message); }
            } while (enable);
        }

        private void DisconnectFromTCPServer()
        {
            pack.ContenctClearAll();    // clean all propared content if there are any

            enable = false;
            if (!Aclient.Connected) return;
            AStream.Close();
            Aclient.Close();    // if not alive then disconnect
            Aclient = null;

        }

        public virtual void ReceivedNewMessage(string _msgIn)
        {
            // transmission test code
            Debug.Log(_msgIn);
        }

        public virtual string NewMessageToSend()
        {
            // transmission test code
            pack.ContentAddNumber(10);
            pack.ContentAddNumber(20);
            pack.ContentAddString("aa");
            return pack.GetPackagedMessage();
        }

        public virtual void IfError (string _msg)
        {
            // not yet usable
            Debug.Log("Catch Error: " + _msg);
        }

    }
}