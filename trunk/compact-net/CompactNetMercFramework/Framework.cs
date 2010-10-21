using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Reflection;
using it.mintlab.mobilenet.tupleengine;
using log4net;

namespace it.mintlab.mobilenet.mercframework
{
    /// <summary>
    /// Core class of MercFramework
    /// </summary>
    public class Framework : IFramework
    {
        // Create a logger for use in this class
        private static readonly ILog log = LogManager.GetLogger(typeof(Framework));
        // NOTE that using System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        // is equivalent to typeof(LoggingExample) but is more portable
        // i.e. you can copy the code directly into another class without
        // needing to edit the code.

        private NodeInterface nodeInterface;
        private MCDictionary<List<string>> messageMap; //messageContent <-> mercNameList (IS THREAD SAFE)
        private Dictionary<string, IFDispatcher> mercMap; //mercName <-> dispatcherRef
        private Dictionary<string, Uri> remoteMercMap; //Remote MercName <-> URI
        private Object loggerLock = new Object();
        private int nodePort;
        private String nodePublicHost = String.Empty;

        public Framework()
            : this(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\mercConfig.xml")
        {}

        public Framework(string confPath)
        {
            // Uncomment the next line to enable log4net internal debugging
            // log4net.Util.LogLog.InternalDebugging = true;

            // This will instruct log4net to look for a configuration file
            // called config.log4net in the root directory of the device
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\log.config"));

            //Framework Initialization
            init(confPath);
        }

        /// <summary>
        /// Framework initialization
        /// </summary>
        private void init(string confPath)
        {
            logInfo("[Framework] Init");

            messageMap = new MCDictionary<List<string>>();
            mercMap = new Dictionary<string, IFDispatcher>();
            remoteMercMap = new Dictionary<string, Uri>();

            Dictionary<string, string> startMsgMap = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(confPath);

            // Setting Up Remote Interface
            XmlNodeList nodes = doc.SelectNodes("config/framework-node");
            if (nodes.Count != 0)
            {
                nodePort = Int32.Parse(nodes[0].Attributes.GetNamedItem("port").Value);
                if (nodes[0].Attributes.GetNamedItem("public-host") != null)
                    nodePublicHost = nodes[0].Attributes.GetNamedItem("public-host").Value;
                nodeInterface = new NodeInterface(this, nodePort);
                logInfo("[Framework] Started framework distribuited node on port: " + nodePort);
            }

            // Setting Up Remote Interface
            /*Updater updater = new Updater();
            nodes = doc.SelectNodes("config/update-center");
            if (nodes.Count != 0)
            {
                try
                {
                    nodes = doc.SelectSingleNode("config/update-center").ChildNodes;
                    foreach (XmlNode node in nodes)
                    {
                        switch (node.LocalName)
                        {
                            case Updater.UPDATE_MODE_RESTJSON:
                                break;
                            case Updater.UPDATE_MODE_FILELIST:
                                updater.setTextList(new Uri(node.Attributes["uri"].Value));
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    logError(e.Message, e);
                }
            }*/

            // Register Local Mercs
            nodes = doc.SelectNodes("config/mercs/merc");
            foreach (XmlNode node in nodes)
            {
                string mercName = node.Attributes.GetNamedItem("name").Value;
                /*if (mercName.IndexOf("%machinename%") == -1)
                    mercName = mercName.Replace("%machinename%", System.Environment.MachineName);*/
                string mercClass = node.Attributes.GetNamedItem("class").Value;
                string updatable;
                if (node.Attributes.GetNamedItem("updatable") != null)
                    updatable = node.Attributes.GetNamedItem("updatable").Value;
                else
                    updatable = "false";

                /* Try to update merc
                if (updatable.Equals("true"))
                {
                    try
                    {
                        String mercDllName = mercClass.Substring(mercClass.IndexOf(',') + 1).Trim() + ".dll";
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo("./" + mercDllName);
                        if (updater.updateMerc(mercClass.Substring(0, mercClass.IndexOf(',')), new Version(fvi.ProductVersion)))
                            logInfo("[Framework] Updated Merc " + mercName + " getting new " + mercDllName);
                    }
                    catch (Exception e)
                    {
                        logError(e.Message, e);
                    }
                }*/

                // Register local mercs
                if (registerMerc(mercName, mercClass))
                {
                    // Save start message
                    if (node.Attributes.GetNamedItem("startmsg") != null)
                        startMsgMap.Add(mercName, node.Attributes.GetNamedItem("startmsg").Value);
                }
            }

            // Register remote mercs
            nodes = doc.SelectNodes("config/mercs/remote-merc");
            foreach (XmlNode node in nodes)
            {
                string mercName = node.Attributes.GetNamedItem("name").Value;
                string mercUri = node.Attributes.GetNamedItem("uri").Value;
                this.registerRemoteMerc(mercName, new Uri(mercUri));
            }

            // Send start messages for Merc that required it
            foreach (KeyValuePair<string, string> pair in startMsgMap)
            {

                try
                {
                    deliverMessage(new Message(null, pair.Key, new MessageContent(MessageContent.Category.COMMAND, pair.Value)));
                }
                catch (Exception e)
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
                    deliverMessage(new Message("Framework", null, new MessageContent(MessageContent.Category.ERROR, termException)));
                }
            }
        }

