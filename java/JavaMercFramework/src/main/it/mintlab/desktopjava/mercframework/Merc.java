package it.mintlab.desktopjava.mercframework;

import java.lang.annotation.Annotation;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.HashMap;

	public class Merc implements IMerc
	{
		protected IMDispatcher dispatcher;
		private HashMap<String, Method> stateBindingMap; // stateName <-> MethodInfo
		private String state = StateBinding.DEFAULT_STATE;
		private boolean onStateChanging = false;

		/** 
		* State property for subclasses
		*/
		public final String getState()
		{
			//return state.substring(1);
			return state;
		}
		
		protected final void setState(String value)
		{
			if(onStateChanging)
			{
				dispatcher.logError("Change to "+value+" is not allowed during change from "+state);
				return;
			}
			onStateChanging = true;
			//value = StateBinding.STATE_PREFIX + value;
			if (state.equals(value)) return;
			if (stateBindingMap.containsKey(value))
			{
				// State method invocation
				try {
					stateBindingMap.get(value).invoke(this, null);
				} catch (Exception e) {
					dispatcher.logError(value + " State Method Invocation Exception");
				}
			}
			dispatcher.logInfo("State change: " + state + " -> " + value);
			state = value;
			onStateChanging = false;
		}

		/* 
		 Get the state of Merc
		 
		 @return 
		
		public final String getRealState()
		{
			return state;
		}*/

		/** 
		 Dispatcher Setter
		 
		 <param id="dispatcher">Merc dispatcher
		*/
		public final void setDispatcher(IMDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		/** 
		* Set Binding beetween state and method to call for state changing
		*/
		public final void loadStateBinding()
		{
			stateBindingMap = new HashMap<String, Method>();
			Method[] methods = this.getClass().getDeclaredMethods();
			for(Method m : methods)
			{
				Annotation[] annotations = m.getAnnotations();
				for(Annotation a : annotations)
				{
					if(a.annotationType().equals(StateBinding.class))
					{	
						StateBinding sb = (StateBinding)a;
						//String state = StateBinding.STATE_PREFIX + sb.state();
						String state = sb.state();
						if (!stateBindingMap.containsKey(state))
						{
							stateBindingMap.put(state, m);
						}
						else
						{
							dispatcher.logError("Method already present for state " + state);
						}
					}
					
				}
			}
		}
	}