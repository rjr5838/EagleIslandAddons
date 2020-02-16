using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TAS.Interface;

namespace TAS
{
    public class InterfaceManager
    {
        private static TimedMessage notification;
        private static List<TimedMessage> messages = new List<TimedMessage>();
        public static void DrawGame(SpriteBatch spriteBatch)
        {
            if (notification != null && !notification.Draw(spriteBatch))
            {
                notification = null;
            }

            for (int i = messages.Count - 1; i >= 0; i--)
            {
                if (!messages[i].Draw(spriteBatch))
                {
                    messages.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Adds a message to the interface.
        /// </summary>
        /// <param name="message">The string to display</param>
        /// <param name="position">The position to display it</param>
        /// <param name="timeToLive">Number of frames to display for</param>
        public static void AddMessage(string message, Vector2 position, int timeToLive)
        {
            messages.Add(new TimedMessage(message, position, timeToLive));
        }

        /// <summary>
        /// Set the notification that will appear top-center of the screen, just below the perks UI.
        /// Only 1 notification can be drawn at a time to avoid overlap.
        /// </summary>
        /// <param name="message">The message to display</param>
        public static void SetNotification(string message)
        {
            notification = new TimedMessage(message, new Vector2(216, 205), 90);
        }
    }
}
