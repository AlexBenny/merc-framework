package it.mintlab.desktopjava.mercframework;

import java.lang.annotation.Documented;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;

	@Documented
	@Retention(RetentionPolicy.RUNTIME)
	public @interface StateBinding
	{
		public static final String DEFAULT_STATE = "$DEFAULT";
		//public static final String STATE_PREFIX = "@";

		public String state();
	}