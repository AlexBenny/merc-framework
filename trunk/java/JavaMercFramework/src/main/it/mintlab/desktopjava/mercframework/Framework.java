package it.mintlab.desktopjava.mercframework;

import java.io.FileInputStream;
import java.io.IOException;
import java.lang.annotation.Annotation;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.ArrayList;
import java.util.Dictionary;
import java.util.HashMap;
import java.util.concurrent.locks.ReentrantReadWriteLock;
import java.util.logging.Level;
import java.util.logging.LogManager;
import java.util.logging.Logger;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.xpath.*;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import it.mintlab.desktopjava.tupleengine.*;

	/** 
	* Core class of MercFramework	 
	*/
	public class Framework implements IFramework
	{
		private static Logger logger;

		private NodeInterface nodeInterface;
		
		private MCDictionary<ArrayList<String>> messageMap; //messageContent <-> mercNameList (IS THREAD SAFE)
		private java.util.HashMap<String, IFDispatcher> mercMap; //mercName <-> dispatcherRef
		private java.util.HashMap<String, URI> remoteMercMap; //Remote MercName <-> URI
		
		private ReentrantReadWriteLock mercMapLock = new ReentrantReadWriteLock();
        private ReentrantReadWriteLock remoteMercMapLock = new ReentrantReadWriteLock();
        
		public Framework()
		{
			this(".\\mercConfig.xml");
		}

		public Framework(String confPath)
		{
			init(confPath);
		}

		/** 
		* Framework initialization
		*/
		private void init(String confPath)
		{
			// Logging Setting
			//Handler fh;
			try {
				LogManager logManager = LogManager.getLogManager();
				logManager.readConfiguration(new FileInputStream("./rsc/loggingMerc.properties"));
				//fh = new FileHandler("/Users/aggshow/desktop/mercframework.log", 10000, 3, true);			
				//logger.addHandler(fh);
			} catch (Exception e) {
				e.printStackTrace();
			}
			logger = Logger.getLogger(Framework.class.getName());
			
			logger.info("[Framework] Init");

			messageMap = new MCDictionary<java.util.ArrayList<String>>();
			mercMap = new HashMap<String, IFDispatcher>();
			remoteMercMap = new HashMap<String, URI>();

			java.util.HashMap<String, String> startMsgMap = new java.util.HashMap<String, String>();
//			WebClient updaterClient = new WebClient();
			DocumentBuilder builder;
			Document doc = null;
			XPathFactory factory = XPathFactory.newInstance();
			XPath xpath = factory.newXPath();
			
			try {
				builder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
				doc = builder.parse(confPath);
				
				// Setting Up Remote Interface from "config/framework-node"
				XPathExpression expr = xpath.compile("//config/framework-node");
				NodeList nodeList = (NodeList)expr.evaluate(doc, XPathConstants.NODESET);
				if (nodeList.getLength() != 0)
				{
					int port = Integer.parseInt(((Element)nodeList.item(0)).getAttribute("port"));
					nodeInterface = new NodeInterface(this, port);
					logger.info("[Framework] Started framework distribuited node on port: " + port);
				}
				
				// Register Local Mercs
				expr = xpath.compile("//config/mercs/merc");
				nodeList = (NodeList)expr.evaluate(doc, XPathConstants.NODESET);
				
				for (int i = 0; i < nodeList.getLength(); i++)
				{
					String mercName = ((Element)nodeList.item(i)).getAttribute("name");
					String mercClass = ((Element)nodeList.item(i)).getAttribute("class");
					
					// Register local mercs
					if(registerMerc(mercName, mercClass))
					{
						// Save start message
						if(((Element)nodeList.item(i)).hasAttribute("startmsg"))
						{
							startMsgMap.put(mercName, ((Element)nodeList.item(i)).getAttribute("startmsg"));
						}
					}
				}
				
				// Register Remote Mercs
				expr = xpath.compile("//config/mercs/remote-merc");
				nodeList = (NodeList)expr.evaluate(doc, XPathConstants.NODESET);
				
				for (int i = 0; i < nodeList.getLength(); i++)
				{
					String mercName = ((Element)nodeList.item(i)).getAttribute("name");
					String mercUri = ((Element)nodeList.item(i)).getAttribute("uri");
					remoteMercMap.put(mercName, new URI(mercUri));
				}
	
				// Send start messages for Merc that required it
				for(java.util.Map.Entry<String,String> pair : startMsgMap.entrySet())
				{
	
					try
					{
						deliverMessage(new Message(null, pair.getKey(), new MessageContent(MessageContent.Category.COMMAND, pair.getValue())));
					}
					catch(Exception e)
					{
						Term termException;
						Atom exAtom;
						if (e.getCause() == null)
						{
							exAtom = new Atom("ex_" + e.getClass());
							exAtom.setRefObject(e);
						}
						else
						{
							exAtom = new Atom("ex_" + e.getCause().getClass());
							exAtom.setRefObject(e.getCause());
						}
						java.util.ArrayList<Term> list = new java.util.ArrayList<Term>();
						list.add(exAtom);
						termException = new Compound("ex", list);
						deliverMessage(new Message("Framework", null, new MessageContent(MessageContent.Category.ERROR, termException)));
					}
				}
			} catch (SAXException e1) {
				// TODO Auto-generated catch block
				e1.printStackTrace();
			} catch (IOException e1) {
				// TODO Auto-generated catch block
				e1.printStackTrace();
			} catch (ParserConfigurationException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (XPathExpressionException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (URISyntaxException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}

		public final boolean deliverMessage(Message msg)
		{
			MessageContent mc = msg.getContent();

			// Logging if Exception message with ex(Exc) tuple
			if (mc.getCategory() == MessageContent.Category.ERROR)
			{
				if (!logExceptionMessage(msg))
                    return false;
			}
			
			// Looking for local recipients
            if (messageMap.containsKey(mc))
            {
                IFDispatcher recipientDispatcher;
                // if broadcast Message
                if (msg.getRecipient().equals(Message.BROADCAST_LOCAL) || msg.getRecipient().equals(Message.BROADCAST_ALL))
                {
                	for(String mercName : messageMap.get(mc))
                    {
                        mercMapLock.readLock().lock();
                        try { recipientDispatcher = mercMap.get(mercName); }
                        finally { mercMapLock.readLock().unlock(); }

                        recipientDispatcher.letMessage(msg);
                    }
                    if (!msg.getRecipient().equals(Message.BROADCAST_ALL)) // to send also remote
                        return true;    // delivery of broadcast message return true
                }
                // if Point To Point Message
                else
                {
                    // look if Merc is really interested
                    if (messageMap.get(mc).contains(msg.getRecipient()))
                    {
                        String recipientName = msg.getRecipient();
                        mercMapLock.readLock().lock();
                        try { recipientDispatcher = mercMap.get(recipientName); }
                        finally { mercMapLock.readLock().unlock(); }

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
            boolean isPresent;
            mercMapLock.readLock().lock();
            try { isPresent = remoteMercMap.containsKey(msg.getRecipient()); }
            finally { mercMapLock.readLock().unlock(); }
            if (isPresent)
            {
                URI mercUri;
                logger.info("[Framework] Remote sending of Message " + msg + " to " + msg.getRecipient());

                mercMapLock.readLock().lock();
                try { mercUri = remoteMercMap.get(msg.getRecipient()); }
                finally { mercMapLock.readLock().unlock();}

                NodeInterface.sendMessage(msg, mercUri );
                return true;
            }
            if(msg.getRecipient().equals(Message.BROADCAST_REMOTE) || msg.getRecipient().equals(Message.BROADCAST_ALL))
            {
                logger.info("[Framework] Remote broadcast sending of Message " + msg);
                ArrayList<URI> recipients = new ArrayList<URI>();
                mercMapLock.readLock().lock();
                try
                {
                    for (URI uri : remoteMercMap.values())
                    {
                        if (!recipients.contains(uri)) // avoid sending to same remote node
                        {
                            NodeInterface.sendMessage(new Message(msg.getSender(), Message.BROADCAST_LOCAL, msg.getContent()), uri);
                            recipients.add(uri);
                        }
                    }
                }
                finally { mercMapLock.readLock().unlock();}
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
            logger.warning("[Framework] No Merc answers to Message " + msg);
            return false;
		}

		public final void setMercMessages(java.util.ArrayList<MessageContent> messages, String mercName)
		{
			for (MessageContent id : messages)
			{
				if (messageMap.containsKey(id))
				{
					messageMap.get(id).add(mercName);
				}
				else
				{
					java.util.ArrayList<String> mercNameList = new java.util.ArrayList<String>();
					mercNameList.add(mercName);
					messageMap.put(id, mercNameList);
				}
			}
		}

		public final void destroyMercNode(String nodeName)
		{
			for (java.util.ArrayList<String> list : messageMap.values())
			{
				list.remove(nodeName);
			}
		}
		
		public void checkForNodeShutdown()
		{
			if (nodeInterface != null)
			{
				boolean oneAlive = false;
				mercMapLock.readLock().lock();
                try
                {
					for(IFDispatcher dispatcher : mercMap.values())
					{
						if (dispatcher.isMercAlive())
							oneAlive = true;
					}
                }
                finally { mercMapLock.readLock().unlock(); }
				// if no Survivor shutdown Node HTTP Interface
				if (!oneAlive)
					nodeInterface.shutdown();
			}
		}
		
		/**
		 * Register a new Merc in the framework at runtime
		 * @param mercName unique merc name
		 * @param className class and assembly names
		 * @return true if registration done, false otherwise. If it's already registered return true
		 */
        public boolean registerMerc(String mercName, String className) 
        {
        	try{
        		// Create Dispatcher for Merc
        		boolean alreadyPresent;
                mercMapLock.readLock().lock();
                try { alreadyPresent = mercMap.containsKey(mercName); }
                finally { mercMapLock.readLock().unlock(); }

                if (!alreadyPresent)
                {
                    // add merc dispatcher
                    IFDispatcher dispatcher = new Dispatcher(this, mercName, Class.forName(className));
                    logger.info("[Framework] Load Merc " + mercName);
                    mercMapLock.writeLock().lock();
                    try { mercMap.put(mercName, dispatcher); }
                    finally { mercMapLock.writeLock().unlock(); }
                }
                else
                {
                    logger.warning("[Framework] Merc with name " + mercName + " already exists");
                }
                return true;
        	} catch (ClassNotFoundException e) {
        	    logger.warning("[Framework] Class not exists for merc " + mercName);
	            return false;
        	}
        }
        
        /**
         * Deregister a present merc from framework at runtime
         * @param mercName unique merc name
         * @param mercMessages message merc names
         * @return true if deregistration done, false otherwise.
         */
        public boolean deregisterMerc(String mercName, ArrayList<MessageContent> mercMessages)
        {
            // remove merc messages
            for (MessageContent mc : mercMessages)
            {    
            	ArrayList<String> mercList = messageMap.get(mc);
                mercList.remove(mercName);
                // if it was last merc that it processed message, remove messageContent
                if(mercList.size() == 0)
                    messageMap.remove(mc);
            }
            
            // remove merc dispatcher
            IFDispatcher remResult;
            mercMapLock.writeLock().lock();
            try { remResult = mercMap.remove(mercName); }
            finally { mercMapLock.writeLock().unlock(); }
            if (remResult != null)
            {
                logger.info("[Framework] Unload Merc " + mercName);
                return true;
            }
            else
                return false;
        }

        /**
         * Register a remote Merc in the framework at runtime
        */
        public boolean registerRemoteMerc(String mercName, URI uri)
        {
            boolean alreadyPresent;
            remoteMercMapLock.readLock().lock();
            try { alreadyPresent = remoteMercMap.containsKey(mercName); }
            finally { remoteMercMapLock.readLock().unlock(); }

            if (!alreadyPresent)
            {
                logger.info("[Framework] Register Remote Merc " + mercName);
                remoteMercMapLock.writeLock().lock();
                try { remoteMercMap.put(mercName, uri); }
                finally { remoteMercMapLock.writeLock().unlock(); }
            }
            else
            {
                logger.warning("[Framework] Remote Merc with name " + mercName + " already exists");
            }
            return true;
        }

        /**
         * Deregister remote Merc from framework at runtime
         */
        public boolean deregisterRemoteMerc(String mercName)
        {
            URI remResult;
            remoteMercMapLock.writeLock();
            try { remResult = remoteMercMap.remove(mercName); }
            finally { remoteMercMapLock.writeLock().unlock();}
            if (remResult != null)
            {
                logger.info("[Framework] Deregister Remote Merc " + mercName);
                return true;
            }
            else
                return false;
        }
		
		/**
        * Logging Exception message with ex(Exc) tuple
        */
        private boolean logExceptionMessage(Message msg)
        {
            MessageContent mc = msg.getContent();
            if (mc.getCategory() == MessageContent.Category.ERROR)
            {
                try
                {
                    Term exTerm = TupleEngine.parse("ex(_ex)");
                    HashMap<String, Term> dict = new HashMap<String, Term>();

                    if (exTerm.unify(mc.getTuple(), dict))
                    {
                        Exception e = ((Exception)dict.get("_ex").getRefObject());
                        logger.severe("[Framework] Exception " + e.getMessage() + " throw by " + msg.getSender());
                    }
                    else
                    {
                    	logger.severe("[Framework] Invalid sending of exception message by " + msg.getSender());
                        return false;
                    }
                }
                catch (Exception ex)
                {
                	logger.severe("[Framework] Invalid sending of exception message by " + msg.getSender());
                    return false;
                }
            }
            return true;
        }

	}