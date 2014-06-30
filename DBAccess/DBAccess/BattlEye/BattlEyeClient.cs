/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2013 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BattleNET
{
	public class BattlEyeClient
	{
		private Socket socket;
		private DateTime packetSent;
		private DateTime packetReceived;
		private BattlEyeDisconnectionType? disconnectionType;
		private bool keepRunning;
        private bool loginAccepted = false;
        private bool disconnectedAfter20s = false;
        private int sequenceNumber;
		private BattlEyeLoginCredentials loginCredentials;
		private Mutex mtxQueue = new Mutex();
		private SortedDictionary<int, string[]> packetQueue;
        private decimal reconnectDelay = 0;
        private Thread threadReceive = null;
        private Thread threadReconnect = null;
        private Thread threadKeepAlive = null;
        private LogType lastLog = LogType.None;

		public bool Connected
		{
			get
			{
				return socket != null && socket.Connected;
			}
		}

        public decimal ReconnectDelay
        {
            get
            {
                return reconnectDelay;
            }

            set
            {
                reconnectDelay = value;
                if (value > 0)
                {
                    if (threadReconnect == null)
                    {
                        threadReconnect = new Thread(ThreadReconnect);
                        threadReconnect.Start();
                    }
                }
                else
                {
                    threadReconnect = null;
                }
            }
        }
        
        public int CommandQueue
		{
			get
			{
				mtxQueue.WaitOne();
				int count = packetQueue.Count;
				mtxQueue.ReleaseMutex();
				return count;
			}
		}

		public BattlEyeClient(BattlEyeLoginCredentials loginCredentials)
		{
			this.loginCredentials = loginCredentials;
		}

		public BattlEyeConnectionResult Connect()
		{
			packetSent = DateTime.Now;
			packetReceived = DateTime.Now;

			sequenceNumber = 0;
			packetQueue = new SortedDictionary<int, string[]>();
			keepRunning = true;

			EndPoint remoteEP = new IPEndPoint(loginCredentials.Host, loginCredentials.Port);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.ReceiveBufferSize = Int32.MaxValue;
			socket.ReceiveTimeout = 5000;

			try
			{
				socket.Connect(remoteEP);

				if (SendLoginPacket(loginCredentials.Password) == BattlEyeCommandResult.Error)
					return BattlEyeConnectionResult.ConnectionFailed;

				var bytesReceived = new Byte[4096];
				int bytes = 0;

				bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);

				if (bytesReceived[7] == 0x00)
				{
					if (bytesReceived[8] == 0x01)
					{
						OnConnect(loginCredentials, BattlEyeConnectionResult.Success);

						loginAccepted = true;
                        disconnectedAfter20s = false;
					}
					else
					{
						OnConnect(loginCredentials, BattlEyeConnectionResult.InvalidLogin);
						return BattlEyeConnectionResult.InvalidLogin;
					}
				}
			}
			catch(Exception ex)
			{
				if (disconnectionType == BattlEyeDisconnectionType.ConnectionLost)
				{
                    if (MainWindow.IsDebug)
                        MessageBox.Show(ex.Message, "BATTLEYE EXCEPTION");
                    Disconnect(BattlEyeDisconnectionType.ConnectionLost);
					return BattlEyeConnectionResult.ConnectionFailed;
				}
				else
				{
					OnConnect(loginCredentials, BattlEyeConnectionResult.ConnectionFailed);
					return BattlEyeConnectionResult.ConnectionFailed;
				}
			}

            if (Connected)
            {
                if (threadReceive == null)
                {
                    threadReceive = new Thread(ThreadReceive);
                    threadReceive.Start();
                }

                if (threadKeepAlive == null)
                {
                    threadKeepAlive = new Thread(ThreadKeepAlive);
                    threadKeepAlive.Start();
                }
            }

			return BattlEyeConnectionResult.Success;
		}

		private BattlEyeCommandResult SendLoginPacket(string command)
		{
			try
			{
				if (!socket.Connected)
					return BattlEyeCommandResult.NotConnected;

				byte[] packet = ConstructPacket(BattlEyePacketType.Login, 0, command);
				socket.Send(packet);

				packetSent = DateTime.Now;
			}
			catch
			{
				return BattlEyeCommandResult.Error;
			}

			return BattlEyeCommandResult.Success;
		}

		private BattlEyeCommandResult SendAcknowledgePacket(string command)
		{
			try
			{
				if (!socket.Connected)
					return BattlEyeCommandResult.NotConnected;

				byte[] packet = ConstructPacket(BattlEyePacketType.Acknowledge, 0, command);
				socket.Send(packet);

				packetSent = DateTime.Now;
			}
			catch
			{
				return BattlEyeCommandResult.Error;
			}

			return BattlEyeCommandResult.Success;
		}

		public int SendCommand(string command, bool log = true)
		{
			return SendCommandPacket(command, log);
		}

		private int SendCommandPacket(string command, bool log = true)
		{
			int packetID = sequenceNumber;

			try
			{
				if (!socket.Connected)
					return 256;

				byte[] packet = ConstructPacket(BattlEyePacketType.Command, sequenceNumber, command);

				packetSent = DateTime.Now;

				if (log)
				{
					mtxQueue.WaitOne();
			        try
			        {
    					packetQueue.Add(sequenceNumber, new string[] { command, packetSent.ToString() });
                    }
                    catch
                    {
                    }
                    mtxQueue.ReleaseMutex();
				}

				socket.Send(packet);

				sequenceNumber = (sequenceNumber == 255) ? 0 : sequenceNumber + 1;
			}
			catch
			{
				return 256;
			}

			return packetID;
		}

		public int SendCommand(BattlEyeCommand command, string parameters = "")
		{
			return SendCommandPacket(command, parameters);
		}

		private int SendCommandPacket(BattlEyeCommand command, string parameters = "")
		{
			int packetID = sequenceNumber;

			try
			{
				if (!socket.Connected)
					return 256;

				byte[] packet = ConstructPacket(BattlEyePacketType.Command, sequenceNumber, Helpers.StringValueOf(command) + parameters);

				packetSent = DateTime.Now;

				mtxQueue.WaitOne();
                try
                {
                    packetQueue.Add(sequenceNumber, new string[] { Helpers.StringValueOf(command) + parameters, packetSent.ToString() });
                }
                catch
                {
                }
                mtxQueue.ReleaseMutex();

				socket.Send(packet);

				sequenceNumber = (sequenceNumber == 255) ? 0 : sequenceNumber + 1;
			}
			catch
			{
				return 256;
			}

			return packetID;
		}

		private byte[] ConstructPacket(BattlEyePacketType packetType, int sequenceNumber, string command)
		{
			string type;

			switch (packetType)
			{
				case BattlEyePacketType.Login:
					type = Helpers.Hex2Ascii("FF00");
					break;
				case BattlEyePacketType.Command:
					type = Helpers.Hex2Ascii("FF01");
					break;
				case BattlEyePacketType.Acknowledge:
					type = Helpers.Hex2Ascii("FF02");
					break;
				default:
					return new byte[] { };
			}

			if (packetType != BattlEyePacketType.Acknowledge)
			{
				if (command != null) command = Encoding.GetEncoding(1252).GetString(Encoding.UTF8.GetBytes(command));
			}

			string count = Helpers.Bytes2String(new byte[] { (byte)sequenceNumber });

			byte[] byteArray = new CRC32().ComputeHash(Helpers.String2Bytes(type + ((packetType != BattlEyePacketType.Command) ? "" : count) + command));

			string hash = new string(Helpers.Hex2Ascii(BitConverter.ToString(byteArray).Replace("-", "")).ToCharArray().Reverse().ToArray());

			string packet = "BE" + hash + type + ((packetType != BattlEyePacketType.Command) ? "" : count) + command;

			return Helpers.String2Bytes(packet);
		}

		public void Disconnect()
		{
			keepRunning = false;

            if (threadReconnect != null) threadReconnect.Abort();
            if (threadKeepAlive != null) threadKeepAlive.Abort();
            if (threadReceive != null) threadReceive.Abort();

            threadReconnect = null;
            threadKeepAlive = null;
            threadReceive = null;

			if (socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}

			OnDisconnect(loginCredentials, BattlEyeDisconnectionType.Manual);
		}

		private void Disconnect(BattlEyeDisconnectionType? disconnectionType)
		{
			if (disconnectionType == BattlEyeDisconnectionType.ConnectionLost)
				this.disconnectionType = BattlEyeDisconnectionType.ConnectionLost;

            if (loginAccepted == false)
            {
                Disconnect();
                return;
            }

            if (threadKeepAlive != null) threadKeepAlive.Abort();
            if (threadReceive != null) threadReceive.Abort();

            threadKeepAlive = null;
            threadReceive = null;

			if (socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}

			if (disconnectionType != null)
				OnDisconnect(loginCredentials, disconnectionType);
		}

		private void ThreadReceive()
		{
		    byte[] buffer = new byte[2048];
		    StringBuilder message = new StringBuilder();
		    int packetsTodo = 0;

            while(keepRunning)
            {
                try
                {
                    int bytesRead = socket.Receive(buffer, 0);
                    if (bytesRead > 0)
                    {
                        if (buffer[7] == 0x02)
                        {
                            SendAcknowledgePacket(Helpers.Bytes2String(new[] { buffer[8] }));
                            OnBattlEyeMessage(Helpers.Bytes2String(buffer, 9, bytesRead - 9), 256);
                        }
                        else if (buffer[7] == 0x01)
                        {
                            if (bytesRead > 9)
                            {
                                if (buffer[7] == 0x01 && buffer[9] == 0x00)
                                {
                                    if (buffer[11] == 0)
                                    {
                                        packetsTodo = buffer[10];
                                    }

                                    if (packetsTodo > 0)
                                    {
                                        message.Append(Helpers.Bytes2String(buffer, 12, bytesRead - 12));
                                        packetsTodo--;
                                    }

                                    if (packetsTodo == 0)
                                    {
                                        OnBattlEyeMessage(message.ToString(), buffer[8]);
                                        message = new StringBuilder();
                                        packetsTodo = 0;
                                    }
                                }
                                else
                                {
                                    // Temporary fix to avoid infinite loops with multi-packet server messages
                                    message = new StringBuilder();
                                    packetsTodo = 0;

                                    OnBattlEyeMessage(Helpers.Bytes2String(buffer, 9, bytesRead - 9), buffer[8]);
                                }
                            }

                            mtxQueue.WaitOne();
                            try
                            {
                                if (packetQueue.ContainsKey(buffer[8]))
                                {
                                    packetQueue.Remove(buffer[8]);
                                }
                            }
                            catch /*(Exception ex)*/
                            {
                            }
                            mtxQueue.ReleaseMutex();
                        }

                        packetReceived = DateTime.Now;
                    }
                }
                catch(Exception ex)
                {
                    if (socket.Connected)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    System.Diagnostics.Debug.Print(ex.Message);
                    Thread.Sleep(5000);
                }
            }
		}

		private void OnBattlEyeMessage(string message, int id)
		{
			if (BattlEyeMessageReceived != null)
				BattlEyeMessageReceived(new BattlEyeMessageEventArgs(message, id));
		}

		private void OnConnect(BattlEyeLoginCredentials loginDetails, BattlEyeConnectionResult connectionResult)
		{
            LogType logtype = (connectionResult == BattlEyeConnectionResult.Success) ? LogType.Connected : LogType.Disconnected;

            if (lastLog != logtype)
            {
                if (loginAccepted == false)
                    if (connectionResult == BattlEyeConnectionResult.ConnectionFailed || connectionResult == BattlEyeConnectionResult.InvalidLogin)
                        Disconnect(null);

                if (BattlEyeConnected != null)
                    BattlEyeConnected(new BattlEyeConnectEventArgs(loginDetails, connectionResult));

                lastLog = logtype;
            }
		}

		private void OnDisconnect(BattlEyeLoginCredentials loginDetails, BattlEyeDisconnectionType? disconnectionType)
		{
            if (lastLog != LogType.Disconnected)
            {
                if (BattlEyeDisconnected != null)
                    BattlEyeDisconnected(new BattlEyeDisconnectEventArgs(loginDetails, disconnectionType));

                lastLog = LogType.Disconnected;
            }
		}
        private void ThreadReconnect()
        {
            long remaining_ticks = (long)(ReconnectDelay * 10000000);

            keepRunning = true;

            while (keepRunning && (ReconnectDelay > 0))
            {
                long last_ticks = DateTime.Now.Ticks;
                Thread.Sleep(250);
                remaining_ticks -= (DateTime.Now.Ticks - last_ticks);

                if (remaining_ticks <= 0)
                {
                    remaining_ticks = (long)(ReconnectDelay * 10000000);

                    if (loginAccepted && !Connected)
                    {
                        Connect();
                    }
                }
            }
        }
        private void ThreadKeepAlive()
        {
            while (keepRunning)
            {
                int timeoutClient = (int)(DateTime.Now - packetSent).TotalSeconds;
                int timeoutServer = (int)(DateTime.Now - packetReceived).TotalSeconds;

                if (timeoutClient >= 5)
                {
                    if (timeoutServer >= 20 && !disconnectedAfter20s)
                    {
                        Disconnect(BattlEyeDisconnectionType.ConnectionLost);
                        keepRunning = true;
                        disconnectedAfter20s = true;
                    }
                    else
                    {
                        if (packetQueue.Count == 0)
                        {
                            SendCommandPacket(null, false);
                        }
                    }
                }

                mtxQueue.WaitOne();
                try
                {
                    if (socket.Connected && socket.Available == 0)
                    {
                        if (packetQueue.Count > 0)
                        {
                            int key = packetQueue.First().Key;
                            string value = packetQueue[key][0];
                            DateTime date = DateTime.Parse(packetQueue[key][1]);
                            int timeDiff = (int)(DateTime.Now - date).TotalSeconds;
                            if (timeDiff > 5)
                            {
                                SendCommandPacket(value, false);
                                packetQueue.Remove(key);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Prevent possible crash when packet is received at the same moment it's trying to resend it.
                    System.Diagnostics.Debug.Print(ex.Message);
                }
                mtxQueue.ReleaseMutex();

                Thread.Sleep(250);
            }

            if (!socket.Connected)
            {
                OnDisconnect(loginCredentials, BattlEyeDisconnectionType.ConnectionLost);
            }
        }

        private enum LogType
        {
            None,
            Connected,
            Disconnected
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
		public event BattlEyeConnectEventHandler BattlEyeConnected;
		public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;
	}

	public class StateObject
	{
		public Socket WorkSocket = null;
		public const int BufferSize = 2048;
		public byte[] Buffer = new byte[BufferSize];
		public StringBuilder Message = new StringBuilder();
		public int PacketsTodo = 0;
	}
}
