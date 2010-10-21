package it.mintlab.desktopjava.mercframework;

import java.lang.annotation.Documented;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;

	@Documented
	@Retention(RetentionPolicy.RUNTIME)
	public @interface MessageBinding
	{
		public String message();
		public MessageContent.Category category() default MessageContent.Category.COMMAND;
		//public String state() default StateBinding.DEFAULT_STATE;
        public String[] states() default { StateBinding.DEFAULT_STATE };
        public String[] senders() default {};
        public String senderParam() default "";

/*		
		public String getstate()
		{
			return _state;
		}
		
		public void setstate(String value)
		{
			_state = StateBinding.STATE_PREFIX + value;
		}*/
	}