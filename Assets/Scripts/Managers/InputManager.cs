using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurfaceEdit
{
    public sealed class InputManager
    {
        private List<InputTrigger> Triggers = new List<InputTrigger> ();

        public InputManager ()
        {
            UnityCallbackRegistrator.Instance.OnUpdate += Update;
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
                trigger.Trigger ();
        }
    }
    
    public abstract class InputTrigger
    {
        public Action triggeredCallback;
        public Action notTriggeredCallback;

        public InputTrigger AddTriggeredCallback(Action callback)
        {
            triggeredCallback += callback;
            return this;
        }
        public InputTrigger RemoveTriggeredCallback(Action callback)
        {
            triggeredCallback -= callback;
            return this;
        }

        public InputTrigger AddNotTriggeredCallback (Action callback)
        {
            notTriggeredCallback += callback;
            return this;
        }
        public InputTrigger RemoveNoyTriggeredCallback (Action callback)
        {
            notTriggeredCallback -= callback;
            return this;
        }

        public void Trigger ()
        {
            if ( IsTriggered () )
                triggeredCallback?.Invoke ();
            else
                notTriggeredCallback?.Invoke ();
        }
        protected abstract bool IsTriggered ();
    }

    public class KeyCombination : InputTrigger
    {
        public KeyCode key = KeyCode.None;
        public KeyTriggerType keyTriggerType = KeyTriggerType.Down;
        public Fact ShiftFact = Fact.IsFalse;
        public Fact CtrlFact = Fact.IsFalse;
        public Fact AltFact = Fact.IsFalse;
        
        public KeyCombination Key (KeyCode key, KeyTriggerType keyTriggerType = KeyTriggerType.Down)
        {
            this.key = key;
            this.keyTriggerType = keyTriggerType;
            return this;
        }
        public KeyCombination Shift(Fact fact = Fact.IsTrue)
        {
            ShiftFact = fact;
            return this;
        }
        public KeyCombination Alt (Fact fact = Fact.IsTrue)
        {
            AltFact = fact;
            return this;
        }
        public KeyCombination Ctrl (Fact fact = Fact.IsTrue)
        {
            CtrlFact = fact;
            return this;
        }

        protected override bool IsTriggered ()
        {
            var isKeyTriggered = false;

            if      ( keyTriggerType == KeyTriggerType.Down  ) isKeyTriggered = Input.GetKeyDown (key);
            else if ( keyTriggerType == KeyTriggerType.Press ) isKeyTriggered = Input.GetKey (key);
            else                                               isKeyTriggered = Input.GetKeyUp (key);

            if ( !isKeyTriggered )
                return false;

            bool CheckTriggered(Fact fact, bool triggered)
            {
                if ( fact == Fact.IsFalse && triggered )
                    return false;
                else if ( fact == Fact.IsTrue && !triggered )
                    return false;

                return true;
            }

            var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
            if ( !CheckTriggered (ShiftFact, isShiftPressed) ) return false;

            var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl);
            if ( !CheckTriggered (CtrlFact, isCtrlPressed) ) return false;

            var isAltPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey (KeyCode.RightAlt);
            return CheckTriggered (AltFact, isAltPressed);
        }
    }

    public enum Fact
    {
        IsTrue,
        IsFalse,
        Any
    }
    public enum KeyTriggerType
    {
        Press,
        Down,
        Up
    }
}
