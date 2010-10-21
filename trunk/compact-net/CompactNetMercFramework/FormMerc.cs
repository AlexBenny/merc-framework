using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;

namespace it.mintlab.mobilenet.mercframework
{
    public class FormMerc : Form, IMerc
    {
        private System.Windows.Forms.Timer tmNextMsg;

        protected IMDispatcher dispatcher;
        private Dictionary<string, MethodInfo> stateBindingMap; // stateName <-> MethodInfo
        private string state = StateBinding.DEFAULT_STATE;
        private bool onStateChanging = false;

        public FormMerc()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dispatcher Setter
        /// </summary>
        /// <param id="dispatcher">Merc dispatcher</param>
        public void setDispatcher(IMDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// State property for subclasses
        /// </summary>
        protected string State
        {
            get
            {
                return state.Substring(1);
            }
            set
            {
                value = StateBinding.STATE_PREFIX + value;
                if (!state.Equals(value))
                {
                    if (onStateChanging)
                    {
                        dispatcher.logError("Change to " + value + " is not allowed during change from " + state, null);
                        return;
                    }
                    onStateChanging = true;
                    if (stateBindingMap.ContainsKey(value))
                    {
                        // State method invocation
                        stateBindingMap[value].Invoke(this, null);
                    }
                    dispatcher.logInfo("State change: " + state + " -> " + value);
                    state = value;
                    onStateChanging = false;
                }
                else
                    dispatcher.logInfo("Same State " + state + " change attempted");
            }
        }

        /// <summary>
        /// Get the state of Merc
        /// </summary>
        /// <returns></returns>
        public string getState()
        {
            return state;
        }


        /// <summary>
        /// Set Binding beetween state and method to call for state changing
        /// </summary>
        public void loadStateBinding()
        {
            stateBindingMap = new Dictionary<string, MethodInfo>();
            MethodInfo[] methods = this.GetType().GetMethods();
            foreach (MethodInfo mi in methods)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(mi, typeof(StateBinding), true);
                foreach (Attribute a in attrs)
                {
                    string state = StateBinding.STATE_PREFIX + ((StateBinding)a).state;
                    if (!stateBindingMap.ContainsKey(state))
                    {
                        stateBindingMap.Add(state, mi);
                    }
                    else
                    {
                        dispatcher.logInfo("Method already present for state " + state);
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.tmNextMsg = new System.Windows.Forms.Timer();
            this.SuspendLayout();
            // 
            // tmNextMsg
            // 
            this.tmNextMsg.Enabled = false;
            this.tmNextMsg.Tick += new System.EventHandler(this.tmNextMsg_Tick);
            // 
            // MercView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(341, 251);
            this.Name = "MercForm";
            this.ResumeLayout(false);

        }

        protected void showForm()
        {
            // if exception while showing, throw it but reopen form            
            //while (true)  //useless
            //{
                try
                {
                    this.tmNextMsg.Enabled = true;
                   // AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);
                    this.ShowDialog();

                    return;
                }
                catch (Exception e)
                {
                    dispatcher.deliverMessage("ex($0)", new object[] { e }, MessageContent.Category.ERROR);
                    MessageBox.Show(e.Message);   // It'll be showed it for a sec
                    this.Close();
                    //this.Dispose();
                    
                }
            //}
        }

        // In CF case only, ALL unhandled exceptions come here
        private static void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                MessageBox.Show(ex.Message);
                // Can't imagine e.IsTerminating ever being false
                // or e.ExceptionObject not being an Exception
                //SomeClass.SomeStaticHandlingMethod(ex, e.IsTerminating);
            }
        }

        private void tmNextMsg_Tick(object sender, EventArgs e)
        {
            if (dispatcher.hasMessage())
                dispatcher.processNextMessage();
        }
        
    }
}
