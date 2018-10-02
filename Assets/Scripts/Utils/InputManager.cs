using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit
{
    // TODO: It seems that in the future I will need to rewrite this entire file to support user-changeable input settings with support for key combinations
    // because now key combination conflicts are configured manually.

    public sealed class InputManager
    {
        private List<InputTrigger> Triggers = new List<InputTrigger> ();

        public InputManager ()
        {
            UnityUpdateRegistrator.Instance.OnUpdate += Update;
        }
        
        public void AddTrigger(InputTrigger trigger)
        {
            Assert.ArgumentNotNull (trigger, nameof (trigger));
            
            Triggers.Add (trigger);
        }
        
        public void Reset()
        {
            Triggers.Clear ();
        }

        private void Update ()
        {
            foreach ( var trigger in Triggers )
                if ( trigger.IsTriggered (out Action callback) )
                    callback?.Invoke ();
        }
    }
    
    public abstract class InputTrigger
    {
        public Action callback;

        public InputTrigger(Action callback)
        {
            this.callback = callback;
        }

        public InputTrigger AddCallback(Action callback)
        {
            this.callback += callback;
            return this;
        }
        public InputTrigger RemoveCallback(Action callback)
        {
            this.callback -= callback;
            return this;
        }

        public abstract bool IsTriggered (out Action callback);
    }

    /// <summary>
    /// Class for handling key combination conflicts. For example, we have Ctrl + Z key combination for undo and Ctrl + Shift + Z for redo.
    /// If user will press Ctrl + Shift + Z redo operation will perform, but at the same time Ctrl + Z is pressed, so undo also will be performed.
    /// This class allows us to perform only one operation (the last one) at the specific time. In the constructor you need to pass triggers in order of complexity.
    /// For example Ctrl + Z will be the first, Ctrl + Shift + Z will be the second, Ctrl + Shift + Whatever + Z will be the third and so on.
    /// </summary>
    public class InputTriggerConflictChain : InputTrigger
    {
        private List<InputTrigger> triggers = new List<InputTrigger> ();

        public InputTriggerConflictChain (params InputTrigger[] triggers) : base(null)
        {
            if ( triggers != null )
                this.triggers.AddRange (triggers);
        }

        // TODO: Add methods to add, rearrange and delete triggers 

        public override bool IsTriggered (out Action callback)
        {
            InputTrigger lastTrigger = default;
            callback = null;

            foreach ( var trigger in triggers )
                if ( trigger.IsTriggered (out Action triggerCallback) )
                {
                    lastTrigger = trigger;
                    callback = triggerCallback;
                }
            
            return lastTrigger != null;
        }
    }

    /// <summary>
    /// Class for handling one key combination.
    /// </summary>
    public class InputTriggerKeyCombination : InputTrigger
    {
        public List<IInputTriggerEntry> Entries { get; private set; } = new List<IInputTriggerEntry> ();

        public InputTriggerKeyCombination (Action callback = null) : base(callback) { }

        public override bool IsTriggered (out Action callback)
        {
            callback = this.callback;

            foreach ( var entry in Entries )
                if ( !entry.IsTriggered () )
                    return false;

            return true;
        }
        
        public InputTriggerKeyCombination WhenKeyPress (KeyCode key)
        {
            Entries.Add (new KeyInputTriggerEntry (key, KeyTriggerType.Press));
            return this;
        }
        public InputTriggerKeyCombination WhenKeyDown (KeyCode key)
        {
            Entries.Add (new KeyInputTriggerEntry (key, KeyTriggerType.Down));
            return this;
        }
        public InputTriggerKeyCombination WhenKeyUp (KeyCode key)
        {
            Entries.Add (new KeyInputTriggerEntry (key, KeyTriggerType.Up));
            return this;
        }

        public InputTriggerKeyCombination WhenAnyKeyPress (params KeyCode[] keys)
        {
            Entries.Add (new AnyKeyInputTriggerEntry (KeyTriggerType.Press, keys));
            return this;
        }
        public InputTriggerKeyCombination WhenAnyKeyDown (params KeyCode[] keys)
        {
            Entries.Add (new AnyKeyInputTriggerEntry (KeyTriggerType.Down, keys));
            return this;
        }
        public InputTriggerKeyCombination WhenAnyKeyUp (params KeyCode[] keys)
        {
            Entries.Add (new AnyKeyInputTriggerEntry (KeyTriggerType.Up, keys));
            return this;
        }

        public InputTriggerKeyCombination WhenEveryKeyPress (params KeyCode[] keys)
        {
            Entries.Add (new EveryKeyInputTriggerEntry (KeyTriggerType.Press, keys));
            return this;
        }
        public InputTriggerKeyCombination WhenEveryKeyDown (params KeyCode[] keys)
        {
            Entries.Add (new EveryKeyInputTriggerEntry (KeyTriggerType.Down, keys));
            return this;
        }
        public InputTriggerKeyCombination WhenEveryKeyUp (params KeyCode[] keys)
        {
            Entries.Add (new EveryKeyInputTriggerEntry (KeyTriggerType.Up, keys));
            return this;
        }
    }

    public interface IInputTriggerEntry
    {
        bool IsTriggered ();
    }

    public class FuncInputTriggerEntry : IInputTriggerEntry
    {
        private Func<bool> func;

        public FuncInputTriggerEntry(Func<bool> func)
        {
            Assert.ArgumentNotNull (func, nameof (func));

            this.func = func;
        }

        public bool IsTriggered ()
            => func ();
    }
    public class KeyInputTriggerEntry : IInputTriggerEntry
    {
        public KeyCode key;
        public KeyTriggerType triggerType;

        public KeyInputTriggerEntry (KeyCode key, KeyTriggerType triggerType)
        {
            this.key = key;
            this.triggerType = triggerType;
        }
        public bool IsTriggered()
        {
            switch ( triggerType )
            {
                case KeyTriggerType.Press:
                    if ( !Input.GetKey (key) )
                        return false;
                    break;

                case KeyTriggerType.Down:
                    if ( !Input.GetKeyDown (key) )
                        return false;
                    break;

                case KeyTriggerType.Up:
                    if ( !Input.GetKeyUp (key) )
                        return false;
                    break;
            }
            return true;
        }
    }
    public class AnyKeyInputTriggerEntry : IInputTriggerEntry
    {
        public List<KeyCode> Keys { get; private set; } = new List<KeyCode> ();
        public KeyTriggerType triggerType;

        public AnyKeyInputTriggerEntry(KeyTriggerType triggerType, params KeyCode[] keys)
        {
            this.triggerType = triggerType;
            Keys.AddRange (keys);
        }

        public bool IsTriggered ()
        {
            foreach (var key in Keys)
                switch ( triggerType )
                {
                    case KeyTriggerType.Press:
                        if ( Input.GetKey (key) )
                            return true;
                        break;

                    case KeyTriggerType.Down:
                        if ( Input.GetKeyDown (key) )
                            return true;
                        break;

                    case KeyTriggerType.Up:
                        if ( Input.GetKeyUp (key) )
                            return true;
                        break;
                }

            return false;
        }
    }
    public class EveryKeyInputTriggerEntry : IInputTriggerEntry
    {
        public List<KeyCode> Keys { get; private set; } = new List<KeyCode> ();
        public KeyTriggerType triggerType;

        public EveryKeyInputTriggerEntry (KeyTriggerType triggerType, params KeyCode[] keys)
        {
            this.triggerType = triggerType;
            Keys.AddRange (keys);
        }

        public bool IsTriggered ()
        {
            foreach ( var key in Keys )
                switch ( triggerType )
                {
                    case KeyTriggerType.Press:
                        if ( !Input.GetKey (key) )
                            return false;
                        break;

                    case KeyTriggerType.Down:
                        if ( !Input.GetKeyDown (key) )
                            return false;
                        break;

                    case KeyTriggerType.Up:
                        if ( !Input.GetKeyUp (key) )
                            return false;
                        break;
                }

            return true;
        }
    }

    public enum KeyTriggerType
    {
        Press,
        Down,
        Up
    }
}
