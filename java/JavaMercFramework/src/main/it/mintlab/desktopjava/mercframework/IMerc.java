package it.mintlab.desktopjava.mercframework;

	public interface IMerc
	{
		/** 
		 Dispatcher Setter
		 
		 <param id="dispatcher">
		*/
		void setDispatcher(IMDispatcher dispatcher);

		/** 
		 Get the state of Merc
		 
		 @return 
		*/
		String getState();

		/** 
		 Set Binding beetween state and method to call for state changing
		 
		*/
		void loadStateBinding();

	}