package it.mintlab.desktopjava.mercframework;

import it.mintlab.desktopjava.tupleengine.*;

import java.io.FileInputStream;
import java.lang.annotation.Annotation;
import java.lang.reflect.Constructor;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.logging.LogManager;
import java.util.logging.Logger;

	public class Dispatcher implements IMDispatcher, IFDispatcher, Runnable
	{
		private static Logger logger;

		private IFramework framework;
		private String mercName;
		private Class<?> mercClass;

		private boolean live;
		private MessageQueue mq;
		private IMerc merc;
		private MCDictionary<HashMap<String,Method>> bindingMap; // MessageContent <-> (State <-> Method)

		private Object lifeGivingLock = new Object(); //to prevent multiple threads creation

		/** 
		* Costructor
		* 
		* <param id="mercName">
		* <param id="mercClass">
		*/
		public Dispatcher(IFramework framework, String mercName, Class<?> mercClass)
		{
			try {
				LogManager logManager = LogManager.getLogManager();
				logManager.readConfiguration(new FileInputStream("./rsc/loggingMerc.properties"));
				//fh = new FileHandler("/Users/aggshow/desktop/mercframework.log", 10000, 3, true);
			} catch (Exception e) {
				e.printStackTrace();
			}
			logger = Logger.getLogger(Dispatcher.class.getName());
			
			this.framework = framework;
			this.mercName = mercName;
			this.mercClass = mercClass;

			live = false;
			mq = new MessageQueue();
			loadBinding(); //load messages binding and set interesting messages to framework
		}

		/** 
		 Create Merc and dispatch message to him 
		 
		*/
		public void run()
		{
			logInfo("Hello!");

			// Merc Creation
			try
			{
				Constructor c = mercClass.getConstructor(new java.lang.Class[] { });
				merc = (IMerc)c.newInstance(new Object[] { });
				merc.setDispatcher(this);
				merc.loadStateBinding();
				
				// Cicling dispatching
				dispatchMessages();
			}
			catch (Exception e)
			{
				deliverExceptionToFramework(e);
			}

			logInfo("Bye Bye!");
		}

		/** 
		 Dispatch Messages to Merc until lives
		 
		*/
		private void dispatchMessages()
		{
			while (live)
			{
				if (!mq.isEmpty())
				{
					processNextMessage();
				}
				else
				{
					try {
						Thread.sleep(100);
					} catch (InterruptedException e) {
						// TODO Auto-generated catch block
						//e.printStackTrace();
					}
				}
			}
		}
		
		/**
		 * @see it.mintlab.desktopjava.mercframework.IMDispatcher#registerMerc(java.lang.String, java.lang.String)
		 */
		public boolean registerMerc(String mercName, String className)
		{
			return framework.registerMerc(mercName, className);
		}

		/**
		 * @see it.mintlab.desktopjava.mercframework.IMDispatcher#deregisterMerc()
		 */
		public boolean deregisterMerc()
		{
			this.killMerc();
			return framework.deregisterMerc(this.mercName, (ArrayList<MessageContent>)bindingMap.keySet() );
		}

		public final boolean hasMessage(String tuple, MessageContent.Category category)
		{
			return mq.contains(new MessageContent(category, tuple));
		}

		public final boolean hasMessage()
		{
			return !mq.isEmpty();
		}

		public final void processNextMessage(String tuple, MessageContent.Category category)
		{
			Term t = TupleEngine.parse(tuple);
			MessageContent mc = new MessageContent(category, t);
			Message msg = null;
			try {
				msg = mq.dequeue(mc);
				processMessage(msg);
			} catch (Exception e) {
				deliverExceptionToFramework(e);
			}
		}

		public final void processNextMessage()
		{
			Message msg;
			try {
				msg = mq.dequeue();
				processMessage(msg);
			} catch (Exception e) {
				deliverExceptionToFramework(e);
			}
		}

		/** 
		* Invoke the right merc method based on current state and message tuple
		*/
		private void processMessage(Message msg)
		{
			try
			{
				HashMap<String, Method> d = bindingMap.get(msg.getContent());
				Method mi;
				mi = d.get(StateBinding.DEFAULT_STATE);
				if(mi == null)
				{
					mi = d.get(merc.getState());
					if(mi == null)
					{	
						return; //No invocation because no method declaration for this state and for default state
					}
				}
				logInfo("Process message " + msg);
				
				// Parameters management
				Annotation[] annotations = mi.getAnnotations();
				for(Annotation a : annotations)
				{
					if(a.annotationType().equals(MessageBinding.class))
					{
				
						MessageBinding mb = (MessageBinding)a;			
						Term filter = TupleEngine.parse(mb.message());
						Term tuple = msg.getContent().getTuple();
						if (tuple.match(filter))
						{
							Class<?>[] parsTypes = mi.getParameterTypes();
							HashMap<String, Term> vars = new HashMap<String, Term>();
							filter.unify(tuple, vars);
							// Inserting sender name in tuple (last parameters position)
							if (!mb.senderParam().isEmpty())
							{
								vars.put("_" + (parsTypes.length-1), new Atom(msg.getSender()));
							}
							// There's no way to obtain parameters names. It will used the order.
							Object[] pars = new Object[parsTypes.length];
							try {
								for(int i = 0; i < parsTypes.length; i++)
								{
									Term t = vars.get("_" + i);
									if(t == null)
										throw new Exception("No tuple corresponding for parameter at position " + i + " in method " + mi.getName());
									pars[i] = t.getRefObject();
								}
								// METHOD INVOCATION
								if(mb.senders().length > 0)
								{
									for(String s: mb.senders())
									{
										if(msg.getSender().equals(s))
										{
											mi.invoke(merc, pars);
											return;
										}
									}
								}
								else
								{
									mi.invoke(merc, pars);
								}
							} catch (Exception e) {
								deliverExceptionToFramework(e);
							}
							return;
						}
					}
				}
			}
			catch (RuntimeException e)
			{
				deliverExceptionToFramework(e);
			}
		}

		public final void deleteMessage(String tuple, MessageContent.Category category)
		{
			mq.clear(new MessageContent(category, tuple));
		}

		public final void deliverMessage(String tuple)
		{
			deliverMessage(null, tuple);
		}

		public final void deliverMessage(String recipient, String tuple)
		{
			MessageContent mc = new MessageContent(MessageContent.Category.COMMAND, tuple);
			if(recipient == null)
			{
				logInfo("Send broadcast message " + mc);
			}
			else
			{
				logInfo("Send message " + mc + " to " + recipient);
			}
			framework.deliverMessage(new Message(mercName, recipient, mc));
		}

		public final void deliverMessage(String tuple, Object[] parameters)
		{
			deliverMessage(null, tuple, parameters, MessageContent.Category.COMMAND);
		}

		public final void deliverMessage(String recipient, String tuple, Object[] parameters)
		{
			deliverMessage(recipient, tuple, parameters, MessageContent.Category.COMMAND);
		}

		public final void deliverMessage(String tuple, Object[] parameters, MessageContent.Category category)
		{
			deliverMessage(null, tuple, parameters, category);
		}

		public final void deliverMessage(String recipient, String tuple, Object[] parameters, MessageContent.Category category)
		{
			MessageContent mc = new MessageContent(category, tuple, parameters);
			if (recipient == null)
			{
				logInfo("Send broadcast message " + mc);
			}
			else
			{
				logInfo("Send message " + mc + " to " + recipient);
			}
			framework.deliverMessage(new Message(mercName, recipient, mc));
		}

		public final void updateMerc()
		{
			// find dll path
			// download dll
			// stop Merc
			// remove Merc refs
			// remove messages form MCDictionary
			// replace dll
			// reload dll
			// add messages form MCDictionary
			// restart Merc
		}

		public final void killMerc()
		{
			live = false;
			mq.clear();
			framework.checkForNodeShutdown();
		}

		public final void logInfo(String logMsg)
		{
			logger.info("[" + mercName + "] " + logMsg);
		}
		
		public final void logError(String logMsg)
		{
			logger.severe("[" + mercName + "] " + logMsg);
		}

		public final void letMessage(Message message)
		{

			mq.enqueue(message);

			// inside lock nobody can give more than one life
			synchronized (lifeGivingLock)
			{
				if (!live)
				{
					live = true;
					(new Thread(this)).start();
				}
			}

		}

		/** 
		* Load messages binding with Merc
		* 
		* @return 
		*/
		private void loadBinding()
		{
			java.util.ArrayList<MessageContent> msgNames = new java.util.ArrayList<MessageContent>();
			bindingMap = new MCDictionary<HashMap<String,Method>>();
			Method[] methods = mercClass.getDeclaredMethods();
			for (Method mi : methods)
			{
				Annotation[] annotations = mi.getAnnotations();
				for(Annotation a : annotations)
				{
					if(a.annotationType().equals(MessageBinding.class))
					{
						MessageBinding mb = (MessageBinding)a;
						MessageContent key = new MessageContent(mb.category(), mb.message());
						if (!bindingMap.containsKey(key))
						{
							bindingMap.put(key, new HashMap<String, Method>());
							msgNames.add(key);
						}
						//If state already defined for this method catch exception with logging
						for(String st : mb.states())
						{
							try
							{
							bindingMap.get(key).put(st, mi);
							}
							catch (IllegalArgumentException ae)
							{
								logInfo("Method already present for message" + msgNames.toString() + " and state " + st);
							}
						}
					}
				}
			}
			framework.setMercMessages(msgNames, mercName);
		}
		
		public boolean isMercAlive()
		{
			return live;
		}

		private void deliverExceptionToFramework(Exception e)
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
			framework.deliverMessage(new Message(mercName, null, new MessageContent(MessageContent.Category.ERROR, termException)));
		}


	}