package it.mintlab.desktopjava.mercframework;

import java.util.ArrayList;

	/** 
	 This Interface represent the dispatcher api for merc
	 
	*/
	public interface IMDispatcher
	{
		/**
		 * Register a new Merc in the framework at runtime
		 * @param mercName unique merc name
		 * @param className class and assembly name
		 * @return true if registration done, false otherwise
		 */
		boolean registerMerc(String mercName, String className);
		
		/**
		 * Deregister itself from framework at runtime
		 * @return true if deregistration done, false otherwise.
		 */
		boolean deregisterMerc();
		
		/** 
		 Return true if the message is in queue
		 
		 <param id="tuple">tuple
		 <param id="category">message category
		 @return 
		*/
		boolean hasMessage(String tuple, MessageContent.Category category);

		/** 
		 Return true if any message in queue
		 
		 @return 
		*/
		boolean hasMessage();

		/** 
		 Consume the message (execute his command)
		 
		 <param id="tuple">message tuple
		 <param id="category">message category
		 @return 
		*/
		void processNextMessage(String tuple, MessageContent.Category category);

		/** 
		 Consume the message fisrt message in queue
		 
		 @return 
		*/
		void processNextMessage();

		/** 
		 Remove messages with this command id from queue
		 
		 <param id="tuple">message tuple
		 <param id="category">message category
		 @return 
		*/
		void deleteMessage(String tuple, MessageContent.Category category);

		/** 
		 Send broadcast message
		 
		 <param id="tuple">tuple
		 @return 
		*/
		void deliverMessage(String tuple);

		/** 
		 Send a broadcast message with parameters
		 
		 <param id="tuple">tuple
		 <param id="parameters">parameters
		 @return 
		*/
		void deliverMessage(String tuple, Object[] parameters);

		/** 
		 Send a broadcast message to another Merc with parameters and category
		 
		 <param id="tuple">tuple
		 <param id="parameters">Message parameters
		 <param id="category">Message category
		 @return 
		*/
		void deliverMessage(String tuple, Object[] parameters, MessageContent.Category category);

		/** 
		 Send message to another Merc
		 
		 <param id="recipient">Merc recipient
		 <param id="tuple">tuple
		 @return 
		*/
		void deliverMessage(String recipient, String tuple);

		/** 
		 Send message to another Merc with parameters
		 
		 <param id="recipient">Merc recipient
		 <param id="tuple">tuple
		 <param id="parameters">parameters
		 @return 
		*/
		void deliverMessage(String recipient, String tuple, Object[] parameters);

		/** 
		 Send message to another Merc with parameters and category
		 
		 <param id="recipient">Merc recipient
		 <param id="tuple">tuple
		 <param id="parameters">Message parameters
		 <param id="category">Message category
		 @return 
		*/
		void deliverMessage(String recipient, String tuple, Object[] parameters, MessageContent.Category category);

		/** 
		 Kill the Merc
		 
		*/
		void killMerc();

		/** 
		* Logging method at Info Level
		*/
		void logInfo(String logMsg);
		
		/** 
		* Logging method at Error Level
		*/
		void logError(String logMsg);

	}