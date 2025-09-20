namespace Suburb.Inputs
{
    public abstract class BaseInputPlugin<TMember,TCompositor> : IInputPlugin
        where TMember : class, new()
        where TCompositor : class, IPluginCompositor
    {
        protected TMember Member { get; private set; }
        protected TCompositor Compositor { get; private set; }
        
        public bool SetReceiver(object receiver)
        {
            Member = receiver as TMember;
            return Member != null;
        }

        public virtual bool SetSender(object sender)
        {
            Compositor = sender as TCompositor;
            return Compositor != null;
        }

        public virtual void Unlink()
        {
            Member = null;
            Compositor = null;
        }
    }
}