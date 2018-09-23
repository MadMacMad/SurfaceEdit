using System.Collections.Generic;
using System.Linq;

namespace SurfaceEdit
{
    public sealed class TextureChannelCollection : PropertyChangedNotifier
    {
        public IReadOnlyCollection<TextureChannel> List => channels.AsReadOnly();
        private List<TextureChannel> channels = new List<TextureChannel>();

        public TextureChannelCollection() { }
        public TextureChannelCollection(TextureChannel channel)
        {
            channels.Add (channel);
        }
        public TextureChannelCollection (IEnumerable<TextureChannel> channels)
        {
            if ( channels != null )
            { 
                foreach ( var c in channels )
                    if (!this.channels.Contains(c))
                        this.channels.Add (c);
            }
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
