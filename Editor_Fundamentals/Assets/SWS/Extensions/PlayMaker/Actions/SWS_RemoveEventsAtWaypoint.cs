using UnityEngine;
using SWS;

//manipulates the events list for removing events from a listener
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Simple Waypoint System")]
    [Tooltip("Removes all events of a walker object from a specific event listener.")]
    public class SWS_RemoveEventsAtWaypoint : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Walker object")]
        public FsmOwnerDefault walkerObject;

        [RequiredField]
        [UIHint(UIHint.FsmGameObject)]
        [Tooltip("Listener that was added using SWS_AddEventAtWaypoint")]
        public PlayMakerFSM listener;


        public override void Reset()
        {
            walkerObject = null;
            listener = null;
        }


        public override void OnEnter()
        {
            Execute();

            Finish();
        }


        void Execute()
        {
            var go = Fsm.GetOwnerDefaultTarget(walkerObject);
            if (go == null || listener == null) return;

            SWS_AddEventAtWaypoint action = null;
            for(int i = 0; i < listener.FsmStates.Length; i++)
            {
                for(int j = 0; j < listener.FsmStates[i].Actions.Length; j++)
                {
                    if(listener.FsmStates[i].Actions[j] is SWS_AddEventAtWaypoint)
                    {
                        action = listener.FsmStates[i].Actions[j] as SWS_AddEventAtWaypoint;
                        break;
                    }
                }
            }

            if (action == null) return;
            splineMove spline = go.GetComponentInChildren<splineMove>();
            if (spline)
            {
                spline.movementChangeEvent -= action.EventListener;
            }
            else
            {
                navMove nav = go.GetComponentInChildren<navMove>();
                if (nav)
                    nav.movementChangeEvent -= action.EventListener;
            }
        }
    }
}
