/**
 * @author Claudio Buda
 * Mint s.n.c.
 * Forl“, Jan 17, 2010
 */
package it.mintlab.desktopjava.mercframework.test;

import it.mintlab.desktopjava.mercframework.*;

/**
 * This merc lives on framework of receiver. Used for broadcasting testing purpose
 */
public class ReceiverFriendMerc extends Merc {

	@MessageBinding(message = "init")
    public void init()
    {
		dispatcher.logInfo("Waiting for friend..");
    }

    @MessageBinding(message = "hello")
    public void hello()
    {
    }

    @MessageBinding(message = "response(_0)")
    public void response(String msg)
    {
    }

    @MessageBinding(message = "question(_0)")
    public void question(String msg)
    {
        dispatcher.deliverMessage("Sender", "response('Looking for a present!')");
    }

    @MessageBinding(message = "bye")
    public void bye()
    {
        dispatcher.killMerc();
    }
}
