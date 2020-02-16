using Eagle_Island;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TAS.Interface
{
    class TimedMessage
    {
        public string Message { get; private set; }
        public int TimeToLive { get; private set; }
        public Vector2 Position { get; private set; }
        public Color Color { get; private set; }

        private int liveTime = 0;

        /// <summary>
        /// Creates a new timed message at the given position. If a timeToLive parameter
        /// is supplied it will stop drawing after that many frames, otherwise it will
        /// always be drawn.
        /// </summary>
        /// <param name="message">The message to draw</param>
        /// <param name="position">The position to draw it. (0,0) is top-left, 216 is center x</param>
        /// <param name="timeToLive">Frames to draw the message</param>
        public TimedMessage(string message, Vector2 position, int timeToLive = -1)
        {
            this.Message = message;
            this.TimeToLive = timeToLive;
            this.Position = position;
            this.Color = Color.White;
        }

        // Returns true if the message should still exist
        public bool Draw(SpriteBatch spriteBatch)
        {
            if (TimeToLive >= 0)
            {
                liveTime++;
                if (liveTime > TimeToLive)
                {
                    return false; 
                }
            }

            Graphics.DrawString(spriteBatch, Graphics.SfPixelOperator, Message, Position, Graphics.TextAlign.Center, Color);

            return true;
        }
    }
}
