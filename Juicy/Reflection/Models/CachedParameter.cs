using Juicy.Reflection.Interfaces;
using System;

 namespace Juicy.Reflection.Models {

    internal class CachedParameter : AttributeHolder, ICachedParameter {

        protected CachedParameter(ICachedParameterComponent component) : base(component) {
            Type = component._Type;
            Position = component._Position;
        }

        public Type Type { get; }

        public int Position { get; }

        #region Builder

        protected interface ICachedParameterComponent : IAttributeHolderComponent {
            Type _Type { get; }

            int _Position { get; }
        }

        public class CachedParameterComponent<T> : AttributeHolderComponent<T>, ICachedParameterComponent
                where T : CachedParameterComponent<T> {
            public Type _Type { get; private set; }

            public int _Position { get; private set; }

            public T Type(Type type) {
                _Type = type;
                return this as T;
            }

            public T Position(int position) {
                _Position = position;
                return this as T;
            }
        }

        new public class Builder : CachedParameterComponent<Builder>, IBuilder<CachedParameter> {

            public CachedParameter Build() {
                return new CachedParameter(this);
            }
        }

        #endregion Builder
    }
}