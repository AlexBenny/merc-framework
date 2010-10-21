using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.mobilenet.tupleengine;

namespace it.mintlab.mobilenet.mercframework
{
    public class MessageContent
    {
        private Category category;
        private Term tuple;

        public enum Category
        {
            COMMAND,
            ERROR,
            LOG
        };


        public MessageContent(Category category, string tuple)
            : this(category, TupleEngine.parse(tuple))
        { }

        public MessageContent(Category category, string tuple, object[] pars)
            : this(category, TupleEngine.parse(tuple, pars))
        { }

        public MessageContent(Category category, Term tuple)
        {
            this.category = category;
            this.tuple = tuple;
        }

        public Category getCategory()
        {
            return category;
        }

        public Term getTuple()
        {
            return tuple;
        }

        public override bool Equals(object obj)
        {
            MessageContent mc = (MessageContent)obj;
            return category == mc.getCategory() && tuple.match(mc.getTuple());
        }

        public override int GetHashCode()
        {
            return getKey().GetHashCode();
        }

        public override string ToString()
        {
            return tuple + " [" + category + "]";
        }

        internal string getKey()
        {
            string functor = tuple.ToString().Split(new char[] { '(' })[0]; //return the first part of tuple
            return "" + category + functor;
        }

    }
}
