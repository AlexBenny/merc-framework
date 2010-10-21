using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.mercframework
{
    public interface IFramework
    {
        /// <summary>
        /// Deliver a message
        /// </summary>
        /// <param name="msg">Message to deliver</param>
        /// <returns>return true if message was delivered</returns>
        bool deliverMessage(Message msg);

        /// <summary>
        /// Set messages that interest to a Merc
        /// </summary>
        /// <param id="messages">messages names</param>
        /// <param id="mercName">merc id</param>
        void setMercMessages(List<MessageContent> messages, string mercName);

        /// <summary>
        /// If no survivors the Framework close the node interface
        /// </summary>
        void checkForNodeShutdown();

        /// <summary>
        /// Force node shutdown (used in windows service)
        /// </summary>
        void forceNodeShutdown();

        /// <summary>
        /// Register a new Merc in the framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <param name="className">class and assembly names</param>
        /// <returns>true if registration done, false otherwise. If it's already registered return true</returns>
        bool registerMerc(string mercName, string className);

        /// <summary>
        /// Deregister a present merc from framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <param id="messages">message merc names</param>
        /// <returns>true if deregistration done, false otherwise.</returns>
        bool deregisterMerc(string mercName, List<MessageContent> mercMessages);

        /// <summary>
        /// Register a remote Merc in the framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <param name="uri">remote framework uri</param>
        /// <returns>true if registration done, false otherwise</returns>
        bool registerRemoteMerc(string mercName, Uri uri);

        /// <summary>
        /// Deregister remote Merc from framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <returns>true if deregistration done, false otherwise.</returns>
        bool deregisterRemoteMerc(string mercName);

        /// <summary>
        /// Get the URI where framework listen remote request.
        /// </summary>
        Uri getFrameworkURI();

        /// <summary>
        /// Log Info Messages
        /// </summary>
        /// <param name="logMsg"></param>
        void logInfo(string logMsg);

        /// <summary>
        /// log Error Messages
        /// </summary>
        /// <param name="logMsg"></param>
        void logError(string logMsg, Exception exception);
    }
}