        /// <summary>
        /// Framework initialization
        /// </summary>
        /*private void init2(string confPath)
        {
            log.Info("[Framework] Init");
            //Console.WriteLine("enabled? " + log.IsErrorEnabled);

            messageMap = new MCDictionary<List<string>>();
            mercMap = new Dictionary<string, IFDispatcher>();

            XmlDocument doc = new XmlDocument();

            doc.Load(confPath);
            XmlNodeList nodes = doc.SelectNodes("mercs/merc");
            foreach (XmlNode node in nodes)
            {
                string mercName = node.Attributes.GetNamedItem("name").Value;
                string mercClass = node.Attributes.GetNamedItem("class").Value;

                if (Type.GetType(mercClass) == null)
                    log.Warn("[Framework] Class not exists for merc " + mercName);
                else
                {
                    if (!mercMap.ContainsKey(mercName))
                    {
                        log.Info("[Framework] Load merc " + mercName);
                        mercMap.Add(mercName, new Dispatcher(this, mercName, Type.GetType(mercClass)));
                    }
                    else
                    {
                        log.Warn("[Framework] Merc with name " + mercName + " already exists");
                    }
                }

                //Send start message for merc if exist
                if (node.Attributes.GetNamedItem("startmsg") != null)
                {
                    try
                    {
                        deliverMessage(new Message(null, mercName, new MessageContent(MessageContent.Category.COMMAND, node.Attributes.GetNamedItem("startmsg").Value)));
                    }
                    catch (Exception e)
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
                        deliverMessage(new Message("Framework", null, new MessageContent(MessageContent.Category.ERROR, termException)));
                    }
                }
            }
        }*/

#region IFramework Members

