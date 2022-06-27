using SWS;

//implements a PlayMaker FSM method call via event insert
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Simple Waypoint System")]
    [Tooltip("Adds an event to a walker object for calling your own FSM state at waypoints.")]
    public class SWS_AddEventAtWaypoint : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Walker object")]
        public FsmOwnerDefault walkerObject;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("Specific Waypoint")]
        public FsmBool waypointOnly;

        [UIHint(UIHint.FsmInt)]
        [Tooltip("Waypoint index")]
        public FsmInt wpIndex;

        [RequiredField]
        [UIHint(UIHint.FsmGameObject)]
        [Tooltip("Receiver with the FSM event")]
        public PlayMakerFSM fsmReceiver;

        [RequiredField]
        [UIHint(UIHint.FsmString)]
        [Tooltip("Receiver FSM event to call")]
        public FsmString fsmEvent;


        public override void Reset()
        {
            walkerObject = null;
            wpIndex = null;
            fsmReceiver = null;
            fsmEvent = null;
        }


        public override void OnEnter()
        {
            Execute();

            Finish();
        }


        void Execute()
        {
            var go = Fsm.GetOwnerDefaultTarget(walkerObject);
            if (go == null) return;

            splineMove spline = go.GetComponentInChildren<splineMove>();
            if (spline)
                spline.movementChangeEvent += EventListener;
            else
            {
                navMove nav = go.GetComponentInChildren<navMove>();
                if (nav)
                    nav.movementChangeEvent += EventListener;
            }
        }


        public void EventListener(int index)
        {
            if(!waypointOnly.Value || index == wpIndex.Value)
                fsmReceiver.SendEvent(fsmEvent.Value);
        }
    }
}
