using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SurfaceEdit
{
    public class UnityCallbackRegistrator : UnitySingleton<UnityCallbackRegistrator>
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;
        
        private List<Action> oneTimeActions = new List<Action> ();

        public void RegisterOneTimeUpdateAction(Action action)
            => oneTimeActions.Add (action);
        private void Update ()
        {
            OnUpdate?.Invoke ();

            if ( oneTimeActions.Count > 0 )
            {
                foreach ( var action in oneTimeActions )
                    action?.Invoke();
                oneTimeActions.Clear ();
            }
        }
        private void LateUpdate ()
        {
            OnLateUpdate?.Invoke ();
        }
    }
}
