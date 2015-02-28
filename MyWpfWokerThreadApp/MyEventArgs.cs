using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWpfWokerThreadApp
{
    public class MyEventArgs
    {
        public int Id { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MyEventArgs()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        public MyEventArgs(int id,string content)
        {
            Id = id;
            Content = content;
        }
    }
}
