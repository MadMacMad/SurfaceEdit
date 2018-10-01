using System;
using System.Collections.Generic;

namespace SurfaceEdit
{
    public class UnityUpdateRegistrator : UnitySingleton<UnityUpdateRegistrator>
    {
        public event Action OnUpdate;

        private List<Action> oneTimeActions = new List<Action> ();

        public void RegisterOneTimeActionOnUpdate(Action action)
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
    }
}
