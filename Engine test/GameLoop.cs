using OpenGLEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_test
{
    internal  class GameLoop
    {
        Entity e;
        bool running;
        public GameLoop(Entity e)
        {
            this.e = e;
            running = true;
        }
        public void run()
        {
            float rot = 0f;
            float rotationSpeed = 0.03f;
            while (running)
            {
                rot += rotationSpeed;
                rot = rot % ((float)Math.PI*2f);
                e.setRotation(rot, rot, 0);
                e.setLocation((float)Math.Cos(rot), (float)Math.Sin(rot), 0);
                Thread.Sleep(25);
            }
        }
        public void stop()
        {
            running = false;
        }
    }
}
