using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using it.mintlab.mobilenet.tupleengine;
using log4net;
using log4net.Core;

namespace it.mintlab.mobilenet.mercframework
{
    internal class Dispatcher : IMDispatcher, IFDispatcher
    {
        // Create a logger for use in this class
        //private static readonly ILog log = log4net.LogManager.GetLogger(typeof(Dispatcher));
        // NOTE that using System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        // is equivalent to typeof(LoggingExample) but is more portable
        // i.e. you can copy the code directly into another class without
        // needing to edit the code.

        private IFramework framework;
        private string mercName;
        private Type mercClass;
        
        private bool live;
        private MessageQueue mq;
        private IMerc merc;
        private MCDictionary<Dictionary<string,MethodInfo>> bindingMap;   // MsgContent <-> (state <-> method) 

        private Object lifeGivingLock = new Object(); //to prevent multiple threads creation

        /// <summary>
        /// Costructor
        /// </summary>
        /// <param id="mercName"></param>
        /// <param id="mercClass"></param>
        public Dispatcher(IFramework framework, string mercName, Type mercClass)
        {
            this.framework = framework;
            this.mercName = mercName;
            this.mercClass = mercClass;
            
            live = false;
            mq = new MessageQueue();
            loadBinding(); //load messages binding and set interesting messages to framework
        }

        /// <summary>
        /// Create Merc and dispatch messages to him 
        /// </summary>
        private void run()
        {
            try
            {
                logInfo("Hello!");

                // Merc Creation
                try
                {
                    ConstructorInfo c = mercClass.GetConstructor(new Type[] { });
                    merc = (IMerc)c.Invoke(new object[] { });
                    merc.setDispatcher(this);
                    merc.loadStateBinding();
                }
                catch (Exception ex1)
                {
                    deliverErrorToFramework(ex1);
                }

                // Cicling dispatching
                dispatchMessages();

                logInfo("Bye Bye!");
            }
            catch (Exception ex2)
            {
                // could need set live = false..
                deliverErrorToFramework(ex2);
            }
        }
        

        /// <summary>
        /// Dispatch Messages to Merc until lives
        /// </summary>
        private void dispatchMessages()
        {
            while (live)
            {
                if (!mq.isEmpty())
                    processNextMessage();
                else
                    Thread.Sleep(100);
            }
        }

#region IDispatcher Members

        public bool registerMerc(string mercName, string className)
        {
            return framework.registerMerc(mercName, className);
        }

        public bool deregisterMerc()
        {
            this.killMerc();
            return framework.deregisterMerc(this.mercName, (List<MessageContent>)bindingMap.Keys);
        }

        /// <summary>
        /// Register a remote Merc in the framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <param name="uri">remote framework uri</param>
        /// <returns>true if registration done, false otherwise</returns>
        public bool registerRemoteMerc(string mercName, Uri uri)
        {
            return framework.registerRemoteMerc(mercName, uri);
        }

        /// <summary>
        /// Deregister remote Merc from framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <returns>true if deregistration done, false otherwise.</returns>
        public bool deregisterRemoteMerc(string mercName)
        {
            return framework.deregisterRemoteMerc(mercName);
        }

        /// <summary>
        /// Get the URI where framework listen remote request.
        /// </summary>
        public Uri getFrameworkURI()
        {
            return framework.getFrameworkURI();
        }

        public bool hasMessage(string tuple, MessageContent.Category category)
        {
            return mq.contains(new MessageContent(category, tuple));
        }

        public bool hasMessage()
        {
            return !mq.isEmpty();
        }

        public void processNextMessage(string tuple, MessageContent.Category category)
        {
            Term t = TupleEngine.parse(tuple);
            MessageContent mc = new MessageContent(category, t);
            Message msg = mq.dequeue(mc);
            processMessage(msg);
        }

        public void processNextMessage()
        {
            Message msg = mq.dequeue();
            processMessage(msg);
        }