        /// <summary>
        /// Deliver a message from localMerc to any Merc
        /// </summary>
        /// <param name="msg">Message to deliver</param>
        /// <returns>return true if message was delivered</returns>
        public bool deliverMessage(Message msg)
        {
            MessageContent mc = msg.getContent();

            // Logging if Exception message with ex(Exc) tuple
            if (mc.getCategory() == MessageContent.Category.ERROR)
            {
                if (!logExceptionMessage(msg))
                    return false;
            }

            // Looking for local recipients
            if (messageMap.ContainsKey(mc))
            {
                IFDispatcher recipientDispatcher;
                // if broadcast Message
                if (msg.getRecipient().Equals(Message.BROADCAST_LOCAL) || msg.getRecipient().Equals(Message.BROADCAST_ALL))
                {
                    foreach (string mercName in messageMap[mc])
                    {
                        recipientDispatcher = mercMap[mercName];
                        recipientDispatcher.letMessage(msg);
                    }
                    if (!msg.getRecipient().Equals(Message.BROADCAST_ALL)) // to send also remote
                        return true;    // delivery of broadcast message return true
                }
                // if Point To Point Message
                else
                {
                    // look if Merc is really interested
                    if (messageMap[mc].Contains(msg.getRecipient()))
                    {
                        string recipientName = msg.getRecipient();
                        recipientDispatcher = mercMap[recipientName];
                        recipientDispatcher.letMessage(msg);
                        return true;
                    }
                    // Looking for remote recipients -- AFTER
                    //else if (remoteMercMap.ContainsKey(msg.getRecipient()))
                    //{
                    //    log.Info("[Framework] Remote sending of Message " + msg);
                    //    nodeInterface.sendMessage(msg, remoteMercMap[msg.getRecipient()]);
                    //    return true;
                    //}
                    //else
                    //{
                    //    log.Warn("[Framework] Message " + mc + " send by " + msg.getSender() + " is not interesting for " + msg.getRecipient());
                    //    return false;
                    //}
                }
            }

            // Looking for remote recipients
            bool isPresent;
            isPresent = remoteMercMap.ContainsKey(msg.getRecipient());
            if (isPresent)
            {
                Uri mercUri;
                logInfo("[Framework] Remote sending of Message " + msg + " to " + msg.getRecipient());

                mercUri = remoteMercMap[msg.getRecipient()];

                nodeInterface.sendMessage(mercUri, msg);
                return true;
            }
            if (msg.getRecipient().Equals(Message.BROADCAST_REMOTE) || msg.getRecipient().Equals(Message.BROADCAST_ALL))
            {
                logInfo("[Framework] Remote broadcast sending of Message " + msg);
                List<Uri> recipients = new List<Uri>();
                foreach (Uri uri in remoteMercMap.Values)
                {
                    if (!recipients.Contains(uri)) // avoid sending to same remote node
                    {
                        nodeInterface.sendMessage(uri, new Message(msg.getSender(), Message.BROADCAST_LOCAL, msg.getContent()));
                        recipients.Add(uri);
                    }
                }
                return true;    // delivery of broadcast message return true
            }
            //if (msg.getRecipient().Equals(Message.BROADCAST_REMOTE_MULTIHOP))
            //{
            //    if(remoteMercMap.ContainsKey(msg.getSender()))
            //    {
            //        senderUri = remoteMercMap[msg.getSender()];
            //    }
            //    log.Info("[Framework] Remote broadcast sending of Message " + msg);
            //    List<Uri> recipients = new List<Uri>();
            //    foreach (Uri uri in remoteMercMap.Values)
            //    {
            //        if (!recipients.Contains(uri) && !uri.Equals(senderUri)) // avoid sending to same remote node and sender node
            //        {
            //            nodeInterface.sendMessage(msg, uri);
            //            recipients.Add(uri);
            //        }
            //    }
            //    return true;    // delivery of broadcast message return true
            //}

            // ...if no already sent 
            logInfo("[Framework] No Merc answers to Message " + msg);  // WARNING
            return false;
        }

        public void setMercMessages(List<MessageContent> messages, string mercName)
        {
            foreach (MessageContent id in messages)
                if (messageMap.ContainsKey(id))
                    messageMap[id].Add(mercName);
                else
                {
                    List<string> mercNameList = new List<string>();
                    mercNameList.Add(mercName);
                    messageMap.Add(id, mercNameList);
                }
        }

        public void checkForNodeShutdown()
        {
            if (nodeInterface != null)
            {
                bool oneAlive = false;
                foreach (IFDispatcher dispatcher in mercMap.Values)
                {
                    if (dispatcher.isMercAlive())
                        oneAlive = true;
                }
                // if no Survivor shutdown Node HTTP Interface
                if (!oneAlive)
                    nodeInterface.shutdown();
            }
        }

        public void forceNodeShutdown()
        {
            foreach (IFDispatcher dispatcher in mercMap.Values)
            {
                if (dispatcher.isMercAlive())
                    dispatcher.killMerc();
            }
            if (nodeInterface != null)
                nodeInterface.shutdown();
        }

