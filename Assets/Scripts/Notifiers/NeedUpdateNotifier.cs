namespace SurfaceEdit
{
    public abstract class NeedUpdateNotifier : INotifyNeedUpdate
    {
        public event NeedUpdateEventHandler NeedUpdate;

        protected void NotifyNeedUpdate ()
            => NeedUpdate?.Invoke (this, null);
    }
}