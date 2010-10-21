using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.mercframework
{
    /// <summary>
    /// This Interface represent the dispatcher api for Merc
    /// </summary>
    public interface IMDispatcher
    {
        /// <summary>
        /// Register a new Merc in the framework at runtime
        /// </summary>
        /// <param name="mercName">unique merc name</param>
        /// <param name="className">class and assembly names</param>
        /// <returns>true if registration done, false otherwise</returns>
        bool registerMerc(string mercName, string className);

        /// <summary>
        /// Deregister itself from framework at runtime
        /// </summary>
        /// <returns>true if deregistration done, false otherwise.</returns>
        bool deregisterMerc();

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
        /// Return true if the message is in queue
        /// </summary>
        /// <param id="tuple">tuple</param>
        /// <param id="category">message category</param>
        /// <returns></returns>
        bool hasMessage(string tuple, MessageContent.Category category);

        /// <summary>
        /// Return true if any message in queue
        /// </summary>
        /// <returns></returns>
        bool hasMessage();

        /// <summary>
        /// Consume the message (execute his command)
        /// </summary>
        /// <param id="tuple">message tuple</param>
        /// <param id="category">message category</param>
        /// <returns></returns>
        void processNextMessage(string tuple, MessageContent.Category category);

        /// <summary>
        /// Consume the message fisrt message in queue
        /// </summary>
        /// <returns></returns>
        void processNextMessage();

        /// <summary>
        /// Remove messages with this command id from queue
        /// </summary>
        /// <param id="tuple">message tuple</param>
        /// <param id="category">message category</param>
        /// <returns></returns>
        void deleteMessage(string tuple, MessageContent.Category category);

        /// <summary>
        /// Send broadcast message
        /// </summary>
        /// <param id="tuple">tuple</param>
        /// <returns></returns>
        void deliverMessage(string tuple);

        /// <summary>
        /// Send a broadcast message with parameters
        /// </summary>
        /// <param id="tuple">tuple</param>
        /// <param id="parameters">parameters</param>
        /// <returns></returns>
        void deliverMessage(string tuple, object[] parameters);

        /// <summary>
        /// Send a broadcast message to another Merc with parameters and category
        /// </summary>
        /// <param id="tuple">tuple</param>
        /// <param id="parameters">Message parameters</param>
        /// <param id="category">Message category</param>
        /// <returns></returns>
        void deliverMessage(string tuple, object[] parameters, MessageContent.Category category);

        /// <summary>
        /// Send message to another Merc
        /// </summary>
        /// <param id="recipient">Merc recipient</param>
        /// <param id="tuple">tuple</param>
        /// <returns></returns>
        void deliverMessage(string recipient, string tuple);

        /// <summary>
        /// Send message to another Merc with parameters
        /// </summary>
        /// <param id="recipient">Merc recipient</param>
        /// <param id="tuple">tuple</param>
        /// <param id="parameters">parameters</param>
        /// <returns></returns>
        void deliverMessage(string recipient, string tuple, object[] parameters);

        /// <summary>
        /// Send message to another Merc with parameters and category
        /// </summary>
        /// <param id="recipient">Merc recipient</param>
        /// <param id="tuple">tuple</param>
        /// <param id="parameters">Message parameters</param>
        /// <param id="category">Message category</param>
        /// <returns></returns>
        void deliverMessage(string recipient, string tuple, object[] parameters, MessageContent.Category category);

        /// <summary>
        /// Kill the Merc
        /// </summary>
        void killMerc();

        /// <summary>
        /// Logging method at Info Level
        /// </summary>
        /// <param id="logMsg">log message</param>
        void logInfo(string logMsg);

        /// <summary>
        /// Logging method at Error Level
        /// </summary>
        /// <param id="logMsg">log message</param>
        void logError(string logMsg, Exception exception);

    }
}