        /// <summary>
        /// Invoke the right merc method based on current state and message tuple
        /// </summary>
        /// <param name="msg"></param>
        private void processMessage(Message msg)
        {
            try
            {
                Dictionary<string, MethodInfo> d = bindingMap[msg.getContent()];
                MethodInfo mi;

                try
                {
                    mi = d[StateBinding.DEFAULT_STATE];
                }
                catch
                {
                    try
                    {
                        mi = d[merc.getState()];
                    }
                    catch
                    {
                        return;  //No invocation because no method declaration for this state and for DEFAULT state
                    }
                }
                logInfo("Process message " + msg);
                Attribute[] attrs = Attribute.GetCustomAttributes(mi, typeof(MessageBinding), true);
                foreach (Attribute a in attrs)
                {
                    Term filter = TupleEngine.parse(((MessageBinding)a).message);
                    Term tuple = msg.getContent().getTuple();
                    if (tuple.match(filter))
                    {
                        Dictionary<string, Term> vars = new Dictionary<string, Term>();
                        filter.unify(tuple, vars);
                        //assign sender name to a tuple variable
                        if (((MessageBinding)a).senderParam != null)
                        {
                            vars.Add(((MessageBinding)a).senderParam, new Atom(msg.getSender()));
                        }
                        //
                        ParameterInfo[] parsInfo = mi.GetParameters();
                        object[] pars = new object[parsInfo.Length];
                        int i = 0;
                        try
                        {
                            foreach (ParameterInfo pi in parsInfo)
                            {
                                Term t = vars["_" + pi.Name];
                                pars[i] = t.getRefObject();
                                i++;
                            }
                        }
                        catch (KeyNotFoundException knfe)
                        {
                            logInfo("Parameter "+ (i+1) +" mismatch for "+ mi.Name);
                            throw knfe;
                        }
                        //Method invocation
                        if (((MessageBinding)a).senders.Length > 0)
                        {
                            foreach (string s in ((MessageBinding)a).senders)
                            {
                                if (msg.getSender().Equals(s))
                                {
                                    mi.Invoke(merc, pars);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            mi.Invoke(merc, pars);
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                deliverErrorToFramework(e);
            }
        }

        public void deleteMessage(string tuple, MessageContent.Category category)
        {
            mq.clear(new MessageContent(category,tuple));
        }


        public void deliverMessage(string tuple)
        {
            deliverMessage(null, tuple);
        }

        public void deliverMessage(string tuple, object[] parameters)
        {
            deliverMessage(null, tuple, parameters, MessageContent.Category.COMMAND);
        }

        public void deliverMessage(string tuple, object[] parameters, MessageContent.Category category)
        {
            deliverMessage(null, tuple, parameters, category);
        }

        public void deliverMessage(string recipient, string tuple)
        {
            MessageContent mc = new MessageContent(MessageContent.Category.COMMAND, tuple);
            if (recipient == null)
            {
                logInfo("Send broadcast message " + mc);
            } else {
                logInfo("Send message " + mc + " to " + recipient);
            }
            framework.deliverMessage(new Message(mercName, recipient, mc));
        }

        public void deliverMessage(string recipient, string tuple, object[] parameters)
        {
            deliverMessage(recipient, tuple, parameters, MessageContent.Category.COMMAND);
        }

        public void deliverMessage(string recipient, string tuple, object[] parameters, MessageContent.Category category)
        {
            MessageContent mc = new MessageContent(category, tuple, parameters);
            if (recipient == null)
            {
                logInfo("Send broadcast message " + mc);
            } else {
                logInfo("Send message " + mc + " to " + recipient);
            }
            framework.deliverMessage(new Message(mercName, recipient, mc));
        }

        public void killMerc()
        {
            live = false;
            mq.clear();
            framework.checkForNodeShutdown();
        }

        public void logInfo(string logMsg)
        {
            framework.logInfo("[" + mercName + "] " + logMsg);
        }

        public void logError(string logMsg, Exception exception)
        {
            framework.logError("[" + mercName + "] " + logMsg, exception);
        }

#endregion

#region IFDispatcher Members

        public void letMessage(Message message)
        {
            Dictionary<string, MethodInfo> d = bindingMap[message.getContent()];
            MethodInfo mi;
            try
            {
                mi = d[StateBinding.DEFAULT_STATE];
            }
            catch
            {
                try
                {
                    mi = d[merc.getState()];
                }
                catch
                {
                    return;  //No message pushing because no method declaration for this state and for default state
                }
            }
            mq.enqueue(message);

            // inside lock nobody can give more than one life
            lock (lifeGivingLock)
            {
                if (!live)
                {
                    Thread t = new Thread(new ThreadStart(this.run));
                    live = true;
                    t.Start();
                }
            }
            
        }

        /// <summary>
        /// Load messages binding with Merc
        /// </summary>
        /// <returns></returns>
        private void loadBinding()
        {
            List<MessageContent> msgNames = new List<MessageContent>();
            bindingMap = new MCDictionary<Dictionary<string,MethodInfo>>();
            MethodInfo[] methods = mercClass.GetMethods();
            foreach (MethodInfo mi in methods)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(mi, typeof(MessageBinding), true);
                foreach (Attribute a in attrs)
                {
                    MessageBinding b = (MessageBinding)a;
                    MessageContent key = new MessageContent(b.category, b.message);
                    if (!bindingMap.ContainsKey(key))
                    {
                       bindingMap.Add(key, new Dictionary<string, MethodInfo>());
                       msgNames.Add(key);
                    }
                    // if state already defined for this method catch exception with logging
                    foreach (string st in b.states)
                    {
                        try
                        {
                            bindingMap[key].Add(st, mi);
                        }
                        catch (ArgumentException ae)
                        {
                            logInfo("Method already present for message" + msgNames.ToString() + " and state " + st);
                        }
                    }

                }
            }
            framework.setMercMessages(msgNames, mercName);
        }

        public bool isMercAlive()
        {
            return live;
        }

#endregion

        private void deliverErrorToFramework(Exception e)
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
            framework.deliverMessage(new Message(mercName, null, new MessageContent(MessageContent.Category.ERROR, termException)));
        }
    }
}
