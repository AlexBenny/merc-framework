using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using it.mintlab.mobilenet.tupleengine;
using System.Net.Sockets;


namespace it.mintlab.mobilenet.mercframework
{
    public class NodeInterface
    {
        private static TcpListener tcpListener;
        private static AutoResetEvent listenForNextRequest;
        private static IFramework mercFramework;
        private static Uri frameworkURI;

        public Uri FrameworkURI
        {
            get { return frameworkURI; }
            set { frameworkURI = value; }
        }
        
        public NodeInterface(IFramework framework, int port)
        {
            mercFramework = framework;
            IPAddress ip = (Dns.GetHostEntry(Dns.GetHostName()).AddressList)[0];
            frameworkURI = new Uri("http://" + ip.ToString() + ":" + port + "/");
            listenForNextRequest = new AutoResetEvent(false);
            // Create a listener.
            tcpListener = new TcpListener(port);
            //tcpListener.Prefixes.Add("http://+:" + port + "/");//"http://localhost:" + port + "/");
            //tcpListener.Start();
            // Move our listening loop off to a worker thread so that the caller doesn't lock up.
            //ThreadPool.QueueUserWorkItem(new WaitCallback(listen));
        }

        /* Loop here to begin processing of new requests.
        private void listen(object state)
        {
            while (tcpListener.IsListening)
            {
                tcpListener.BeginGetContext(new AsyncCallback(listenerCallback), tcpListener);
                listenForNextRequest.WaitOne();
            }
        }

        // Handle the processing of a request in here.
        private static void listenerCallback(IAsyncResult ar)
        {
            HttpListener hl = (HttpListener)ar.AsyncState;
            HttpListenerContext context = null;

            if (hl == null) return;

            try
            {
                // The EndGetContext() method, as with all Begin/End asynchronous methods in the .NET Framework,
                // blocks until there is a request to be processed or some type of data is available.
                context = hl.EndGetContext(ar);
            }
            catch (Exception ex)
            {
                // You will get an exception when httpListener.Stop() is called
                // because there will be a thread stopped waiting on the .EndGetContext()
                // method, and again, that is just the way most Begin/End asynchronous
                // methods of the .NET Framework work.
                //System.Diagnostics.Debug.WriteLine(ex.ToString());
                return;
            }
            finally
            {
                // Once we know we have a request (or exception), we signal the other thread
                // so that it calls the BeginGetContext() (or possibly exits if we're not
                // listening any more) method to start handling the next incoming request
                // while we continue to process this request on a different thread.
                listenForNextRequest.Set();
            }

            if (context == null) return;

            // Send merc message from http request to framework
            HttpListenerRequest request = context.Request;
            bool delivery = false;
            if (request.HasEntityBody)
            {
                using (StreamReader sr = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    delivery = mercFramework.deliverMessage(JsonLib.Json2MercMessage(sr.ReadToEnd()));
                }
            }

            // Response
            using (System.Net.HttpListenerResponse response = context.Response)
            {
                // Construct a response.
                string responseString;
                if (delivery)
                    responseString = JsonLib.JsonOkResponse();
                else
                    responseString = JsonLib.JsonKoResponse();
                
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.LongLength;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }*/

        public void shutdown()
        {
            //tcpListener.Stop();
        }
        
        public void sendMessage(Uri mercAddress, Message msg)
        {
            // Move our sending to a worker thread so that the caller doesn't lock up.
            mercFramework.logInfo("Feature not implemented yet: Trying to send remote message " + msg + ".");
            //ThreadPool.QueueUserWorkItem(new WaitCallback(send), new SendParams(mercAddress, msg));
        }
        /*
        private void send(object state){
            SendParams pars = (SendParams)state;
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(pars.mercAddress);
            
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = false;
            byte[] bytes = Encoding.ASCII.GetBytes(JsonLib.MercMessage2Json(pars.msg));
            Stream os = null;
            try
            {
                // send the Post
                httpWebRequest.ContentLength = bytes.Length;   //Count bytes to send
                os = httpWebRequest.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);         //Send it
            }
            catch (WebException ex)
            {
                deliverErrorToFramework(ex, pars.msg.getSender());
            }
            finally
            {
                if (os != null)
                {
                    os.Close();
                }
            }

            // get the response
            try
            {
                WebResponse webResponse = httpWebRequest.GetResponse();
                if (webResponse != null)
                {
                    StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                    if (JsonLib.wasDelivered(sr.ReadToEnd().Trim()))
                        mercFramework.logInfo("[*" + pars.msg.getRecipient() + "] Remote Message " + pars.msg.ToString() + " delivered");
                    else
                        mercFramework.logInfo("[*" + pars.msg.getRecipient() + "] Remote Message " + pars.msg.ToString() + " not delivered");
                }
                webResponse.Close();
            }
            catch (WebException ex)
            {
                deliverErrorToFramework(ex, pars.msg.getSender());
            }

        }

        private class SendParams
        {
            public Uri mercAddress;
            public Message msg;

            public SendParams(Uri mercAddress, Message msg)
            {
                this.mercAddress = mercAddress;
                this.msg = msg;
            }
        }*/

        private void deliverErrorToFramework(Exception e, String mercSender)
        {
            Term termException;
            Atom exAtom;
            if (e.InnerException == null)
            {
                exAtom = new Atom("ex_" + e.GetType());
                exAtom.setRefObject(e);
            }
            else
            {
                exAtom = new Atom("ex_" + e.InnerException.GetType());
                exAtom.setRefObject(e.InnerException);
            }
            List<Term> list = new List<Term>();
            list.Add(exAtom);
            termException = new Compound("ex", list);
            mercFramework.deliverMessage(new Message(mercSender, null, new MessageContent(MessageContent.Category.ERROR, termException)));
        }
    }
}
