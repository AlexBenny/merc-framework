package it.mintlab.desktopjava.mercframework;

import java.util.ArrayList;

	public interface IFramework
	{
		/**
		 * This method destroy a MercNode by id
		 * @param nodeName id in App.config
		 */
		void destroyMercNode(String nodeName);

		/**
		 * Deliver a message
		 * @param msg
		 * @return return true if message was delivered
		 */
		boolean deliverMessage(Message msg);

		/** 
		 Set messages that interest to a Merc
		 
		 <param id="messages">messages names
		 <param id="mercName">merc id
		*/
		void setMercMessages(java.util.ArrayList<MessageContent> messages, String mercName);

		/**
		* If no survivors the Framework close the node interface
		*/
		void checkForNodeShutdown();
		
		/**
		 * Register a new Merc in the framework at runtime
		 * @param mercName unique merc name
		 * @param className class and assembly names
		 * @return true if registration done, false otherwise. If it's already registered return true
		 */
		boolean registerMerc(String mercName, String className);
		
		/**
		 * Deregister a present merc from framework at runtime
		 * @param mercName unique merc name
		 * @param mercMessages message merc names
		 * @return true if deregistration done, false otherwise.
		 */
		boolean deregisterMerc(String mercName, ArrayList<MessageContent> mercMessages);
	}