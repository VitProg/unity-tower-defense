using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UnityEngine;

namespace td.common.messageBus
{
    public interface IMessageBus
    {
        // IMessageBus AddGroup(TMessageGroup group);
        //
        // bool HasGroup(TMessageGroup group);
        //
        // TG GetGroup<TG>(TMessageGroup group) where TG : TMessageGroup;
        //
        // Subject<object> MainS();
        //
        // IObservable<object> GroupS<G, P>(G group)
        //     where G : TMessageGroup
        //     where P : ExtractMessagePayloads<G>;
        //
        // Observable<IMessage<TP>> MessageS<T, TP>(T messageStaticFunc)
        //     where T : Func<>
        //     where TP : ExtractPayload<T>;
        //
        // IMessageBus Publish<M>(IMessage<M> messageStaticResult) where M : AnyObject;

        void UnsubscribeAll();
        
        IObservable<TM> messageS<TM>();

        void Publish<TM>(TM message);
        void Publish<TM>();
    }

    public interface IMessage<TP> where TP : struct 
    {
        string GroupId { get; set; }
        string ID { get; set; }
        TP Payload { get; set; }
    }

    public abstract class StateMessages
    {
        [Serializable]
        public struct Lives
        {
            public float lives;
        }
        
        [Serializable]
        public struct Wave
        {
            public int count;
            public int number;
        }

        [Serializable]
        public class BayTowerClicked
        {
        }
    }
    
    public class A
    {
        private readonly IMessageBus bus = default;
        
        public A ()
        {
            bus.Publish(new StateMessages.Lives()
            {
                lives = 100f,
            });

            var destroyS = new Subject<bool>();

            var s = bus.messageS<StateMessages.Lives>()
                .TakeUntil(destroyS)
                .Where(item => item.lives < 10)
                .Select(item =>
                    new
                    {
                        lives = item.lives,
                        isLow = item.lives < 10,
                    });
            
            s.Subscribe(obj =>
            {
                Debug.Log(obj.lives);
                Debug.Log(obj.isLow);
            });

        }
    }
}