using UnityEngine;
using System;


public interface IUpdatable
{
    void UpdateMe();
}
namespace GokUtil.UpdateManager
{
    public class UpdateManager : MonoBehaviour
    {
        const int InitialSize = 16;

        private int tail = 0;
        private IUpdatable[] updatableArray = new IUpdatable[InitialSize];

        [SerializeField] bool reduceArraySizeWhenNeed = false;

        public static bool ReduceArraySizeWhenNeed
        {
            get { return manager.reduceArraySizeWhenNeed; }
            set { manager.reduceArraySizeWhenNeed = value; }
        }

        private static UpdateManager manager
        {
            get
            {
                if (!_manager)
                {
                    _manager = FindObjectOfType<UpdateManager>();
                    if (!_manager)
                    {
                        _manager = new GameObject("UpdateManager").AddComponent<UpdateManager>();
                    }
                }
                return _manager;
            }
        }
        static UpdateManager _manager;

        void Awake()
        {
            if (manager && _manager != this)
                Destroy(gameObject);
        }

        void Update()
        {
            for (int i = 0; i < tail; i++)
            {
                if (updatableArray[i] == null) continue;
                updatableArray[i].UpdateMe();
            }
        }

        /// <summary>
        /// Update 対象の追加.
        /// </summary>
        public static void AddUpdatable(IUpdatable updatable)
        {
            if (updatable == null) return;
            manager.addUpdatable(updatable);
        }

        void addUpdatable(IUpdatable updatable)
        {
            if (updatableArray.Length == tail)
            {
                Array.Resize(ref updatableArray, checked(tail * 2));
            }
            updatableArray[tail++] = updatable;
        }

        /// <summary>
        /// 指定した Updatable を Update 対象から除外する.
        /// </summary>
        public static void RemoveUpdatable(IUpdatable updatable)
        {
            if (updatable == null) return;
            manager.removeUpdatable(updatable);
        }

        void removeUpdatable(IUpdatable updatable)
        {
            for (int i = 0; i < updatableArray.Length; i++)
            {
                if (updatableArray[i] == updatable)
                {
                    updatableArray[i] = null;
                    refreshUpdatableArray();
                    return;
                }
            }
        }

        /// <summary>
        /// 配列整理.
        /// </summary>
        public static void RefreshUpdatableArray()
        {
            manager.refreshUpdatableArray();
        }

        void refreshUpdatableArray()
        {
            var j = tail - 1;

            // 指定した部分は null に,
            // null の部分には配列内の一番後ろにある要素を代入.
            for (int i = 0; i < updatableArray.Length; i++)
            {
                if (updatableArray[i] == null)
                {
                    while (i < j)
                    {
                        var fromTail = updatableArray[j];
                        if (fromTail != null)
                        {
                            updatableArray[i] = fromTail;
                            updatableArray[j] = null;
                            j--;
                            goto NEXTLOOP;
                        }
                        j--;
                    }

                    tail = i;
                    break;
                }

                NEXTLOOP:
                continue;
            }

            if (reduceArraySizeWhenNeed && tail < updatableArray.Length / 2)
                Array.Resize(ref updatableArray, updatableArray.Length / 2);
        }
    }
}