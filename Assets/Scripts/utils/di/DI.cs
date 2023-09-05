using System;
using System.Reflection;
using Leopotam.EcsProto.QoL;
using UnityEngine;

namespace td.utils.di
{
    public class MonoInjectable : MonoBehaviour
    {
        private readonly Type _diAttrType = typeof(DIAttribute);
        private readonly Type _serviceStaticGenericType = typeof(Service<>);

        protected void Start()
        {
            var type = GetType();
            foreach (var fi in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fi.IsStatic || !Attribute.IsDefined(fi, _diAttrType)) continue;

                var value = ServiceContainer.Get(fi.FieldType);

                if (value != null)
                {
                    fi.SetValue(this, value);
                }
#if UNITY_EDITOR
                else
                {

                    throw new Exception(
                        $"ошибка инъекции пользовательских данных в MonoBehaviour\"{type.Name}\", тип \"{fi.FieldType.Name}\" для поля \"{fi.Name}\" отсутствует в списке сервисов");
                }
#endif
            }
        }
    }
}
