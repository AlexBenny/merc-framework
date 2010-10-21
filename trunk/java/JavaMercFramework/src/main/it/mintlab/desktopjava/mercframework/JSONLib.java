/**
 * @author Claudio Buda
 * Mint s.n.c.
 * Forl“, Nov 6, 2009
 */
package it.mintlab.desktopjava.mercframework;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * JSON Library for MercMessages
 */
public class JSONLib {

	public static Message json2MercMessage(String jsonString)
	{
		Message mercMessage = null;
		JSONObject jsonObject;
		try {
			jsonObject = new JSONObject(jsonString);
			mercMessage = new Message(
					jsonObject.getString("sender"),
					jsonObject.getString("recipient"),
					new MessageContent(MessageContent.Category.COMMAND, jsonObject.getString("content")));
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return mercMessage;
	}
	
	public static String mercMessage2Json(Message mercMessage)
	{
		JSONObject jsonObject = new JSONObject();
		try {
			jsonObject.put("sender", mercMessage.getSender());
			jsonObject.put("recipient", mercMessage.getRecipient());
			jsonObject.put("content", mercMessage.getContent().getTuple());
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return jsonObject.toString();
	}
	
	public static String JsonOkResponse() 
	{
		try {
			return new JSONObject().put("sendingResult", true).toString();
		} catch (JSONException e) {
			e.printStackTrace();
			return null;
		}
	}
	
	public static String JsonKoResponse()
	{
		try {
			return new JSONObject().put("sendingResult", false).toString();
		} catch (JSONException e) {
			e.printStackTrace();
			return null;
		}
	}
}
