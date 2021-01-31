using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLocker.Extensions
{
    public class RectangleX 
    {
        public Rectangle ResultingRect { get; set; }
        public RectangleX(Point p1, Point p2)
        {
            ResultingRect = new Rectangle(p1.X, p1.Y, p2.X, p2.Y);
        }
    }
}
