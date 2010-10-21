using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;

namespace it.mintlab.mobilenet.mercframework
{
    public class Merc : IMerc
    {
        protected IMDispatcher dispatcher;
        private Dictionary<string, MethodInfo> stateBindingMap; // stateName <-> MethodInfo
        private string state = StateBinding.DEFAULT_STATE;
        private bool onStateChanging = false;

        /// <summary>
        /// State Property for subclasses
        /// </summary>
        /// <returns></returns>
        protected string State
        {
            get { return state.Substring(1); }
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
        /// Dispatcher Setter
        /// </summary>
        /// <param id="dispatcher">Merc dispatcher</param>
        public void setDispatcher(IMDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Get the state of Merc
        /// </summary>
        /// <returns></returns>
        public string getState() { return state; }

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
                        dispatcher.logError("Method already present for state " + state, null);
                    }
                }
            }
        }

    }
}
