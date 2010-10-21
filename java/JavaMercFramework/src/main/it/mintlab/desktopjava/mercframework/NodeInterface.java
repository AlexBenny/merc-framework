/**
 * @author Claudio Buda
 * Mint s.n.c.
 * Forl“, Nov 6, 2009
 */
package it.mintlab.desktopjava.mercframework;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.InetSocketAddress;
import java.net.URI;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import com.sun.net.httpserver.Headers;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpServer;

/**
 * Framework interface for comunicate with remote Mercs
 */
public class NodeInterface {
	
	IFramework mercFramework;
	HttpServer httpServer;

	public NodeInterface(IFramework framework, int port)
	{
		mercFramework = framework;
		InetSocketAddress address = new InetSocketAddress(port);
		
		// request handler
		HttpHandler handler = new HttpHandler() {
			@Override
	        public void handle(HttpExchange exchange) throws IOException {
				String requestBody = convertStreamToString(exchange.getRequestBody());
				String response = "";
				//OutputStream responseBody = exchange.getResponseBody();
			    //Headers requestHeaders = exchange.getRequestHeaders()
			    boolean delivery = mercFramework.deliverMessage(JSONLib.json2MercMessage(requestBody));
			    if(delivery)
			    	response = JSONLib.JsonOkResponse();
			    else
			    	response = JSONLib.JsonKoResponse();
			    
			    Headers responseHeaders = exchange.getResponseHeaders();
			    responseHeaders.set("Content-type", "application/json");
			    exchange.sendResponseHeaders(HttpURLConnection.HTTP_OK, response.length());
		        exchange.getResponseBody().write(response.getBytes());
		        exchange.close();
	        }

	    };
	    
	    try {
			httpServer = HttpServer.create(address, 0);
		    httpServer.createContext("/", handler);
		    // set a pool of 10 threads to serve request
		    httpServer.setExecutor(Executors.newFixedThreadPool(10));
		    httpServer.start();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
			
	/**
	 * To convert the InputStream to String we use the BufferedReader.readLine()
	 * method. We iterate until the BufferedReader return null which means
     * there's no more data to read. Each line will appended to a StringBuilder
     * and returned as String.
     */
	private String convertStreamToString(InputStream is) throws IOException {
        if (is != null) {
        	StringBuilder sb = new StringBuilder();
            String line;

            try {
                BufferedReader reader = new BufferedReader(new InputStreamReader(is, "UTF-8"));
                while ((line = reader.readLine()) != null) {
                    sb.append(line).append("\n");
                }
            } finally {
                is.close();
            }
            return sb.toString();
        } else {       
            return "";
        }
	}
	
	/*public NodeInterface(IFramework framework, int port, String merda) {
		
		
		GrizzlyWebServer ws = new GrizzlyWebServer(port);
	    try{
	        ws.addAsyncFilter(new AsyncFilter() {
	        	private final ScheduledThreadPoolExecutor scheduler = 
                    new ScheduledThreadPoolExecutor(1);
            public boolean doFilter(final AsyncExecutor asyncExecutor) {
                //Throttle the request
            	scheduler.schedule(new Callable() {
                    public Object call() throws Exception {
                        asyncExecutor.execute();
                        Thread.sleep(1000);
                        asyncExecutor.postExecute();
                        return null;
                    }
                }, 2, TimeUnit.SECONDS);


	
	                // Call the next AsyncFilter
	                return true;
	            }
	        });
	
	        ws.addGrizzlyAdapter(new GrizzlyAdapter(){                  
	            public void service(GrizzlyRequest request, GrizzlyResponse response){
	                try {
	                	System.out.println("Richiesta thread2:" + Thread.currentThread().getName());
	                    response.getWriter().println("Grizzly is soon cool");
	                } catch (IOException ex) {                        
	                }
	            }
	        }, new String[]{"/Adapter-1"});
	        ws.start();
	    } catch (IOException ex){
	        // Something when wrong.
	    } 
	}

	public NodeInterface(IFramework framework, int port, int altro) {
		this.mercFramework = framework;
		
		grizzlyWebServer = new GrizzlyWebServer(port);
		
		// For asynchronous responses
//		grizzlyWebServer.addAsyncFilter(
//				new AsyncFilter() {
//					
//					
//		           // private final ScheduledThreadPoolExecutor scheduler = new ScheduledThreadPoolExecutor(1);	//1 is enought?
//		            
//		            public boolean doFilter(final AsyncExecutor asyncExecutor) {
//		            	System.out.println("doFilter");
//		            	
//		            	//Throttle the request
//		              //  scheduler.execute(new Runnable() {
//		
//						//	public void run() {
//								try {
//		
//									
//									asyncExecutor.execute();
//									asyncExecutor.postExecute();
//									
//		
//								} catch (Exception e) {
//									// TODO Auto-generated catch block
//									e.printStackTrace();
//								}		// Call grizzly adapter service method
//					//		}
//		
//		              //  });
//		
//		                // Call the next AsyncFilter
//		                return false;
//		            }
//				});
		grizzlyWebServer.addAsyncFilter(new AsyncFilter() {
            private final ThreadPoolExecutor executor = (ThreadPoolExecutor) Executors.newFixedThreadPool(10);
           
            public boolean doFilter(final AsyncExecutor asyncExecutor) {
                //Throttle the request
            	executor.execute(new Runnable() {
                    
            		@Override
					public void run() {
						// TODO Auto-generated method stub
						try {
							System.out.println("Richiesta thread1:" + Thread.currentThread().getName());
							asyncExecutor.execute();
							asyncExecutor.postExecute();
						} catch (Exception e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
                        
					}
                });

                // Call the next AsyncFilter
                return true;
            }
        });



		// for non static serving
		grizzlyWebServer.addGrizzlyAdapter(
				new GrizzlyAdapter(){  
					
					public void service(GrizzlyRequest request, GrizzlyResponse response){
	                    try {
	                    	System.out.println("Richiesta thread2:" + Thread.currentThread().getName());
	                        response.getWriter().println("Grizzly is so cool!!!");
	                    } catch (Exception ex) {                  
	                    	ex.printStackTrace();
	                    }
	                }
//			            	string result;
//			            	system.out.println("richiesta thread2:" + thread.currentthread().getname());
//			            	try {
//			            		//boolean delivery = mercframework.delivermessage(jsonlib.json2mercmessage("{hello{howareyou}"));
//								response.getwriter().println("ciccio");
//							} catch (ioexception e) {
//								// todo auto-generated catch block
//								e.printstacktrace();
//							}
//			            	
//			            	thread.sleep(3000);
//			            	bufferedreader reader = request.getreader();
//			            		
//			            	string contextpath = request.getcharacterencoding();
//			            	system.out.println(contextpath);
//			            	stringbuilder sb = new stringbuilder();
//			            	string line = null;
//			        	    while ((line = reader.readline()) != null) {
//			        	        sb.append(line + "\n");
//			        	    }
//			        	    result = sb.tostring();
			        	    
			            	
//			            	boolean delivery = mercframework.delivermessage(jsonlib.json2mercmessage(result));
//			                if(delivery)
//			                {
//			                	synchronized(response){
//			                	response.getwriter().println(jsonlib.jsonokresponse());
//			                	}
//			                }else{
//			                	synchronized(response){
//			                	response.getwriter().println(jsonlib.jsonkoresponse());
//			                	}
//			                }
			            
					
			        	
		        }, 
		        null);
	    
        try {
        	grizzlyWebServer.start();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
        
	}*/

	public void shutdown() {
		((ExecutorService)httpServer.getExecutor()).shutdown();
		httpServer.stop(0);
	}

	/**
	 * Send message asynchronously
	 * @param msg
	 * @param uri
	 */
	public static void sendMessage(Message msg, URI uri) {
		ExecutorService executor = Executors.newSingleThreadExecutor();
		Runnable command = new SendExecutorCommand(msg, uri);
		executor.execute(command);
		executor.shutdown();
	}
}