using System.Collections.Generic;
using System.Text;

namespace SurfaceEdit
{
    public sealed class ImmutableChannels
    {
        public readonly IReadOnlyCollection<Channel> List;
        private List<Channel> list;

        public ImmutableChannels (Channel channel)
        {
            list = new List<Channel> ();
            list.Add (channel);
            List = list.AsReadOnly ();
        }
        public ImmutableChannels (IEnumerable<Channel> channels)
        {
            Assert.ArgumentNotNull (channels, nameof (channels));

            var list = new List<Channel> ();
            list.AddRange (channels);
            this.list = list;

            List = list.AsReadOnly ();
        }

        public override string ToString ()
        {
            var builder = new StringBuilder ("ImmutableChannels(");
            for ( int i = 0; i < list.Count; i++ )
            {
                var channel = list[i];
                builder.Append (channel.ToString());
                if ( i + 1 != list.Count - 1 )
                    builder.Append (", ");
            }
            builder.Append (")");
            return builder.ToString ();
        }
    }

    public sealed class Channels : ObjectChangedNotifier
    {
        public IReadOnlyCollection<Channel> List => list.AsReadOnly();
        private List<Channel> list = new List<Channel>();

        public Channels() { }
        public Channels(Channel channel)
        {
            list.Add (channel);
        }
        public Channels (IEnumerable<Channel> channels)
        {
            Assert.ArgumentNotNull (channels, nameof (channels));

            foreach ( var c in channels )
                if (!list.Contains(c))
                    list.Add (c);
        }

        public void AddChannel(Channel channel)
        {
            if ( !list.Contains (channel) )
            {
                list.Add (channel);
                NotifyPropertyChanged (nameof (List));
            }
        }
        public void RemoveChannel(Channel channel)
        {
            if (list.Contains(channel))
            {
                list.Remove (channel);
                NotifyPropertyChanged (nameof (List));
            }
        }

        public ImmutableChannels ToImmutable ()
            => new ImmutableChannels (list);

        public static implicit operator Channels(Channel channel)
            => new Channels(channel);
    }
}
