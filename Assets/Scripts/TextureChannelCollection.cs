using System.Collections.Generic;

namespace Tilify
{
    public sealed class TextureChannelCollection : PropertyChangedNotifier
    {
        public IReadOnlyCollection<TextureChannel> List => channels.AsReadOnly();
        private List<TextureChannel> channels = new List<TextureChannel>();

        public TextureChannelCollection() { }
        public TextureChannelCollection(List<TextureChannel> channels)
        {
            if ( channels != null )
                this.channels = channels;
        }

        public void AddChannel(TextureChannel channel)
        {
            if ( !channels.Contains (channel) )
            {
                channels.Add (channel);
                NotifyPropertyChanged (nameof (List));
            }
        }
        public void RemoveChannel(TextureChannel channel)
        {
            if (channels.Contains(channel))
            {
                channels.Remove (channel);
                NotifyPropertyChanged (nameof (List));
            }
        }
    }
}
