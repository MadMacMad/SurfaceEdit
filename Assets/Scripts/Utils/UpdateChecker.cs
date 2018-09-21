using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tilify
{
    public class UpdateChecker : MonoBehaviour
    {
        public static UpdateChecker Instance
        {
            get
            {
                if ( instance == null )
                    instance = new GameObject ("UpdateChecker").AddComponent<UpdateChecker> ();
                return instance;
            }
        }
        private static UpdateChecker instance;

        private List<ConditionCheckerEntry> conditionCheckerEntries = new List<ConditionCheckerEntry>();

        public void RegisterConditionChecker(Func<bool> condition, Action callback)
        {
            Assert.ArgumentNotNull (condition, nameof (condition));
            Assert.ArgumentNotNull (callback, nameof (callback));

            conditionCheckerEntries.Add (new ConditionCheckerEntry (condition, callback));
        }

        private void Update ()
        {
            for (int i = 0; i < conditionCheckerEntries.Count; i++ )
            {
                var entry = conditionCheckerEntries[i];
                if ( entry.condition () )
                {
                    entry.callback ();
                    conditionCheckerEntries.Remove (entry);
                    i--;
                }
            }
        }

        private class ConditionCheckerEntry
        {
            public Func<bool> condition;
            public Action callback;

            public ConditionCheckerEntry (Func<bool> condition, Action callback)
            {
                this.condition = condition;
                this.callback = callback;
            }
        }
    }
}
