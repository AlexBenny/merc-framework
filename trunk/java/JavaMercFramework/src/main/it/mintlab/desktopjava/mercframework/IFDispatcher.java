package it.mintlab.desktopjava.mercframework;

	/** 
	 This interface show Merc Node api for the Framework
	 
	*/
	public interface IFDispatcher
	{
		/** 
		 Give message to Merc Node
		 
		 <param id="message">message
		*/
		void letMessage(Message message);

		/** 
		 Kill the Merc
		 
		*/
		void killMerc();

		/**
		 *  Return Merc State
		 */
		boolean isMercAlive();

	}