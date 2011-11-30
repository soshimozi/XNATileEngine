using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileEngine
{
    public interface ITalkable
    {
        void StartConversation(string conversationName);

        void EndConversation();

        bool InSpeakingRange(AnimatedSprite sprite);

        float ReactionRadius
        {
            get;
            set;
        }
    }
}
