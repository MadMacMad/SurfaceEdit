using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tilify.Commands
{
    public class SetPropertyCommand<T> : ICommand
    {
        public readonly string propertyName;
        public readonly T oldValue;
        public readonly T newValue;

        private Ref<T> property;
        private Action notifyPropertyChangedAction;
        private string filePath;

        public SetPropertyCommand (Ref<T> property, T newValue, Action notifyPropertyChangedAction, [CallerFilePath] string filePath = "", string propertyName = "")
        {
            this.propertyName = propertyName;
            this.property = property;
            this.notifyPropertyChangedAction = notifyPropertyChangedAction;
            this.filePath = filePath;
            oldValue = property.Value;
            this.newValue = newValue;
        }

        public void Do ()
        {
            property.Value = newValue;
            notifyPropertyChangedAction?.Invoke ();
        }
        public void Undo ()
        {
            property.Value = oldValue;
            notifyPropertyChangedAction?.Invoke ();
        }
        public void Dispose () { }

        public override string ToString ()
        {
            return nameof (SetPropertyCommand<T>) + $"(propertyName: {propertyName}; filePath: {filePath}; oldValue: {oldValue.ToString()}; newValue: {newValue.ToString()})";
        }
    }

}
