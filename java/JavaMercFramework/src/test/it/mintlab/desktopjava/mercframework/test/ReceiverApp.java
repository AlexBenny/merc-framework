/**
 * @author Claudio Buda
 * Mint s.n.c.
 * Forl“, Nov 6, 2009
 */
package it.mintlab.desktopjava.mercframework.test;

import it.mintlab.desktopjava.mercframework.Framework;

/**
 * 
 */
public class ReceiverApp {

	/**
	 * The main entry point for an application.
	 * @param args
	 */
	public static void main(String[] args) {
		new Framework("./rsc/receiverMercConfig.xml");
	}

}
