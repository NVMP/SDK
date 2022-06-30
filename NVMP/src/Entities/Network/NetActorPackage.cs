namespace NVMP.Entities
{
    /// <summary>
    /// Actor package interface. Allows for custom GECK driven behaviour applied to objects, to then be simulated by
    /// the host of the object who is simulating it.
    /// </summary>
    public interface INetActorPackage
    {
        void Run(INetActor owner);
    }

    /// <summary>
    /// A package that will make the AI repeatedly start combat on the targets provided.
    /// </summary>
    public class NetActorCombatPackage : INetActorPackage
    {
        private INetActor[] Targets;

        public void Run(INetActor owner)
        {
            owner.ClearTargets();

            foreach (var target in Targets)
            {
                owner.AddTarget(target);
            }
        }

        public NetActorCombatPackage(INetActor target)
        {
            Targets = new INetActor[] { target };
        }

        public NetActorCombatPackage(INetActor[] targets)
        {
            Targets = targets;
        }
    }
}