        /// <summary>
        /// Register a new Merc in the framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <param name="className">class and assembly names</param>
        /// <returns>true if registration done, false otherwise. If it's already registered return true</returns>
        public bool registerMerc(string mercName, string className)
        {
            // Create Dispatcher for Merc
            if (Type.GetType(className) == null)
            {
                logInfo("[Framework] Class not exists for merc " + mercName);   // WARNING
                return false;
            }
            else
            {
                bool alreadyPresent;
                alreadyPresent = mercMap.ContainsKey(mercName);
                
                if (!alreadyPresent)
                {
                    logInfo("[Framework] Load Merc " + mercName);
                    // add merc dispatcher
                    IFDispatcher dispatcher = new Dispatcher(this, mercName, Type.GetType(className));
                    mercMap.Add(mercName, dispatcher);
                }
                else
                {
                    logInfo("[Framework] Merc with name " + mercName + " already exists"); // WARNING
                }
                return true;
            }
        }

        /// <summary>
        /// Deregister a present merc from framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <param id="messages">message merc names</param>
        /// <returns>true if deregistration done, false otherwise.</returns>
        public bool deregisterMerc(string mercName, List<MessageContent> mercMessages)
        {
            // remove merc messages
            foreach (MessageContent mc in mercMessages)
            {
                List<string> mercList = messageMap[mc];
                mercList.Remove(mercName);
                // if it was last merc that it processed message, remove messageContent
                if (mercList.Count == 0)
                    messageMap.Remove(mc);
            }

            // remove merc dispatcher
            bool remResult;
            remResult = mercMap.Remove(mercName);
            if (remResult)
            {
                logInfo("[Framework] Unload Merc " + mercName);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Register a remote Merc in the framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <param name="uri">remote framework uri</param>
        /// <returns>true if registration done, false otherwise</returns>
        public bool registerRemoteMerc(string mercName, Uri uri)
        {
            bool alreadyPresent;
            alreadyPresent = remoteMercMap.ContainsKey(mercName);
            
            if (!alreadyPresent)
            {
                logInfo("[Framework] Register Remote Merc " + mercName);
                remoteMercMap.Add(mercName, uri);
            }
            else
            {
                logInfo("[Framework] Remote Merc with name " + mercName + " already exists");   // WARNING
            }
            return true;
        }

        /// <summary>
        /// Deregister remote Merc from framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <returns>true if deregistration done, false otherwise.</returns>
        public bool deregisterRemoteMerc(string mercName)
        {
            bool remResult;
            remResult = remoteMercMap.Remove(mercName);
            if (remResult)
            {
                logInfo("[Framework] Deregister Remote Merc " + mercName);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Get the URI where framework listen remote request.
        /// </summary>
        public Uri getFrameworkURI()
        {
            if (nodePublicHost.Equals(String.Empty))
                return nodeInterface.FrameworkURI;
            else
            {
                return new Uri(nodeInterface.FrameworkURI.Scheme + "://" + nodePublicHost + ":" + nodeInterface.FrameworkURI.Port + "/");
            }
        }

#endregion

        /// <summary>
        /// Logging Exception message with ex(Exc) tuple
        /// </summary>
        /// <returns>false if exc message malformed</returns>
        private bool logExceptionMessage(Message msg)
        {
            MessageContent mc = msg.getContent();
            if (mc.getCategory() == MessageContent.Category.ERROR)
            {
                try
                {
                    Term exTerm = TupleEngine.parse("ex(_ex)");
                    Dictionary<string, Term> dict = new Dictionary<string, Term>();

                    if (exTerm.unify(mc.getTuple(), dict))
                    {
                        Exception e = ((Exception)dict["_ex"].getRefObject());
                        logError("[Framework] Exception " + e.Message + " throw by " + msg.getSender(), e);
                    }
                    else
                    {
                        logError("[Framework] Invalid sending of exception message by " + msg.getSender(), null);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    logError("[Framework] Invalid sending of exception message by " + msg.getSender(), ex);
                    return false;
                }
            }
            return true;
        }

        public void logInfo(string logMsg)
        {
            lock (loggerLock) { try { log.Info(logMsg); } catch { } }
        }

        public void logError(string logMsg, Exception exception)
        {
            if (exception != null)
                lock (loggerLock) { try { log.Error(logMsg, exception); } catch { } }
            else
                lock (loggerLock) { try { log.Error(logMsg); } catch { } }
        }
    }
}
