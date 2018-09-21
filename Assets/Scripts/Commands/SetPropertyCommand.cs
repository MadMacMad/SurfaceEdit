using System;
using System.Runtime.CompilerServices;

namespace SurfaceEdit.Commands
{
    public class SetPropertyCommand<T> : ICommand
    {
        public readonly string propertyName;
        public readonly T oldValue;
        public readonly T newValue;

        private Ref<T> property;
        private Action callback;
        private string filePath;

        public SetPropertyCommand (Ref<T> property, T newValue, Action callback, [CallerFilePath] string filePath = "", string propertyName = "")
        {
            Assert.ArgumentNotNull (property, nameof (property));

            this.propertyName = propertyName;
            this.property = property;
            this.callback = callback;
            this.filePath = filePath;
            oldValue = property.Value;
            this.newValue = newValue;
        }

        public void Do ()
        {
            property.Value = newValue;
            callback?.Invoke ();
        }
        public void Undo ()
        {
            property.Value = oldValue;
            callback?.Invoke ();
        }
        public void Dispose () { }

        public override string ToString ()
        {
            return nameof (SetPropertyCommand<T>) + $"(propertyName: {propertyName}; filePath: {filePath}; oldValue: {oldValue.ToString()}; newValue: {newValue.ToString()})";
        }
    }

}
