using System;
using System.Collections.Generic;

namespace SurfaceEdit
{
    public class UnityUpdateRegistrator : UnitySingleton<UnityUpdateRegistrator>
    {
        private List<Action> oneTimeActions = new List<Action> ();

        public void OnUpdateRegisterOneTimeAction(Action action)
            => oneTimeActions.Add (action);
        
        private void Update ()
        {
            if ( oneTimeActions.Count > 0 )
            {
                foreach ( var action in oneTimeActions )
                    action ();
                oneTimeActions.Clear ();
            }
        }
    }
}
