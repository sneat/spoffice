using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spoffice.Lib
{

    public class Controller
    {

        private static Controller current;
        public static Controller Current
        {
            get
            {
                return current ?? (current = new Controller());
            }
        }
        protected Controller()
        {

        }

        // just forces there to be a new controller. doesnt do anything with it.
        public static void Start()
        {
            Controller c = Current;
        }
    }
}
