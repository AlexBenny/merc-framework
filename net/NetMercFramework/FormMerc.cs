using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;

namespace it.mintlab.desktopnet.mercframework
{
    public class FormMerc : Form, IMerc
    {
        private System.ComponentModel.IContainer components;
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
            this.components = new System.ComponentModel.Container();
            
            this.SuspendLayout();
            // 
            // tmNextMsg
            // 
            this.tmNextMsg = new System.Windows.Forms.Timer(this.components);
            this.tmNextMsg.Enabled = false;
            this.tmNextMsg.Tick += new System.EventHandler(this.tmNextMsg_Tick);
            // 
            // MercForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 271);
            this.Name = "MercForm";
            this.ResumeLayout(false);
        }

        protected void showForm()
        {
            // if exception while showing, throw it but reopen form            
            while (true)
            {
                try
                {
                    this.tmNextMsg.Enabled = true;
                    Application.EnableVisualStyles();
                    this.ShowDialog();
                    return;
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                    dispatcher.deliverMessage("ex($0)", new object[] { e }, MessageContent.Category.ERROR);
                }
            }
        }

        protected void showFormIfInvisible()
        {
            this.tmNextMsg.Enabled = true;
            this.Visible = false;
            //Application.EnableVisualStyles();
            Application.ThreadException += new ThreadExceptionEventHandler(new ThreadExceptionHandler(this).ApplicationThreadException);
            Application.Run();
        }

        protected void closeFormIfInvisible()
        {
            Application.Exit();
        }

        private void tmNextMsg_Tick(object sender, EventArgs e)
        {
            if (dispatcher.hasMessage())
                dispatcher.processNextMessage();
        }

        private void throwGUIException(Exception e) 
        {
            //MessageBox.Show(e.Message);
            dispatcher.deliverMessage("ex($0)", new object[] { e }, MessageContent.Category.ERROR);
        }

        /// <summary>
        /// Handles any thread exceptions
        /// </summary>
        public class ThreadExceptionHandler
        {
            FormMerc mercSource;
            
            public ThreadExceptionHandler(FormMerc merc) 
            {
                this.mercSource = merc;
            }

            public void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
            {
                mercSource.throwGUIException(e.Exception);
            }
        }
    }
}
