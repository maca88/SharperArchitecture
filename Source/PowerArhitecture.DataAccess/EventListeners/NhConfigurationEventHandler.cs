using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess.Attributes;
using PowerArhitecture.DataAccess.Events;
using NHibernate.Event;
using Ninject;

namespace PowerArhitecture.DataAccess.EventListeners
{
    public class NhConfigurationEventHandler : BaseEventHandler<NhConfigurationEvent>
    {
        private readonly IList<ISaveOrUpdateEventListener> _saveOrUpdateEventListeners;
        private readonly IList<IFlushEventListener> _flushEventListeners;
        private readonly IList<IDeleteEventListener> _deleteEventListeners;
        private readonly IList<IAutoFlushEventListener> _autoFlushEventListeners;

        private readonly IList<IPreInsertEventListener> _preInsertEventListeners;
        private readonly IList<IPreUpdateEventListener> _preUpdateEventListeners;
        private readonly IList<IPreDeleteEventListener> _preDeleteEventListeners;
        private readonly IList<IPreCollectionUpdateEventListener> _preCollectionUpdateEventListeners;
        private readonly IList<IPostInsertEventListener> _postInsertEventListeners;
        private readonly IList<IPostUpdateEventListener> _postUpdateEventListeners;
        private readonly IList<IPostDeleteEventListener> _postDeleteEventListeners;
        private readonly IList<IPostCollectionUpdateEventListener> _postCollectionUpdateEventListeners;

        
        public NhConfigurationEventHandler(
            IList<ISaveOrUpdateEventListener> saveOrUpdateEventListeners,
            IList<IFlushEventListener> flushEventListeners,
            IList<IDeleteEventListener> deleteEventListeners,
            IList<IAutoFlushEventListener> autoFlushEventListeners,
            IList<IPreInsertEventListener> preInsertEventListeners,
            IList<IPreUpdateEventListener> preUpdateEventListeners,
            IList<IPreCollectionUpdateEventListener> preCollectionUpdateEventListeners,
            IList<IPreDeleteEventListener> preDeleteEventListeners,
            IList<IPostInsertEventListener> postInsertEventListeners,
            IList<IPostUpdateEventListener> postUpdateEventListeners,
            IList<IPostCollectionUpdateEventListener> postCollectionUpdateEventListeners,
            IList<IPostDeleteEventListener> postDeleteEventListeners
            )
        {
            _saveOrUpdateEventListeners = saveOrUpdateEventListeners;
            _flushEventListeners = flushEventListeners;
            _deleteEventListeners = deleteEventListeners;
            _autoFlushEventListeners = autoFlushEventListeners;
            _preInsertEventListeners = preInsertEventListeners;
            _preUpdateEventListeners = preUpdateEventListeners;
            _preCollectionUpdateEventListeners = preCollectionUpdateEventListeners;
            _preDeleteEventListeners = preDeleteEventListeners;
            _postInsertEventListeners = postInsertEventListeners;
            _postUpdateEventListeners = postUpdateEventListeners;
            _postCollectionUpdateEventListeners = postCollectionUpdateEventListeners;
            _postDeleteEventListeners = postDeleteEventListeners;
        }

        public override void Handle(NhConfigurationEvent e)
        {
            var config = e.Configuration;
            var eventListeners = config.EventListeners;

            //ISaveOrUpdateEventListener
            config.SetListeners(ListenerType.SaveUpdate, MergeListeners(eventListeners.SaveOrUpdateEventListeners, _saveOrUpdateEventListeners));
            config.SetListeners(ListenerType.Save, MergeListeners(eventListeners.SaveEventListeners, _saveOrUpdateEventListeners));
            config.SetListeners(ListenerType.Update, MergeListeners(eventListeners.UpdateEventListeners, _saveOrUpdateEventListeners));

            //IFlushEventListener
            config.SetListeners(ListenerType.Flush, MergeListeners(eventListeners.FlushEventListeners, _flushEventListeners));

            //IDeleteEventListener
            config.SetListeners(ListenerType.Delete, MergeListeners(eventListeners.DeleteEventListeners, _deleteEventListeners));

            //IAutoFlushEventListener
            config.SetListeners(ListenerType.Autoflush, MergeListeners(eventListeners.AutoFlushEventListeners, _autoFlushEventListeners));

            //IPreInsertEventListener
            config.AppendListeners(ListenerType.PreInsert, MergeListeners(eventListeners.PreInsertEventListeners, _preInsertEventListeners));

            //IPreUpdateEventListener
            config.AppendListeners(ListenerType.PreUpdate, MergeListeners(eventListeners.PreUpdateEventListeners, _preUpdateEventListeners));

            //IPreCollectionUpdateEventListener
            config.AppendListeners(ListenerType.PreCollectionUpdate, MergeListeners(eventListeners.PreCollectionUpdateEventListeners, _preCollectionUpdateEventListeners));

            //IPreDeleteEventListener
            config.AppendListeners(ListenerType.PreDelete, MergeListeners(eventListeners.PreDeleteEventListeners, _preDeleteEventListeners));

            //IPostInsertEventListener
            config.AppendListeners(ListenerType.PostInsert, MergeListeners(eventListeners.PostInsertEventListeners, _postInsertEventListeners));

            //IPostUpdateEventListener
            config.AppendListeners(ListenerType.PostUpdate, MergeListeners(eventListeners.PostUpdateEventListeners, _postUpdateEventListeners));

            //IPostCollectionUpdateEventListener
            config.AppendListeners(ListenerType.PostCollectionUpdate, MergeListeners(eventListeners.PostCollectionUpdateEventListeners, _postCollectionUpdateEventListeners));

            //IPostDeleteEventListener
            config.AppendListeners(ListenerType.PostDelete, MergeListeners(eventListeners.PostDeleteEventListeners, _postDeleteEventListeners));
        }

        private static T[] MergeListeners<T>(IList<T> currentListeners, IList<T> newListeners) 
            where T : class
        {
            currentListeners = currentListeners.ToList();
            newListeners = newListeners ?? new List<T>();
            foreach (var newListener in newListeners)
            {
                var evntListnrTypeAttr = newListener.GetType().GetCustomAttributes<NhEventListenerTypeAttribute>(false)
                    .FirstOrDefault(o => o.Type == typeof (T));
                var evntListnrAttr = newListener.GetType().GetCustomAttribute<NhEventListenerAttribute>(false) ?? new NhEventListenerAttribute();

                var replaceListener = evntListnrTypeAttr?.ReplaceListener ?? evntListnrAttr.ReplaceListener;
                if (replaceListener != null)
                {
                    var toReplace = currentListeners.FirstOrDefault(o => o.GetType() == replaceListener);
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
                var type = o.GetType();
                var attr = type.GetCustomAttribute<NhEventListenerAttribute>(false) ?? new NhEventListenerAttribute();
                var order = attr.Order;
                var evntListnrTypeAttr = type.GetCustomAttributes<NhEventListenerTypeAttribute>(false)
                    .FirstOrDefault(a => a.Type == typeof(T));
                if (evntListnrTypeAttr != null)
                {
                    order = evntListnrTypeAttr.Order;
                }
                return order;
            }).ToArray();
        }
    }
}
