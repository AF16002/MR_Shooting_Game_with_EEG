using UnityEngine;
using System.Collections;
using System.Text;

using System.Threading;
using System.Collections.Generic;

#if UNITY_EDITOR
using System;
using System.Net;
using System.Net.Sockets;
#else
using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.IO;
using System.Threading.Tasks;
#endif



public class TCPReceiver : MonoBehaviour
    {

        //public static int Isconcentrate;
        private BufferManager BufferManager;
        //private DebugManager DebugManager;
        private bool isExecuted = true;

#if UNITY_UWP
        async void Start()
        {
            //バッファ用
            BufferManager = GameObject.Find("ValueFromDataProcessor").GetComponent<BufferManager>();
            //DebugManager = GameObject.Find("ValueFromDataProcessor").GetComponent<DebugManager>();



            try
            {
                //Create a StreamSocketListener to start listening for TCP connections.
                StreamSocketListener socketListener = new StreamSocketListener();

                //Hook up an event handler to call when connections are received.
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                //Start listening for incoming TCP connections on the specified port. You can specify any port that' s not currently in use.
                await socketListener.BindServiceNameAsync("18888");


                //DebugManager.BufferDebug.Enqueue("[TCPReceiver] Server is Listening ...");

            }
            catch (Exception e)
            {
                //Handle exception.
                isExecuted = false;
                //Debug.Log(e.ToString());
                //DebugManager.BufferDebug.Enqueue(e.ToString());

            }
        }

        private async void SocketListener_ConnectionReceived(StreamSocketListener sender,
        StreamSocketListenerConnectionReceivedEventArgs args)
        {
            //Read line from the remote client.
            //don''t use StreamReader
            //Stream =>byte  StreamReader =>char
            Stream Stream = args.Socket.InputStream.AsStreamForRead(); 
            //DebugManager.BufferDebug.Enqueue("[TCPReceiver] TCPConnection Succeeded!");

            while (isExecuted)
            {
                await ReadAsync(Stream);
                //DebugManager.BufferDebug.Enqueue("while ReadAsync");
            }
        }

        public async Task ReadAsync(Stream Reader)
        {

            //DebugManager.BufferDebug.Enqueue("ReadAsync");

            try
            {
                //DebugManager.BufferDebug.Enqueue("Task Try ReadAsync");
                Payload payload = Packet.GetPayloadFrom(ref Reader);

                //DebugManager.BufferDebug.Enqueue($"ReadAsync Payload : {payload.ToString()}" );
                BufferManager.PayloadBuffer.Enqueue(payload);

            //メインスレッドとは別のスレッドで動いてるから、UnityのAPIは使えません
            //つまりDebug.Logとかが使えないので
            //デバックしたければ、メインスレッドに持っていくべし
            if (payload.EndConnection)
                {
                    isExecuted = false;
                    //DebugManager.BufferDebug.Enqueue("[TCPReceiver] EndConnection");

                }              
            }
            catch (Exception e)
            {
                //Handle exception.
                //Debug.Log(e.ToString());
                //DebugManager.BufferDebug.Enqueue(e.ToString());
            }
        }


#endif
}