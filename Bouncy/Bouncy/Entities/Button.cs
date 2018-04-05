using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouncy.Entities
{
    public class Button : CCNode
    {
        CCSprite graphic;
        public Action Action { private get; set; }



        public Button() : this("NO TEXT") { }

        public Button(string text)
        {
            var filename = text + " button.png";
            graphic = new CCSprite(filename);
            graphic.BlendFunc = CCBlendFunc.NonPremultiplied;
            this.AddChild(graphic);
        }

        public void PerformAction()
        {
            Action?.Invoke();
        }

        public bool ContainsPoint(CCPoint point)
        {
            return graphic.BoundingBoxTransformedToWorld.ContainsPoint(point);
        }
    }
}
