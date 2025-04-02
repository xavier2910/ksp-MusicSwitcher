using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSwitcher
{
    // For whatever reason, System.Collections.Generic.Queue got stranded somewhere
    // between assemblies. So this is a (specialized) replacement.
    internal class TrackQueue
    {
        // I think these are better names than 'first' and 'last' or
        // 'back' and 'front'. You're entitled to your own opinion.
        // This includes my future self.
        private TQNode nextUp = null;
        private TQNode mostRecent = null;

        public void Enqueue(Track t)
        {
            TQNode newNode = new TQNode(t);
            if (nextUp == null)
            {
                nextUp = newNode;
            }
            else {
                mostRecent.next = newNode;
            }
            mostRecent = newNode;
        }

        public Track Dequeue()
        {
            if (nextUp == null)
            {
                return null;
            }
            var retVal = nextUp.track;
            nextUp = nextUp.next;
            if (nextUp == null)
            {
                mostRecent = null;
            }
            return retVal;
        }

        public void Clear()
        {
            nextUp = null;
            mostRecent = null;
            //dang it; why can't I manage my own memory??
            //I'll just have to hope this doesn't leak or anything...
            //The GC's smart enough to detect whole unreferenced reference
            //chains right? Any GC worth its salt should do that...
        }

        public bool IsEmpty()
        {
            return nextUp == null;
        }

    }

    /// <summary>
    /// Yes, Grandma always dies at the end. It's sad.
    /// </summary>
    internal class TQNode
    {
        public Track track;
        public TQNode next;

        public TQNode(Track t)
        {
            track = t;
        }
    }
}

