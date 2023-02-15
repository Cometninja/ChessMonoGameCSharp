using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Chess1
{
    public class JsonChessPiece
    {
        public int pointX { get; set; }
        public int pointY { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public bool dead { get; set; }

    }
}
