/**
 * @author Claudio Buda
 * Mint s.n.c.
 * Forl“, Jan 17, 2010
 */
package it.mintlab.desktopjava.mercframework;

import java.io.IOException;
import java.net.URI;

import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;

/**
 * 
 */
public class SendExecutorCommand implements Runnable {

	Message msg;
	URI uri;
	
	public SendExecutorCommand(Message msg, URI uri) {
		this.msg = msg;
		this.uri = uri;
	}

	/**
	 * @see java.lang.Runnable#run()
	 */
	@Override
	public void run() {
		HttpClient httpClient = new DefaultHttpClient();

		try {
			
			HttpPost httpPost = new HttpPost(uri);
			StringEntity entity = new StringEntity(JSONLib.mercMessage2Json(msg));
			httpPost.setHeader("Accept", "application/json");
			httpPost.setHeader("Content-type", "application/json");
			httpPost.setEntity(entity);
	
			/*
			 * If Synch needs:
			 * ResponseHandler<String> handler = new ResponseHandler<String>() {
			    public String handleResponse(HttpResponse response) 
			    		throws ClientProtocolException, IOException {
			        HttpEntity entity = response.getEntity();
			        if (entity != null) {
			            return EntityUtils.toString(entity);
			        } else {
			            return null;
			        }
			    }
			};
			String response = httpclient.execute(httppost, handler);*/
			
			httpClient.execute(httpPost);
			
		} catch (ClientProtocolException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
}