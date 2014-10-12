using System.Collections.Generic;
using System.Linq;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Events;
using Castle.Core.Internal;
using NHibernate.Event;
using Ninject;

namespace PowerArhitecture.DataAccess.EventListeners
{
    public class NhConfigurationEventListener : IListener<NhConfigurationEvent>
    {
        private readonly IList<IPreInsertEventListener> _preInsertEventListeners;
        private readonly IList<IPreUpdateEventListener> _preUpdateEventListeners;
        private readonly IList<IPreDeleteEventListener> _preDeleteEventListeners;
        private readonly IList<ISaveOrUpdateEventListener> _saveOrUpdateEventListeners;
        private readonly IList<IPreCollectionUpdateEventListener> _preCollectionUpdateEventListeners;
        

        public NhConfigurationEventListener(
            IList<IPreInsertEventListener> preInsertEventListeners,
            IList<IPreUpdateEventListener> preUpdateEventListeners,
            IList<ISaveOrUpdateEventListener> saveOrUpdateEventListeners,
            IList<IPreCollectionUpdateEventListener> preCollectionUpdateEventListeners,
            IList<IPreDeleteEventListener> preDeleteEventListeners)
        {
            _preInsertEventListeners = preInsertEventListeners;
            _preUpdateEventListeners = preUpdateEventListeners;
            _saveOrUpdateEventListeners = saveOrUpdateEventListeners;
            _preCollectionUpdateEventListeners = preCollectionUpdateEventListeners;
            _preDeleteEventListeners = preDeleteEventListeners;
        }

        public void Handle(NhConfigurationEvent e)
        {
            var config = e.Message;
            var eventListeners = config.EventListeners;

            //ISaveOrUpdateEventListener
            config.SetListeners(ListenerType.SaveUpdate, MergeListeners(eventListeners.SaveOrUpdateEventListeners, _saveOrUpdateEventListeners));
            config.SetListeners(ListenerType.Save, MergeListeners(eventListeners.SaveEventListeners, _saveOrUpdateEventListeners));
            config.SetListeners(ListenerType.Update, MergeListeners(eventListeners.UpdateEventListeners, _saveOrUpdateEventListeners));

            //IPreInsertEventListener
            config.AppendListeners(ListenerType.PreInsert, MergeListeners(eventListeners.PreInsertEventListeners, _preInsertEventListeners));

            //IPreUpdateEventListener
            config.AppendListeners(ListenerType.PreUpdate, MergeListeners(eventListeners.PreUpdateEventListeners, _preUpdateEventListeners));

            //IPreCollectionUpdateEventListener
            config.AppendListeners(ListenerType.PreCollectionUpdate, MergeListeners(eventListeners.PreCollectionUpdateEventListeners, _preCollectionUpdateEventListeners));

            //IPreDeleteEventListener
            config.AppendListeners(ListenerType.PreDelete, MergeListeners(eventListeners.PreDeleteEventListeners, _preDeleteEventListeners));
        }

        private static T[] MergeListeners<T>(IList<T> currentListeners, IList<T> newListeners) 
            where T : class
        {
            currentListeners = currentListeners.ToList();
            newListeners = newListeners ?? new List<T>();
            foreach (var newListener in newListeners)
            {
                var evntListnrAttr = newListener.GetType().GetAttribute<NhEventListenerAttribute>() ?? new NhEventListenerAttribute();
                if (evntListnrAttr.ReplaceListener != null)
                {
                    var toReplace = currentListeners.FirstOrDefault(o => o.GetType() == evntListnrAttr.ReplaceListener);
                    if (toReplace != null)
                        currentListeners.Replace(toReplace, newListener);
                }
                else
                {
                    currentListeners.Add(newListener);
                }
            }

            return currentListeners.OrderBy(o =>
            {
                var order = o.GetType().GetAttribute<NhEventListenerAttribute>() ?? new NhEventListenerAttribute();
                return order.Order;
            }).ToArray();
        }
    }
}
