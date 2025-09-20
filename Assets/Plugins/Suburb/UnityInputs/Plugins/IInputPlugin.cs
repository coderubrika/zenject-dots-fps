namespace Suburb.Inputs
{
    public interface IInputPlugin
    {
        public bool SetReceiver(object receiver);
        public bool SetSender(object sender);

        public void Unlink();
    }
}