using System;
using System.Threading.Tasks;

namespace DAL
{
    public abstract class EntityInfo<T> : EntityInfo
    {
        private T _entity;
        public virtual async Task<T> Entity()
        {
            if (Equals(_entity, default(T)))
            {
                _entity = (T)(await Read(GetEntityQuery(Id), ResolveTypeByProperty()));
            }
            return _entity;
        }

        /// <summary>
        /// Возвращает строку, представляющую текущий объект.
        /// </summary>
        /// <returns>
        /// Строка, представляющая текущий объект.
        /// </returns>
        public override string ToString()
        {
            return $"[{Id}] {TypeName}";
        }

        protected abstract string GetEntityQuery(int id);

        Type ResolveTypeByProperty()
        {
            Type type = null;

            if (!string.IsNullOrWhiteSpace(TypeName))
            {
                string typeName = TypeName.Replace("#SE2.", "KG.SE2.Modules.PlanGrafikModule.Model.API.") + ", KG.SE2.Modules.PlanGrafikModule";
                type = Type.GetType(typeName);
            }

            if (type == null)
                type = ResolveType();

            if(type == null)
                throw new Exception("не удалось определить модельный тип сущности");

            return type;
        }

        protected virtual Type ResolveType()
        {
            return null;
        }

    }
}